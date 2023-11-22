using System.Net.Sockets;
using System.Net;
using System.Diagnostics;


namespace NetLib
{
    public class Server
    {
        public string Addr { get; set; }

        //public Dictionary<string, Handler> Handlers = new Dictionary<string, Handler>();

        //public class Handler
        //{
        //    private Action<Request, Response> H;
        //    public Handler(Action<Request, Response> h) 
        //    { 
        //        H = h;
        //    }
        //}

        private List<IConn> Connection_Pool;

        public static int ReceiveBufferHeaderSize = 6; // header size
        public static int MaxConnectionCount = 16;
        public static int KeepAlive = 1000;

        private IConn Conn;
        public Server(string addr)
        {
            Addr = addr;

            Connection_Pool = new List<IConn>();
        }

        //public void RegisterHandler(string handlerName, Action<Request, Response> func)
        //{
        //    try
        //    {
        //        if (Handlers.ContainsKey(handlerName))
        //        {
        //            throw (new Exception(ServerErrors.HandlerSameName.GetErrorsDescription()));
        //        }

        //        Handlers[handlerName] = new Handler(func);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLineException(e);
        //    }
        //}

        public void Serve()
        {
            try
            {
                var ln = Listen("tcp", Addr);


                Thread serveThread = new Thread(serve);
                serveThread.Start(ln);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Shutdown()
        {

        }

        // Create TCPListener or Other
        private IListen Listen(string network, string addr)
        {
            List<IAddr> addrList = resolveAddrs(network, addr);

            var la = First(addrList, IsIPv4);

            SysListener sl = new SysListener(la, addr, network);

            var l = sl.GetListener(la);

            return l;
        }

        private void serve(System.Object l)
        {
            while (true)
            {
                var conn = ((IListen)l).Accept();

                Debug.WriteLine("Client IP = " + conn.RemoteAddr() + " Connect Succese!");
                //if(Connection_Pool.Count() < MaxConnectionCount)
                //{
                //    Connection_Pool.Add(conn);

                //    //conn.ReceiveTimeout = KeepAlive;


                //}
                //else
                //{
                //    //throw new Exception(ServerErrors.ConnectionCountNotEnough.GetErrorsDescription());
                //}

                Thread handleConnThread = new Thread(DoConn);
                handleConnThread.Start(conn);
            }
        }

        private void DoConn(System.Object c)
        {
            IConn conn;

            if (c.GetType() != typeof(Conn)) throw new Exception(ConnErrors.SocketTypeIsNotCorrect.GetErrorsDescription());

            conn = (Conn)c;

            Conn = conn;

            while (true)
            {
                if (Conn.Connected())
                {
                    ReadRequest();
                }
                else
                {
                    break;
                }

                //Response res;
                //Request r;

                //NewRequestAndResponse(out res, out r);


                //if (msgLen > 0)
                //{
                //    string msg = Encoding.UTF8.GetString(buffer, 0, msgLen);
                //    if(msg == "close")
                //    {
                //        Debug.WriteLine($"Client : {0} + 下線了 {conn.RemoteAddr()}");
                //        conn.Close();
                //        break;
                //    }
                //    msgSet += msg;

                //    if (ReceiveBufferLen > msgLen)
                //    {
                //        Debug.WriteLine(msgSet);
                //        msgSet = "";
                //    }
                //}
            }
        }
        private void ReadRequest()
        {
            while (true)
            {
                ReadPacket();
            }
        }
        private void ReadPacket()
        {
            int payloadLen = ReadPacketHeader();
        }

        private int ReadPacketHeader()
        {

            byte[] buffer = new byte[ReceiveBufferHeaderSize];
            int headerLen = Conn.Read(buffer);

            if (headerLen == -1)
            {
                // print Conn error - Socket Error
                return -1;
            }

            if (headerLen <= 0)
            {

            }
            string msg = "";
            for (int i = 0; i < headerLen; i++)
            {
                msg += buffer[i] + " ";
            }
            Debug.WriteLine(msg);


            // client 端依照每個 stream 來丟出 frame
            // 從 buffer 取出每個 frame 的資訊


            //var f = new FrameHeader
            //{
            //    StreamID = buffer[0],
            //};
            return 0;
        }

        public class PacketHeader
        {
            public int Length { get; set; }
            public byte PacketType { get; set; }
            public byte PacketFlag { get; set; }
        }

        private void NewRequestAndResponse(out Response resp, out Request req)
        {
            while (true)
            {

                // 判斷 buffer 裡面是否屬於同一個request


            }

            //resp = new Response(Conn);
            //req = new Request("test", null);


        }

        private List<IAddr> resolveAddrs(string network, string addr)
        {
            var afnet = parseNetwork(network);

            Parse.HostAndPort hp = Parse.ParseAddress(addr);

            var ipAddrList = InetAddress(hp.Host);

            NetAddrFactory factory = new NetAddrFactory();
            List<IAddr> addrList = new List<IAddr>();
            ipAddrList.ForEach(addr =>
            {
                addrList.Add(factory.CreateAddr(afnet, addr));
            });

            return addrList;
        }



        private IAddr First(List<IAddr> list, Func<IAddr, bool> filterFunc)
        {
            var addr = list.Where(p =>
            {
                return filterFunc(p);
            }).ToList().First();
            return addr;
        }

        private bool IsIPv4(IAddr addr)
        {
            return addr.GetAddressFamily().Equals(AddressFamily.InterNetwork);
        }
        private bool IsIPv6(IAddr addr)
        {
            return addr.GetAddressFamily().Equals(AddressFamily.InterNetworkV6);
        }

        private List<IPAddress> InetAddress(string addr)
        {
            // checking addr can parse to IPAddress
            IPAddress ip;
            if (!IPAddress.TryParse(addr, out ip))
            {
                throw (new Exception(ConnErrors.AddressInvalid.GetErrorsDescription()));
            }
            // checking addr is 0.0.0.0 for IPv4 or :: for IPv6
            if (ip.ToString() == "0.0.0.0")
            {
                ip = IPAddress.Any;
            }

            IPHostEntry hostEntry = Dns.GetHostEntry(ip);
            List<IPAddress> ipAddrList = hostEntry.AddressList.ToList();

            //ipAddrList.ForEach(p =>
            //{
            //    Debug.WriteLine($"AddressFamily: {p.AddressFamily}\n" +
            //        $"IP: {p}\n" +
            //        $"IPv4: {p.MapToIPv4()}\n" +
            //        $"IsIPv4MappedToIPv6: {p.IsIPv4MappedToIPv6}\n" +
            //        $"IsIPv6LinkLocal: {p.IsIPv6LinkLocal}");
            //});


            return ipAddrList;
        }

        private ProtocolType parseNetwork(string network)
        {
            ProtocolType networkProtocol;
            switch (network)
            {
                case "tcp":
                    networkProtocol = ProtocolType.Tcp;
                    break;
                case "udp":
                    networkProtocol = ProtocolType.Udp;
                    break;
                case "ip":
                    networkProtocol = ProtocolType.IP;
                    break;
                default:
                    throw (new Exception(ConnErrors.NetworkInvalid.GetErrorsDescription()));
            }
            return networkProtocol;
        }

        public class NetAddrFactory
        {
            public NetAddrFactory() { }
            public IAddr CreateAddr(ProtocolType network, IPAddress ip)
            {
                switch (network)
                {
                    case ProtocolType.Tcp:
                        return new TCPAddr(ip, network);
                    case ProtocolType.Udp:
                        return new UDPAddr(ip, network);
                    case ProtocolType.IP:
                        return new IPAddr(ip, network);
                    default:
                        throw (new Exception(ConnErrors.NotSupportProtocal.GetErrorsDescription()));
                }
            }
        }

        public class SysListener
        {
            private IAddr IAddr;
            private string Address;
            private string Network;
            public SysListener(IAddr iAddr, string address, string network)
            {
                IAddr = iAddr;
                Address = address;
                Network = network;
            }

            public IListen GetListener(IAddr la)
            {
                IListen l = null;

                switch (la.GetType().ToString())
                {
                    case "TCPAddr":
                        l = ListenTCP(la);
                        break;
                    default:
                        break;
                }
                return l;
            }

            private TCPListener ListenTCP(IAddr la)
            {
                Socket socket = SysSocket.GetSysSocket(la.GetAddressFamily(), SocketType.Stream, la.GetNetwork());

                //解析 address
                Parse.HostAndPort hostAndPort = Parse.ParseAddress(Address);

                IPEndPoint ep = new IPEndPoint(la.GetAddress(), hostAndPort.Port);

                //Bind
                socket.Bind(ep);

                //Listen
                socket.Listen(MaxConnectionCount);

                return new TCPListener(socket);
            }
        }
    }


    public interface IAddr
    {
        public IPAddress GetAddress();
        public AddressFamily GetAddressFamily();
        public ProtocolType GetNetwork();
    }

    public class TCPAddr : IAddr
    {
        private IPAddress Address;
        private ProtocolType Network;
        public TCPAddr(IPAddress addr, ProtocolType network)
        {
            Address = addr;
            Network = network;
        }
        public IPAddress GetAddress()
        {
            return Address;
        }
        public AddressFamily GetAddressFamily()
        {
            return Address.AddressFamily;
        }
        public ProtocolType GetNetwork()
        {
            return Network;
        }
    }

    public class UDPAddr : IAddr
    {
        private IPAddress Address;
        private ProtocolType Network;
        public UDPAddr(IPAddress addr, ProtocolType network)
        {
            Address = addr;
            Network = network;
        }
        public IPAddress GetAddress()
        {
            return Address;
        }
        public AddressFamily GetAddressFamily()
        {
            return Address.AddressFamily;
        }
        public ProtocolType GetNetwork()
        {
            return Network;
        }
    }
    public class IPAddr : IAddr
    {
        private IPAddress Address;
        private ProtocolType Network;
        public IPAddr(IPAddress addr, ProtocolType network)
        {
            Address = addr;
            Network = network;
        }
        public IPAddress GetAddress()
        {
            return Address;
        }
        public AddressFamily GetAddressFamily()
        {
            return Address.AddressFamily;
        }
        public ProtocolType GetNetwork()
        {
            return Network;
        }
    }
}
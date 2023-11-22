using System.Net.Sockets;


namespace NetLib
{
    public interface IListen
    {
        public IConn Accept();
    }



    public class TCPListener : IListen
    {
        Socket socket;
        public TCPListener(Socket s)
        {
            socket = s;
        }
        public IConn Accept()
        {
            Socket s = socket.Accept();
            IConn conn = new Conn(s);
            return conn;
        }
    }



}

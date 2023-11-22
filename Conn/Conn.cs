using System.Net.Sockets;

namespace NetLib
{
    public interface IConn
    {
        public int Read(byte[] buf);
        public void Write(byte[] buf);
        public void Close();
        public string RemoteAddr();
        public bool Connected();
        public SocketError GetErrorCode();
    }

    public class Conn : IConn
    {
        Socket socket;

        SocketError connError;
        public Conn(Socket s)
        {
            socket = s;
            connError = 0;
        }

        public int Read(byte[] buf)
        {

            SocketError errorCode;
            int len = socket.Receive(buf, SocketFlags.None, out errorCode);

            // when client close frocily
            if (errorCode == SocketError.ConnectionReset)
            {
                connError = errorCode;
                return -1;
            }

            return len;

        }
        public void Write(byte[] buf)
        {

        }

        public void Close()
        {
            socket.Close();
        }

        public string RemoteAddr()
        {
            return socket.RemoteEndPoint.ToString();
        }

        public bool Connected()
        {
            return socket.Connected;
        }

        public SocketError GetErrorCode()
        {
            return connError;
        }
    }

}

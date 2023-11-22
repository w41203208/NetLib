using System.Net.Sockets;

namespace NetLib
{
    public static class SysSocket
    {
        public static Socket GetSysSocket(AddressFamily familty, SocketType type, ProtocolType proto)
        {
            return new Socket(familty, type, proto);
        }
    }
}

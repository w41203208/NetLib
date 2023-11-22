using System;
using System.Diagnostics;

namespace NetLib
{
    public static class Parse
    {
        public class HostAndPort
        {
            public int Port { get; set; }
            public string Host { get; set; }

        }
        public static HostAndPort ParseAddress(string address)
        {
            string host;
            int port;
            try
            {
                var i = Last(address, ':');
                if (address[0] == '[')
                {
                    throw (new Exception(UtilErrors.AddressNotSupport.GetErrorsDescription()));
                }
                else
                {
                    host = address[..i];
                }
                port = int.Parse(address[(i + 1)..]);
                return new HostAndPort { Port = port, Host = host };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }

        }

        private static int Last(string s, char n)
        {
            var l = s.Length;
            int i;
            for (i = --l; i >= 0; i--)
            {
                if (s[i] == n)
                {
                    break;
                }
            }
            return i;
        }
    }
}

using System.Collections;
using System.Collections.Generic;

namespace NetLib
{
    public class Response
    {
        public IConn conn;
        public Response(IConn c)
        {
            conn = c;
        }
        public void Write(byte[] buf)
        {
            conn.Write(buf);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace NetLib
{
    

    public class Request
    {
        public string CommandName { get; set; }
        public byte[] Body { get; set; }

        public Request(string cName, byte[] body)
        {
            CommandName = cName;
            Body = body;
        }
    }


}

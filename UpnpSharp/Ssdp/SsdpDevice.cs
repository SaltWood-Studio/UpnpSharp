using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UpnpSharp.Ssdp
{
    public class SsdpDevice
    {
        IPAddress host;
        ushort port;
        string response;

        public SsdpDevice(IPEndPoint address, string message)
        {
            this.host = address.Address;
            this.port = (ushort)address.Port;
            this.response = message;

            this.GetDescription()
        }
    }
}

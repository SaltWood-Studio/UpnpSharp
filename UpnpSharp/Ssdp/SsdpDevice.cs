using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UpnpSharp.Ssdp
{
    public class SsdpDevice
    {
        public IPAddress Address { get; protected set; }
        public ushort Port { get; protected set; }
        public string? Description { get; protected set; }
        string response;

        public SsdpDevice(IPEndPoint address, string message)
        {
            this.Address = address.Address;
            this.Port = (ushort)address.Port;
            this.response = message;

            HttpPacketParser parser = new HttpPacketParser(message);

            this.GetDescription(parser["Location"]).Wait();
            this.RequestType().Wait();
        }

        public async Task<string?> GetDescription(string? url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            this.Description = await response.Content.ReadAsStringAsync();
            return this.Description;
        }

        public async Task RequestType()
        {
            XmlSerializer xml = new XmlSerializer(typeof(DeviceXml));

        }
    }

    public class DeviceXml
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace UpnpSharp.Ssdp
{
    public class SsdpDevice
    {
        public IPAddress Address { get; protected set; }
        public ushort Port { get; protected set; }
        public string? Description { get; protected set; }
        public SsdpXml? SsdpDeviceInfo { get; protected set; }
        public string? FriendlyName => this.SsdpDeviceInfo?.Device.FriendlyName;
        public string? Type => this.SsdpDeviceInfo?.Device.DeviceType;
        public SsdpDeviceServiceXml? Services => this.SsdpDeviceInfo?.Device.ServiceList;
        public string? BaseUrl => this.SsdpDeviceInfo?.Device.UrlBase != null ? this.SsdpDeviceInfo.Device.UrlBase : this.parser["Location"];

        string response;
        protected HttpPacketParser parser;

        public SsdpDevice(IPEndPoint address, string message)
        {
            this.Address = address.Address;
            this.Port = (ushort)address.Port;
            this.response = message;

            this.parser = new HttpPacketParser(message);

            this.GetDescription(this.parser["Location"]).Wait();
            this.ParseXml();
        }

        public async Task<string?> GetDescription(string? url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            this.Description = await response.Content.ReadAsStringAsync();
            return this.Description;
        }

        public void ParseXml()
        {
            XmlSerializer xml = new XmlSerializer(typeof(SsdpXml));
            xml.CanDeserialize(XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(this.Description!))));
            SsdpXml? deviceXml = xml.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(this.Description!))) as SsdpXml;
            this.SsdpDeviceInfo = deviceXml;
        }

        public override string ToString()
        {
            return string.Format("Device {0}", this.FriendlyName);
        }

        public string ToString(string format)
        {
            return string.Format(format, this.FriendlyName, this.Description, this.Type, this.SsdpDeviceInfo?.Device.ModelName, this.SsdpDeviceInfo?.Device.ModelNumber);
        }

        //public var this[string id]
        //{
        //    get => this.SsdpDeviceInfo.Device.ServiceList.Service.
        //}
    }
}

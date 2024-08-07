using System.Net;
using System.Text;
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
        public IEnumerable<SsdpDeviceService>? Services => GetServices();

        public IEnumerable<SsdpDeviceService>? GetServices()
        {
            return this.SsdpDeviceInfo?.Device.GetServices();
        }

        public string? BaseUrl => string.IsNullOrWhiteSpace(this.SsdpDeviceInfo?.Device.UrlBase) ? this.parser["Location"] : this.SsdpDeviceInfo?.Device.UrlBase;

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
            this.SsdpDeviceInfo.Device.GetServices().Select(service => new SsdpService(BaseUrl, service)).ToList();
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

        public SsdpDeviceService? this[string id] => this.Services?.Where(service => service.ServiceId.Split(':')[3] == id).FirstOrDefault();
    }
}

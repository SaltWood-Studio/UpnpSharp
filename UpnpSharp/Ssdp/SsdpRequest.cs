using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UpnpSharp.Ssdp
{
    public class SsdpRequest : IDisposable
    {
        const uint mcastAddress = 0xfaffffef;
        const ushort ssdpPort = 1900;
        protected UdpClient client;
        protected bool disposed;

        public SsdpRequest()
        {
            this.client = new UdpClient();
        }

        public string? RequestMethod { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public string RawRequest
        {
            get
            {
                return $"{this.RequestMethod} * HTTP/1.1\r\n" +
                    string.Join("\r\n", this.Headers.Select(kvp => $"{kvp.Key}: {kvp.Value}")) +
                    "\r\n\r\n";
            }
        }

        public IEnumerable<SsdpDevice> MSearch(int delay = 1000, string st = "ssdp:all", IDictionary<string, string>? headers = null)
        {
            this.RequestMethod = "M-SEARCH";
            this.Headers["HOST"] = $"239.255.255.250:{ssdpPort}";
            this.Headers["MAN"] = "\"ssdp:discover\"";
            this.Headers["MX"] = (delay / 1000).ToString();
            this.Headers["ST"] = st;
            if (headers != null)
            {
                foreach ((string key, string value) in headers)
                {
                    this.Headers[key] = value;
                }
            }

            this.client.Client.ReceiveTimeout = delay;
            return this.Send(this.RawRequest);
        }

        public IEnumerable<SsdpDevice> Send(string message)
        {
            this.client.Send(Encoding.UTF8.GetBytes(message), new IPEndPoint(new IPAddress(mcastAddress), ssdpPort));
            List<SsdpDevice> devices = new List<SsdpDevice>();
            while (true)
            {
                try
                {
                    IPEndPoint? address = null;
                    byte[] buffer = this.client.Receive(ref address);
                    if (buffer.Length > 65507) continue;
                    SsdpDevice device = new SsdpDevice(address, Encoding.UTF8.GetString(buffer));
                    devices.Add(device);
                }
                catch (SocketException)
                {
                    break;
                }
            }
            return devices;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.client.Close();
                    this.client.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~SsdpRequest()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

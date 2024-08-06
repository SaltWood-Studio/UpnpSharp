using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UpnpSharp.Ssdp
{
    public class SsdpRequest
    {
        private UdpClient client;
        const uint mcastAddress = 0xfaffffef;
        const ushort ssdpPort = 1900;

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
            this.Headers["MAN"] = "ssdp:discover";
            this.Headers["MX"] = delay.ToString();
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
    }
}

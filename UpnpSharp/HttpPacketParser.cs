using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace UpnpSharp
{
    public class HttpPacketParser
    {
        public string? Method { get; set; }
        public string? Path { get; set; }
        public string? Protocol { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string? Payload { get; set; }

        public HttpPacketParser(string packet)
        {
            string title = string.Empty;
            string[] splittedPacket = packet.Split("\r\n");
            title = splittedPacket[0];
            string[] temp = title.Split(' ');
            (Method, Path, Protocol) = (temp[0], temp[1], temp[2]);
            for (int i = 1; i < temp.Length; i++)
            {
                if ((splittedPacket[i]) == string.Empty) break;
                string[] keyValuePair = splittedPacket[i].Split(':', 2, StringSplitOptions.TrimEntries);
                this.Headers.Add(keyValuePair[0], keyValuePair[1]);
            }
            this.Payload = packet.Split("\r\n\r\n", 2).Last();
        }

        public string? GetHeaderValue(string key)
        {
            // 使用不区分大小写的比较器查找键
            var headerKey = this.Headers.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            return headerKey != null ? this.Headers[headerKey] : null;
        }

        public string? this[string key] => this.GetHeaderValue(key);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UpnpSharp.Ssdp;

namespace UpnpSharp.Upnp
{
    public class Upnp
    {
        protected SsdpRequest ssdpRequest;
        protected List<SsdpDevice> devices = new();

        public Upnp()
        {
            this.ssdpRequest = new SsdpRequest();
        }

        /// <summary>
        /// 查找 UPnP 设备
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerable<SsdpDevice> Discover(int delay = 2000)
        {
            var devices = ssdpRequest.MSearch(delay, "upnp:rootdevice").ToList();
            this.devices = devices;
            foreach (var device in devices)
            {
                yield return device;
            }
        }
        
        public SsdpDevice? GetIgd()
        {
            List<SsdpDevice> igds = new List<SsdpDevice>();
            foreach (var device in devices)
            {
                string? deviceType = device.Type?.Split(':')[3];
                if (deviceType == "InternetGatewayDevice")
                {
                    igds.Add(device);
                }
            }
            return igds.FirstOrDefault();
        }
    }
}

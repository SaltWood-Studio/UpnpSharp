using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpnpSharp.Ssdp;

namespace UpnpSharp.Upnp
{
    public class Upnp
    {
        protected SsdpRequest ssdpRequest;

        public Upnp()
        {
            this.ssdpRequest = new SsdpRequest();
        }

        /// <summary>
        /// 查找 UPnP 设备
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerable<SsdpDevice> Discover(int delay = 1000)
        {
            foreach (var device in ssdpRequest.MSearch(delay, "upnp:rootdevice"))
            {
                yield return device;
            }
        }
    }
}

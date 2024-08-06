using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpnpSharp.Upnp
{
    internal class Upnp
    {
        /// <summary>
        /// 查找 UPnP 设备
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerable<UpnpDevice> Discover(int delay = 1000)
        {
            foreach (var device in ssdp.MSearch(delay = delay, st = "upnp:rootdevice"))
            {

            }
        }
    }
}

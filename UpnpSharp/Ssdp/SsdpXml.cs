using System.Xml.Serialization;

namespace UpnpSharp.Ssdp
{
    // 注意: 生成的代码可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = "urn:schemas-upnp-org:device-1-0", TypeName = "root")]
    [XmlRoot(Namespace = "urn:schemas-upnp-org:device-1-0", IsNullable = false)]
    public partial class SsdpXml
    {
        [XmlElement(ElementName = "device")]
        public SsdpDeviceXml Device { get; set; } = new SsdpDeviceXml();
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public partial class SsdpDeviceXml
    {
        [XmlElement(ElementName = "deviceType")]
        public string DeviceType { get; set; } = string.Empty;

        [XmlElement(ElementName = "friendlyName")]
        public string FriendlyName { get; set; } = string.Empty;

        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer { get; set; } = string.Empty;

        [XmlElement(ElementName = "manufacturerURL")]
        public string ManufacturerUrl { get; set; } = string.Empty;

        [XmlElement(ElementName = "modelDescription")]
        public string ModelDescription { get; set; } = string.Empty;

        [XmlElement(ElementName = "modelName")]
        public string ModelName { get; set; } = string.Empty;

        [XmlElement(ElementName = "modelNumber")]
        public string ModelNumber { get; set; } = string.Empty;

        [XmlElement(ElementName = "modelURL")]
        public string ModelUrl { get; set; } = string.Empty;

        [XmlElement(ElementName = "serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;

        [XmlElement(ElementName = "UDN")]
        public string Udn { get; set; } = string.Empty;

        [XmlElement(ElementName = "URLBase")]
        public string UrlBase { get; set; } = string.Empty;

        [XmlElement(ElementName = "UPC")]
        public string Upc { get; set; }

        [XmlElement(ElementName = "serviceList")]
        public SsdpDeviceServiceList ServiceList { get; set; } = new();

        /// <remarks/>
        [XmlElement(ElementName = "deviceList")]
        public SsdpDevices DeviceList { get; set; } = new SsdpDevices();

        [XmlElement(ElementName = "presentationURL")]
        public string PresentationURL { get; set; } = string.Empty;

        public IEnumerable<SsdpDeviceService> GetServices()
        {
            return GetServices(this);
        }

        public IEnumerable<SsdpDeviceService> GetServices(SsdpDeviceXml? xml)
        {
            var thisService = xml?.ServiceList.Service;
            if (!string.IsNullOrWhiteSpace(thisService?.ServiceType)) yield return thisService;
            SsdpDeviceXml[]? subDevices = xml?.DeviceList.Devices;
            if (subDevices != null)
            {
                foreach (var device in subDevices)
                {
                    foreach (var service in GetServices(device))
                    {
                        yield return service;
                    }
                }
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class SsdpDevices
    {
        [XmlElement(ElementName = "device")]
        public SsdpDeviceXml[] Devices { get; set; } = Array.Empty<SsdpDeviceXml>();
    }


    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public partial class SsdpDeviceServiceList
    {
        [XmlElement(ElementName = "service")]
        public SsdpDeviceService Service { get; set; } = new SsdpDeviceService();
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:device-1-0")]
    public partial class SsdpDeviceService
    {
        [XmlElement(ElementName = "serviceType")]
        public string ServiceType { get; set; } = string.Empty;

        [XmlElement(ElementName = "serviceId")]
        public string ServiceId { get; set; } = string.Empty;

        [XmlElement(ElementName = "SCPDURL")]
        public string ScpdUrl { get; set; } = string.Empty;

        [XmlElement(ElementName = "controlURL")]
        public string ControlUrl { get; set; } = string.Empty;

        [XmlElement(ElementName = "eventSubURL")]
        public string EventSubUrl { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpnpSharp.Utils;

namespace UpnpSharp.Ssdp
{
    public class SsdpService
    {
        string baseUrl, scpdUrl;
        protected SsdpDeviceService serviceXml;

        public string Type => this.serviceXml.ServiceType;
        public string Id => this.serviceXml.ServiceId;
        public string ScpdUrl => scpdUrl;
        public string ControlUrl => this.serviceXml.ControlUrl;
        public string EventSubUrl => this.serviceXml.EventSubUrl;
        public string BaseUrl => baseUrl;
        public string? Description { get; protected set; }
        public Dictionary<string, string> Actions { get; protected set; } = new();

        public SsdpService(string baseUrl, SsdpDeviceService service)
        {
            this.serviceXml = service;

            this.baseUrl = baseUrl;
            this.scpdUrl = service.ScpdUrl;

            UrlParser baseUri = new UrlParser(baseUrl);
            UrlParser scpdUri = new UrlParser(scpdUrl);
            if (baseUri.Scheme != Uri.UriSchemeHttp)
            {
                throw new Exception($"Unsupported url scheme: {baseUri.Scheme}");
            }

            if (scpdUri.Scheme == Uri.UriSchemeHttp)
            {
                if (scpdUri.Host != baseUri.Host) throw new Exception($"Host doesn't match: \"{scpdUri.Host}\" and \"{baseUri.Host}\"");
            }
            else if (string.IsNullOrWhiteSpace(scpdUri.Scheme))
            {
                if (scpdUrl.StartsWith('/')) this.scpdUrl = $"{baseUri.Scheme}://{baseUri.Host}:{baseUri.Port}{scpdUrl}";
                else this.scpdUrl = $"{baseUri.Scheme}://{baseUri.Host}:{baseUri.Port}/{scpdUri.ToString()}";
            }
            else throw new Exception($"Unsupported url scheme: {scpdUri.Scheme}");

            RequestDescription().Wait();
            RequestStateVariables();
            RequestActions();
        }

        private void RequestActions()
        {
            Console.WriteLine(this.Description);
        }

        private void RequestStateVariables()
        {
            return;
        }

        public async Task RequestDescription()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(this.ScpdUrl);
            if (!response.IsSuccessStatusCode)
            {
                this.Description = null;
                throw new Exception($"Unsuccessful request: 2xx or 3xx status code expected, got {response.StatusCode}.");
            }
            this.Description = await response.Content.ReadAsStringAsync();
        }
    }
}

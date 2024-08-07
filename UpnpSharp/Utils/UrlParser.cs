using System;
using System.Text.RegularExpressions;

namespace UpnpSharp.Utils
{
    public class UrlParser
    {
        public string Scheme { get; protected set; } = string.Empty;
        public string Host { get; protected set; } = string.Empty;
        public ushort Port { get; protected set; } = 0;
        public string Path { get; protected set; } = string.Empty;

        public UrlParser(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri), "The URI cannot be null or empty.");

            ParseUri(uri);
        }

        private void ParseUri(string uri)
        {
            var schemeMatch = Regex.Match(uri, @"^(?<scheme>[a-zA-Z][a-zA-Z0-9+.-]*):");
            if (schemeMatch.Success)
            {
                Scheme = schemeMatch.Groups["scheme"].Value;
                uri = uri.Substring(Scheme.Length + 1); // Remove scheme from uri
            }

            var authorityMatch = Regex.Match(uri, @"^//(?<authority>[^/]+)");
            if (authorityMatch.Success)
            {
                var authority = authorityMatch.Groups["authority"].Value;
                uri = uri.Substring(authority.Length + 2); // Remove authority from uri

                var hostPortMatch = Regex.Match(authority, @"^(?<host>[^:]+)(:(?<port>\d+))?$");
                if (hostPortMatch.Success)
                {
                    Host = hostPortMatch.Groups["host"].Value;
                    Port = hostPortMatch.Groups["port"].Success ? ushort.Parse(hostPortMatch.Groups["port"].Value) : GetDefaultPort(Scheme);
                }
            }

            Path = uri;
        }

        private ushort GetDefaultPort(string scheme)
        {
            switch (scheme?.ToLower())
            {
                case "http":
                    return 80;
                case "https":
                    return 443;
                case "ftp":
                    return 21;
                default:
                    return 0;
            }
        }
    }
}

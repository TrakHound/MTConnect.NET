// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Probe request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
    /// </summary>
    public class MTConnectHttpProbeClient : IMTConnectProbeClient
    {
        private const int DefaultTimeout = 15000;
        private static readonly HttpClient _defaultHttpClient;
        private readonly HttpClient _httpClient;

        static MTConnectHttpProbeClient()
        {
            _defaultHttpClient = new HttpClient();
            _defaultHttpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
        }

        /// <summary>
        /// Resolves <paramref name="serverNameOrURL"/> to its first IPv4 address using DNS. Used
        /// when callers need the agent's numeric address (for instance to constrain TLS hostname
        /// verification or to record the actual endpoint in logs). Catches and swallows DNS errors,
        /// returning <c>false</c> with <paramref name="resolvedIPAddress"/> set to <c>null</c>.
        /// </summary>
        /// <param name="serverNameOrURL">Hostname, URL, or already-numeric address to resolve.</param>
        /// <param name="resolvedIPAddress">On success the resolved IPv4 address; on failure <c>null</c>.</param>
        /// <returns><c>true</c> if at least one IPv4 record was returned; otherwise <c>false</c>.</returns>
        public static bool GetResolvedConnecionIPAddress(string serverNameOrURL, out IPAddress resolvedIPAddress)
        {
            bool isResolved = false;
            IPHostEntry hostEntry = null;
            IPAddress resolvIP = null;
            try
            {
                if (!IPAddress.TryParse(serverNameOrURL, out resolvIP))
                {
                    hostEntry = Dns.GetHostEntry(serverNameOrURL);

                    if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0)
                    {
                        if (hostEntry.AddressList.Length == 1)
                        {
                            resolvIP = hostEntry.AddressList[0];
                            isResolved = true;
                        }
                        else
                        {
                            foreach (IPAddress var in hostEntry.AddressList)
                            {
                                if (var.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    resolvIP = var;
                                    isResolved = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    isResolved = true;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                resolvedIPAddress = resolvIP;
            }

            return isResolved;
        }


        /// <summary>
        /// Initializes a new instance of the MTConnectProbeClient class that is used to perform
        /// a Probe request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpProbeClient(string authority, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = _defaultHttpClient;
            Authority = authority;
            Device = device;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectProbeClient class that is used to perform
        /// a Probe request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="hostname">
        /// The Hostname of the MTConnect Agent
        /// </param>
        /// <param name="port">
        /// The Port of the MTConnect Agent
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpProbeClient(string hostname, int port, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = _defaultHttpClient;
            Authority = CreateUri(hostname, port).ToString();
            Device = device;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectProbeClient class that is used to perform
        /// a Probe request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpProbeClient(HttpClient httpClient, string authority, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = httpClient != null ? httpClient : _defaultHttpClient;
            Authority = authority;
            Device = device;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectProbeClient class that is used to perform
        /// a Probe request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="hostname">
        /// The Hostname of the MTConnect Agent
        /// </param>
        /// <param name="port">
        /// The Port of the MTConnect Agent
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpProbeClient(HttpClient httpClient, string hostname, int port, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = httpClient != null ? httpClient : _defaultHttpClient;
            Authority = CreateUri(hostname, port).ToString();
            Device = device;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }


        /// <summary>
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </summary>
        public string Authority { get; }

        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Gets or Sets the Document Format to return
        /// </summary>
        public string DocumentFormat { get; set; }

        /// <summary>
        /// Gets of Sets the connection timeout for the request
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        public IEnumerable<HttpContentEncoding> ContentEncodings { get; set; }

        /// <summary>
        /// Gets or Sets the Content-type (or MIME-type) to pass to the Accept HTTP Header
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public event EventHandler<IErrorResponseDocument> MTConnectError;

        /// <summary>
        /// Raised when a Document Formatting Error is received
        /// </summary>
        public event EventHandler<IFormatReadResult> FormatError;

        /// <summary>
        /// Raised when an Connection Error occurs
        /// </summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public event EventHandler<Exception> InternalError;


        /// <summary>
        /// Execute the Probe Request
        /// </summary>
        public IDevicesResponseDocument Get()
        {
            // Honours ContentType like GetAsync; runs on the thread pool so a blocked caller cannot deadlock
            return MTConnectHttpDocumentRequest.Send(_httpClient, CreateRequest, DocumentFormat, Timeout, ResponseDocumentFormatter.CreateDevicesResponseDocument, CreateHandlers());
        }

        /// <summary>
        /// Asyncronously execute the Probe Request
        /// </summary>
        public Task<IDevicesResponseDocument> GetAsync(CancellationToken cancellationToken)
        {
            return MTConnectHttpDocumentRequest.SendAsync(_httpClient, CreateRequest, DocumentFormat, Timeout, ResponseDocumentFormatter.CreateDevicesResponseDocument, CreateHandlers(), cancellationToken);
        }

        private HttpRequestMessage CreateRequest()
        {
            return MTConnectHttpDocumentRequest.CreateRequest(CreateUri(), ContentType, DocumentFormat, ContentEncodings);
        }

        private MTConnectHttpDocumentRequest.Handlers CreateHandlers()
        {
            return new MTConnectHttpDocumentRequest.Handlers
            {
                MTConnectError = document => MTConnectError.Raise(this, document, InternalError),
                FormatError = result => FormatError.Raise(this, result, InternalError),
                ConnectionError = exception => ConnectionError.Raise(this, exception, InternalError),
                InternalError = exception => InternalError.Raise(this, exception, InternalError)
            };
        }


        /// <summary>Builds the <c>probe</c> request URI from the client's own <see cref="Authority"/>, <see cref="Device"/>, and <see cref="DocumentFormat"/>.</summary>
        public Uri CreateUri() => CreateUri(Authority, Device, DocumentFormat);

        /// <summary>Convenience overload that passes <c>0</c> for <paramref name="port"/>, taking the port from <paramref name="hostname"/> if present.</summary>
        /// <param name="hostname">Agent base URL or hostname.</param>
        /// <param name="device">Optional device key; null requests the agent-scoped probe.</param>
        /// <param name="documentFormat">Optional document format (<c>documentFormat</c> query parameter).</param>
        public static Uri CreateUri(string hostname, string device = null, string documentFormat = null) => CreateUri(hostname, 0, device, documentFormat);

        /// <summary>
        /// Builds the absolute <c>probe</c> request URI. Combines <paramref name="hostname"/>
        /// (with <paramref name="port"/> appended if positive) and the <paramref name="device"/>
        /// segment, then adds <c>/probe</c> together with the <c>documentFormat</c> query
        /// parameter when supplied.
        /// </summary>
        public static Uri CreateUri(string hostname, int port, string device = null, string documentFormat = null)
        {
            if (!string.IsNullOrEmpty(hostname))
            {
                var url = hostname;

                // Add Port
                url = Url.AddPort(url, port);

                // Remove Probe command from URL
                var cmd = "probe";
                if (url.EndsWith(cmd) && url.Length > cmd.Length)
                    url = url.Substring(0, url.Length - cmd.Length);

                // Check for Trailing Forward Slash
                if (!url.EndsWith("/")) url += "/";
                if (!string.IsNullOrEmpty(device)) url += device + "/";

                // Add Command
                url += cmd;

                // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
                if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

                // Check for http
                if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;


                // Add 'DocumentFormat' parameter
                if (!string.IsNullOrEmpty(documentFormat) && documentFormat != MTConnect.DocumentFormat.XML)
                {
                    url = Url.AddQueryParameter(url, "documentFormat", documentFormat.ToLower());
                }

                return new Uri(url);
            }

            return null;
        }


        private static string GetHostname(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var uri = new Uri(url);
                return uri.Host;
            }

            return null;
        }
    }
}
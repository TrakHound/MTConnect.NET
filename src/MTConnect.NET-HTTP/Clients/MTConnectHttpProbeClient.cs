// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Http;
using System;
using System.Collections.Generic;
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
        private static readonly HttpClient _httpClient;

        static MTConnectHttpProbeClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
        }

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
            catch (Exception ex)
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
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();

                    // Add 'Accept' HTTP Header
                    var contentType = Formatters.ResponseDocumentFormatter.GetContentType(DocumentFormat);
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        request.Headers.Add(HttpHeaders.Accept, contentType);
                    }

                    // Add 'Accept-Encoding' HTTP Header 
                    if (!ContentEncodings.IsNullOrEmpty())
                    {
                        foreach (var contentEncoding in ContentEncodings)
                        {
                            request.Headers.Add(HttpHeaders.AcceptEncoding, contentEncoding.ToString().ToLower());
                        }
                    }

                    // Create Uri and Send Request
#if NET5_0_OR_GREATER
                        using (var response = _httpClient.Send(request))
#else
                    using (var response = _httpClient.SendAsync(request).Result)
#endif
                    {
                        response.EnsureSuccessStatusCode();
                        return HandleResponse(response);
                    }
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                ConnectionError?.Invoke(this, ex);
            }
            catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
            catch (HttpRequestException ex)
            {
                ConnectionError?.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                InternalError?.Invoke(this, ex);
            }

            return null;
        }

        /// <summary>
        /// Asyncronously execute the Probe Request
        /// </summary>
        public async Task<IDevicesResponseDocument> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();

                    // Add 'Accept' HTTP Header
                    var contentType = Formatters.ResponseDocumentFormatter.GetContentType(DocumentFormat);
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        request.Headers.Add(HttpHeaders.Accept, contentType);
                    }

                    // Add 'Accept-Encoding' HTTP Header 
                    if (!ContentEncodings.IsNullOrEmpty())
                    {
                        foreach (var contentEncoding in ContentEncodings)
                        {
                            request.Headers.Add(HttpHeaders.AcceptEncoding, contentEncoding.ToString().ToLower());
                        }
                    }

                    // Create Uri and Send Request
                    using (var response = await _httpClient.SendAsync(request, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();
                        return await HandleResponseAsync(response, cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                ConnectionError?.Invoke(this, ex);
            }
            catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
            catch (HttpRequestException ex)
            {
                ConnectionError?.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                InternalError?.Invoke(this, ex);
            }

            return null;
        }


        public Uri CreateUri() => CreateUri(Authority, Device, DocumentFormat);

        public static Uri CreateUri(string hostname, string device = null, string documentFormat = null) => CreateUri(hostname, 0, device, documentFormat);

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
                //if (url.Contains(Environment.MachineName)) url = url.Replace(Environment.MachineName, "127.0.0.1");

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



        private IDevicesResponseDocument HandleResponse(HttpResponseMessage response)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    ConnectionError?.Invoke(this, new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
                    var documentBytes = response.Content.ReadAsByteArrayAsync().Result;
                    return ReadDocument(response, documentBytes);
                }
            }

            return null;
        }

        private async Task<IDevicesResponseDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    ConnectionError?.Invoke(this, new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
#if NET5_0_OR_GREATER
                    var documentBytes = await response.Content.ReadAsByteArrayAsync(cancel);
#else
                    var documentBytes = await response.Content.ReadAsByteArrayAsync();
#endif

                    return ReadDocument(response, documentBytes);
                }
            }

            return null;
        }

        private IDevicesResponseDocument ReadDocument(HttpResponseMessage response, byte[] documentBytes)
        {
            if (documentBytes != null && documentBytes.Length > 0)
            {
                // Handle Compression Encoding
                var contentEncoding = MTConnectHttpResponse.GetContentHeaderValue(response, HttpHeaders.ContentEncoding);
                var bytes = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, documentBytes);

                // Process MTConnectDevices Document
                var document = Formatters.ResponseDocumentFormatter.CreateDevicesResponseDocument(DocumentFormat.ToString(), bytes).Document;
                if (document != null)
                {
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
                    var errorDocument = Formatters.ResponseDocumentFormatter.CreateErrorResponseDocument(DocumentFormat.ToString(), bytes).Document;
                    if (errorDocument != null)
                    {
                        MTConnectError?.Invoke(this, errorDocument);
                    }
                }
            }

            return null;
        }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Errors;
using MTConnect.Http;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Sample request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
    /// </summary>
    public class MTConnectHttpSampleClient : IMTConnectSampleClient
    {
        private const int DefaultTimeout = 15000;
        private static readonly HttpClient _httpClient;

        static MTConnectHttpSampleClient()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
        }


        /// <summary>
        /// Initializes a new instance of the MTConnectSampleClient class that is used to perform
        /// a Sample request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
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
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        /// <param name="to">The sequence to retrieve the sample data to</param>
        /// <param name="count">The number of sequences to include in the sample data</param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpSampleClient(string authority, string device = null, string path = null, long from = -1, long to = -1, long count = -1, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Authority = authority;
            Device = device;
            Path = path;
            From = from;
            To = to;
            Count = count;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectSampleClient class that is used to perform
        /// a Sample request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
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
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        /// <param name="to">The sequence to retrieve the sample data to</param>
        /// <param name="count">The number of sequences to include in the sample data</param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpSampleClient(string hostname, int port, string device = null, string path = null, long from = -1, long to = -1, long count = -1, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Authority = CreateUri(hostname, port).ToString();
            Device = device;
            Path = path;
            From = from;
            To = to;
            Count = count;
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
        /// Gets or Sets the Document Format to use
        /// </summary>
        public string DocumentFormat { get; set; }

        /// <summary> 
        /// (Optional) The sequence to retrieve the sample data from
        /// </summary>
        public long From { get; set; }

        /// <summary> 
        /// (Optional) The sequence to retrieve the sample data to
        /// </summary>
        public long To { get; set; }

        /// <summary> 
        /// (Optional) The number of sequences to include in the sample data
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// (Optional) The XPath expression specifying the components and/or data items to include
        /// </summary>
        public string Path { get; set; }

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
        public EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }

        /// <summary>
        /// Raised when an Connection Error occurs
        /// </summary>
        public EventHandler<Exception> OnConnectionError { get; set; }

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public EventHandler<Exception> OnInternalError { get; set; }


        /// <summary>
        /// Execute the Samples Request
        /// </summary>
        public IStreamsResponseDocument Get()
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
                OnConnectionError?.Invoke(this, ex);
            }
            catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
            catch (HttpRequestException ex)
            {
                OnConnectionError?.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                OnInternalError?.Invoke(this, ex);
            }

            return null;
        }

        /// <summary>
        /// Asyncronously execute the Samples Request
        /// </summary>
        public async Task<IStreamsResponseDocument> GetAsync(CancellationToken cancel)
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
                    using (var response = await _httpClient.SendAsync(request, cancel))
                    {
                        response.EnsureSuccessStatusCode();
                        return await HandleResponseAsync(response, cancel);
                    }
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                OnConnectionError?.Invoke(this, ex);
            }
            catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
            catch (HttpRequestException ex)
            {
                OnConnectionError?.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                OnInternalError?.Invoke(this, ex);
            }

            return null;
        }


        public Uri CreateUri() => CreateUri(Authority, Device, Path, From, To, Count, DocumentFormat);

        public static Uri CreateUri(string hostname, string device = null, string path = null, long from = 0, long to = 0, long count = 0, string documentFormat = null) => CreateUri(hostname, 0, device, path, from, to, count, documentFormat);

        public static Uri CreateUri(string hostname, int port, string device = null, string path = null, long from = 0, long to = 0, long count = 0, string documentFormat = null)
        {
            if (!string.IsNullOrEmpty(hostname))
            {
                var url = hostname;

                // Add Port
                url = Url.AddPort(url, port);

                var builder = new UriBuilder(url);
                var query = HttpUtility.ParseQueryString(builder.Query);
                if (query.HasKeys())
                {
                    // From
                    var s = query.Get("from");
                    if (from < 0 && s != null && long.TryParse(s, out var l)) from = l;

                    // To
                    s = query.Get("to"); ;
                    if (to < 0 && s != null && long.TryParse(s, out l)) to = l;

                    // Count
                    s = query.Get("count"); ;
                    if (count <= 0 && s != null && long.TryParse(s, out l)) count = l;

                    builder.Query = null;
                    url = builder.Uri.ToString();
                }

                // Remove Sample command from URL
                var cmd = "sample";
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


                // Add 'From' parameter
                if (from > 0) url = Url.AddQueryParameter(url, "from", from);

                // Add 'To' parameter
                if (to > 0) url = Url.AddQueryParameter(url, "to", to);

                // Add 'Count' parameter
                if (count > 0) url = Url.AddQueryParameter(url, "count", count);

                // Add 'Path' parameter
                if (!string.IsNullOrEmpty(path)) url = Url.AddQueryParameter(url, "path", path);

                // Add 'DocumentFormat' parameter
                if (!string.IsNullOrEmpty(documentFormat) && documentFormat != MTConnect.DocumentFormat.XML)
                {
                    url = Url.AddQueryParameter(url, "documentFormat", documentFormat.ToLower());
                }

                return new Uri(url);
            }

            return null;
        }


        private IStreamsResponseDocument HandleResponse(HttpResponseMessage response)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    OnConnectionError?.Invoke(this, new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
                    var xml = response.Content.ReadAsByteArrayAsync().Result;
                    return ReadDocument(response, xml);
                }
            }

            return null;
        }

        private async Task<IStreamsResponseDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    OnConnectionError?.Invoke(this, new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
#if NET5_0_OR_GREATER
                    var documentString = await response.Content.ReadAsByteArrayAsync(cancel);
#else
                    var documentString = await response.Content.ReadAsByteArrayAsync();
#endif

                    return ReadDocument(response, documentString);
                }
            }

            return null;
        }

        private IStreamsResponseDocument ReadDocument(HttpResponseMessage response, byte[] documentBytes)
        {
            if (documentBytes != null && documentBytes.Length > 0)
            {
                // Handle Compression Encoding
                var contentEncoding = MTConnectHttpResponse.GetContentHeaderValue(response, HttpHeaders.ContentEncoding);
                var bytes = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, documentBytes);

                // Process MTConnectDevices Document
                var document = Formatters.ResponseDocumentFormatter.CreateStreamsResponseDocument(DocumentFormat.ToString(), bytes).Document;
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
                        OnMTConnectError?.Invoke(this, errorDocument);
                    }
                }
            }

            return null;
        }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Http;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            _httpClient = new HttpClient();
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
                    var contentType = ResponseDocumentFormatter.GetContentType(DocumentFormat);
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
                RaiseEvent(ConnectionError, ex);
            }
            catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
            catch (HttpRequestException ex)
            {
                RaiseEvent(ConnectionError, ex);
            }
            catch (Exception ex)
            {
                RaiseEvent(InternalError, ex);
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
                    var contentType = ResponseDocumentFormatter.GetContentType(DocumentFormat);
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
                RaiseEvent(ConnectionError, ex);
            }
            catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
            catch (HttpRequestException ex)
            {
                RaiseEvent(ConnectionError, ex);
            }
            catch (Exception ex)
            {
                RaiseEvent(InternalError, ex);
            }

            return null;
        }


        /// <summary>Builds the <c>sample</c> request URI from the client's own <see cref="Authority"/>, <see cref="Device"/>, <see cref="Path"/>, <see cref="From"/>, <see cref="To"/>, <see cref="Count"/>, and <see cref="DocumentFormat"/>.</summary>
        public Uri CreateUri() => CreateUri(Authority, Device, Path, From, To, Count, DocumentFormat);

        /// <summary>Convenience overload that passes <c>0</c> for <paramref name="port"/>, so the port is taken from <paramref name="hostname"/> if it carries one.</summary>
        /// <param name="hostname">Agent base URL or hostname.</param>
        /// <param name="device">Optional device key; null requests an agent-scoped sample.</param>
        /// <param name="path">Optional XPath/JSONPath filter (becomes the <c>path</c> query parameter).</param>
        /// <param name="from">The starting sequence number (<c>from</c> query parameter); <c>0</c> means unset.</param>
        /// <param name="to">The ending sequence number (<c>to</c> query parameter); <c>0</c> means unset.</param>
        /// <param name="count">The maximum number of observations (<c>count</c> query parameter); <c>0</c> means unset.</param>
        /// <param name="documentFormat">Optional document format (<c>documentFormat</c> query parameter).</param>
        public static Uri CreateUri(string hostname, string device = null, string path = null, long from = 0, long to = 0, long count = 0, string documentFormat = null) => CreateUri(hostname, 0, device, path, from, to, count, documentFormat);

        /// <summary>
        /// Builds the absolute <c>sample</c> request URI. Combines <paramref name="hostname"/>
        /// (with <paramref name="port"/> appended if positive) and the <paramref name="device"/>
        /// segment, then adds <c>/sample</c> together with the standard MTConnect query
        /// parameters <c>path</c>, <c>from</c>, <c>to</c>, <c>count</c>, and <c>documentFormat</c>
        /// for the supplied non-default values.
        /// </summary>
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
                    RaiseEvent(ConnectionError, new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
                    var documentStream = response.Content.ReadAsStreamAsync().Result;
                    return ReadDocument(response, documentStream);
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
                    RaiseEvent(ConnectionError, new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
#if NET5_0_OR_GREATER
                    var documentStream = await response.Content.ReadAsStreamAsync(cancel);
#else
                    var documentStream = await response.Content.ReadAsStreamAsync();
#endif

                    return ReadDocument(response, documentStream);
                }
            }

            return null;
        }

        private IStreamsResponseDocument ReadDocument(HttpResponseMessage response, Stream documentStream)
        {
            if (documentStream != null && documentStream.Length > 0)
            {
                // Handle Compression Encoding
                var contentEncoding = MTConnectHttpResponse.GetContentHeaderValue(response, HttpHeaders.ContentEncoding);
                var stream = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, documentStream);
                if (stream != null && stream.Position > 0) stream.Seek(0, SeekOrigin.Begin);

                var formatResult = ResponseDocumentFormatter.CreateStreamsResponseDocument(DocumentFormat, stream);
                if (formatResult.Success)
                {
                    // Process MTConnectDevices Document
                    var document = formatResult.Content;
                    if (document != null)
                    {
                        return document;
                    }
                    else
                    {
                        // Process MTConnectError Document (if MTConnectStreams fails)
                        var errorFormatResult = ResponseDocumentFormatter.CreateErrorResponseDocument(DocumentFormat, stream);
                        if (errorFormatResult.Success)
                        {
                            var errorDocument = errorFormatResult.Content;
                            if (errorDocument != null) RaiseEvent(MTConnectError, errorDocument);
                        }
                        else
                        {
                            // Raise Format Error
                            RaiseEvent(FormatError, errorFormatResult);
                        }
                    }
                }
                else
                {
                    // Raise Format Error
                    RaiseEvent(FormatError, formatResult);
                }
            }

            return null;
        }

        // Iterate the invocation list so one throwing subscriber cannot short-circuit
        // the multicast and starve later subscribers. Each fault is forwarded through
        // InternalError; if InternalError itself faults, swallow that secondary fault
        // so the remaining subscribers in the invocation list still receive the event.
        private void RaiseEvent<T>(EventHandler<T> handler, T arg)
        {
            if (handler == null) return;

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((EventHandler<T>)subscriber).Invoke(this, arg);
                }
                catch (Exception ex)
                {
                    RouteSubscriberFault(ex);
                }
            }
        }

        // Forwards a subscriber fault to InternalError and swallows any secondary
        // fault raised by InternalError itself so the originating event's fan-out
        // can keep running.
        private void RouteSubscriberFault(Exception ex)
        {
            try
            {
                InternalError?.Invoke(this, ex);
            }
            catch
            {
                // A faulting InternalError handler must not break the event fan-out.
            }
        }
    }
}
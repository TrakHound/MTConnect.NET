// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Errors;
using MTConnect.Streams;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MTConnect.Clients.Rest
{
    /// <summary>
    /// Client that is used to perform a Sample request from an MTConnect Agent using the MTConnect REST Api protocol
    /// </summary>
    public class MTConnectSampleClient : IMTConnectSampleClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };


        /// <summary>
        /// Initializes a new instance of the MTConnectSampleClient class that is used to perform
        /// a Sample request from an MTConnect Agent using the MTConnect REST Api protocol
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
        public MTConnectSampleClient(string authority, string device = null, string path = null, long from = -1, long to = -1, long count = -1, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Authority = authority;
            Device = device;
            Path = path;
            From = from;
            To = to;
            Count = count;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }


        /// <summary>
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </summary>
        public string Authority { get; set; }

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
                        request.Headers.Add("Accept", contentType);
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
                        request.Headers.Add("Accept", contentType);
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

        private Uri CreateUri()
        {
            var baseUrl = Authority;
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (query.HasKeys())
            {
                // From
                var s = query.Get("from");
                if (From < 0 && s != null && long.TryParse(s, out var l)) From = l;

                // To
                s = query.Get("to"); ;
                if (To < 0 && s != null && long.TryParse(s, out l)) To = l;

                // Count
                s = query.Get("count"); ;
                if (Count <= 0 && s != null && long.TryParse(s, out l)) Count = l;

                builder.Query = null;
                baseUrl = builder.Uri.ToString();
            }

            // Remove Sample command from URL
            var cmd = "sample";
            if (baseUrl.EndsWith(cmd) && baseUrl.Length > cmd.Length)
                baseUrl = baseUrl.Substring(0, baseUrl.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            if (!string.IsNullOrEmpty(Device)) baseUrl += Device + "/";

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (baseUrl.Contains("localhost")) baseUrl = baseUrl.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://")) baseUrl = "http://" + baseUrl;

            // Create Uri
            var uri = new Uri(baseUrl);
            uri = new Uri(uri, cmd);
            builder = new UriBuilder(uri);

            // Add Query Parameters
            query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'From' parameter
            if (From > 0) query["from"] = From.ToString();

            // Add 'To' parameter
            if (To > 0) query["to"] = To.ToString();

            // Add 'Count' parameter
            if (Count > 0) query["count"] = Count.ToString();

            // Add 'Path' parameter
            if (!string.IsNullOrEmpty(Path)) query["path"] = Path;

            // Add 'DocumentFormat' parameter
            if (!string.IsNullOrEmpty(DocumentFormat) && DocumentFormat != MTConnect.DocumentFormat.XML)
            {
                query["documentFormat"] = DocumentFormat.ToString().ToLower();
            }

            builder.Query = query.ToString();

            return builder.Uri;
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
                    var xml = response.Content.ReadAsStringAsync().Result;
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
                    var documentString = await response.Content.ReadAsStringAsync(cancel);
#else
                    var documentString = await response.Content.ReadAsStringAsync();
#endif

                    return ReadDocument(response, documentString);
                }
            }

            return null;
        }

        private IStreamsResponseDocument ReadDocument(HttpResponseMessage response, string documentString)
        {
            if (!string.IsNullOrEmpty(documentString))
            {
                // Process MTConnectDevices Document
                var document = Formatters.ResponseDocumentFormatter.CreateStreamsResponseDocument(DocumentFormat.ToString(), documentString).Document;
                if (document != null)
                {
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
                    var errorDocument = Formatters.ResponseDocumentFormatter.CreateErrorResponseDocument(DocumentFormat.ToString(), documentString).Document;
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

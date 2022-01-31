// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MTConnect.Clients.Rest
{
    public class MTConnectSampleClient : IMTConnectSampleClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        public MTConnectSampleClient()
        {
            Init();
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        public MTConnectSampleClient(string baseUrl)
        {
            Init();
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public MTConnectSampleClient(string baseUrl, string deviceName)
        {
            Init();
            BaseUrl = baseUrl;
            DeviceName = deviceName;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        public MTConnectSampleClient(string baseUrl, long from)
        {
            Init();
            BaseUrl = baseUrl;
            From = from;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        /// <param name="to">The sequence to retrieve the sample data to</param>
        public MTConnectSampleClient(string baseUrl, long from, long to)
        {
            Init();
            BaseUrl = baseUrl;
            From = from;
            To = to;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        public MTConnectSampleClient(string baseUrl, string deviceName, long from)
        {
            Init();
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            From = from;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public MTConnectSampleClient(string baseUrl, string deviceName, string path)
        {
            Init();
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            Path = path;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        /// <param name="to">The sequence to retrieve the sample data to</param>
        public MTConnectSampleClient(string baseUrl, string deviceName, long from, long to)
        {
            Init();
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            From = from;
            To = to;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        /// <param name="to">The sequence to retrieve the sample data to</param>
        public MTConnectSampleClient(string baseUrl, string deviceName, string path, long from, long to)
        {
            Init();
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            Path = path;
            From = from;
            To = to;
        }

        private void Init()
        {
            From = -1;
            To = -1;
            Count = 0;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// The base URL for the Sample Request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// (Optional) The name of the requested device
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or Sets the Document Format to use
        /// </summary>
        public DocumentFormat DocumentFormat { get; set; }

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
        /// User settable object sent with request and returned in Document on response
        /// </summary>
        public object UserObject { get; set; }

        /// <summary>
        /// Gets of Sets the connection timeout for the request
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public EventHandler<Errors.ErrorDocument> OnMTConnectError { get; set; }

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
        public Streams.StreamsDocument Get()
        {
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();
                    request.Headers.Add("Accept", "application/xml");

                    // Create Uri and Send Request
                    using (var response = _httpClient.SendAsync(request).Result)
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
        public async Task<Streams.StreamsDocument> GetAsync(CancellationToken cancel)
        {
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();
                    request.Headers.Add("Accept", "application/xml");

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
            var baseUrl = BaseUrl;
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
            if (!string.IsNullOrEmpty(DeviceName)) baseUrl += DeviceName + "/";

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

            builder.Query = query.ToString();

            return builder.Uri;
        }

        private Streams.StreamsDocument HandleResponse(HttpResponseMessage response)
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

        private async Task<Streams.StreamsDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
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

        private Streams.StreamsDocument ReadDocument(HttpResponseMessage response, string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                // Process MTConnectStreams Document
                var doc = Streams.StreamsDocument.FromXml(xml);
                if (doc != null)
                {
                    return doc;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectStreams fails)
                    var errorDoc = Errors.ErrorDocument.Create(xml);
                    if (errorDoc != null)
                    {
                        errorDoc.UserObject = UserObject;
                        errorDoc.Url = response.RequestMessage.RequestUri.ToString();
                        OnMTConnectError?.Invoke(this, errorDoc);
                    }
                }
            }

            return null;
        }
    }
}

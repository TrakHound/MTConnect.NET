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
    public class MTConnectCurrentClient : IMTConnectCurrentClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        public MTConnectCurrentClient()
        {
            At = -1;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        public MTConnectCurrentClient(string baseUrl)
        {
            BaseUrl = baseUrl;
            At = -1;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        public MTConnectCurrentClient(string baseUrl, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DocumentFormat = documentFormat;
            At = -1;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = -1;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = -1;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        public MTConnectCurrentClient(string baseUrl, long atSequence)
        {
            BaseUrl = baseUrl;
            At = atSequence;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        public MTConnectCurrentClient(string baseUrl, long atSequence, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            At = atSequence;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, long atSequence)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = atSequence;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, long atSequence, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = atSequence;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, string path)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = -1;
            Path = path;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, string path, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = -1;
            Path = path;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, string path, long atSequence)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = atSequence;
            Path = path;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public MTConnectCurrentClient(string baseUrl, string deviceName, string path, long atSequence, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = atSequence;
            Path = path;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }


        /// <summary>
        /// The base URL for the Current Request
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
        /// (Optional) The sequence number to retrieve the current data for
        /// </summary>
        public long At { get; set; }

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
        /// Execute the Current Request
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

                    switch (DocumentFormat)
                    {
                        case DocumentFormat.XML: request.Headers.Add("Accept", "application/xml"); break;
                        case DocumentFormat.JSON: request.Headers.Add("Accept", "application/json"); break;
                    }

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
        /// Asyncronously execute the Current Request
        /// </summary>
        public async Task<Streams.StreamsDocument> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();

                    switch (DocumentFormat)
                    {
                        case DocumentFormat.XML: request.Headers.Add("Accept", "application/xml"); break;
                        case DocumentFormat.JSON: request.Headers.Add("Accept", "application/json"); break;
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
            return CreateUri(BaseUrl, DeviceName, At, Path, DocumentFormat);
        }

        public static Uri CreateUri(string url, string deviceName, long at = 0, string path = null, DocumentFormat documentFormat = DocumentFormat.XML)
        {
            var baseUrl = url;

            // Remove Current command from URL
            var cmd = "current";
            if (baseUrl.EndsWith(cmd) && baseUrl.Length > cmd.Length)
                baseUrl = baseUrl.Substring(0, baseUrl.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            if (!string.IsNullOrEmpty(deviceName)) baseUrl += deviceName + "/";

            // Check for http
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://")) baseUrl = "http://" + baseUrl;

            // Create UriBuilder
            var builder = new UriBuilder(Url.Combine(baseUrl, cmd));

            // Add Query Parameters
            var query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'At' parameter
            if (at > 0) query["at"] = at.ToString();

            // Add 'Path' parameter
            if (!string.IsNullOrEmpty(path)) query["path"] = path;

            // Add 'DocumentFormat' parameter
            if (documentFormat != DocumentFormat.XML) query["documentFormat"] = documentFormat.ToString().ToLower();

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
                    var documentString = response.Content.ReadAsStringAsync().Result;
                    return ReadDocument(response, documentString);
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

        private Streams.StreamsDocument ReadDocument(HttpResponseMessage response, string documentString)
        {
            if (!string.IsNullOrEmpty(documentString))
            {
                // Process MTConnectStreams Document
                Streams.StreamsDocument document = null;

                switch (DocumentFormat)
                {
                    case DocumentFormat.XML: document = Streams.StreamsDocument.FromXml(documentString); break;
                    case DocumentFormat.JSON: document = Streams.StreamsDocument.FromJson(documentString); break;
                }

                if (document != null)
                {
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectStreams fails)
                    Errors.ErrorDocument errorDocument = null;

                    switch (DocumentFormat)
                    {
                        case DocumentFormat.XML: errorDocument = Errors.ErrorDocument.FromXml(documentString); break;
                        case DocumentFormat.JSON: errorDocument = Errors.ErrorDocument.FromJson(documentString); break;
                    }

                    if (errorDocument != null)
                    {
                        errorDocument.UserObject = UserObject;
                        errorDocument.Url = response.RequestMessage.RequestUri.ToString();
                        OnMTConnectError?.Invoke(this, errorDocument);
                    }
                }
            }

            return null;
        }
    }
}

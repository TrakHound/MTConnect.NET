// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MTConnect.Clients
{
    public class Current
    {
        private const int DEFAULT_TIMEOUT = 5000;

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        public Current()
        {
            At = -1;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        public Current(string baseUrl)
        {
            BaseUrl = baseUrl;
            At = -1;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public Current(string baseUrl, string deviceName)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = -1;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        public Current(string baseUrl, long atSequence)
        {
            BaseUrl = baseUrl;
            At = atSequence;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        public Current(string baseUrl, string deviceName, long atSequence)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = atSequence;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public Current(string baseUrl, string deviceName, string path)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = -1;
            Path = path;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="atSequence">The sequence number to retrieve the current data for</param>
        /// <param name="path">The XPath expression specifying the components and/or data items to include</param>
        public Current(string baseUrl, string deviceName, string path, long atSequence)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            At = atSequence;
            Path = path;
            Timeout = DEFAULT_TIMEOUT;
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
        public event MTConnectErrorHandler Error;

        /// <summary>
        /// Raised when an Connection Error occurs
        /// </summary>
        public event ConnectionErrorHandler ConnectionError;

        /// <summary>
        /// Raised when an MTConnectStreams Document is received successfully
        /// </summary>
        public event MTConnectStreamsHandler Successful;


        /// <summary>
        /// Execute the Current Request
        /// </summary>
        public async Task<MTConnectStreams.Document> Execute(CancellationToken cancellationToken)
        {
            // Create HTTP Client and Request Data
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(Timeout);
            client.DefaultRequestHeaders.Add("Accept", "application/xml");

            try
            {
                var response = await client.GetAsync(CreateUri(), cancellationToken);
                response.EnsureSuccessStatusCode();
                return await ProcessResponse(response, cancellationToken);
            }
            catch (HttpRequestException e)
            {
                ConnectionError?.Invoke(e);
            }

            return null;
        }

        private Uri CreateUri()
        {
            // Check for Trailing Forward Slash
            var baseUrl = BaseUrl;
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            // Create Uri
            var uri = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(DeviceName)) uri = new Uri(uri, DeviceName + "/current");
            else uri = new Uri(uri, "current");
            var builder = new UriBuilder(uri);

            // Add Query Parameters
            var query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'At' parameter
            if (At > -1) query["at"] = At.ToString();

            // Add 'Path' parameter
            if (!string.IsNullOrEmpty(Path)) query["path"] = Path;

            builder.Query = query.ToString();

            return builder.Uri;
        }

        private async Task<MTConnectStreams.Document> ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    ConnectionError?.Invoke(new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
                    //string xml = await response.Content.ReadAsStringAsync();
                    var xml = await Task.Run(response.Content.ReadAsStringAsync, cancellationToken);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        // Process MTConnectStreams Document
                        var doc = MTConnectStreams.Document.Create(xml);
                        if (doc != null)
                        {
                            doc.UserObject = UserObject;
                            doc.Url = response.RequestMessage.RequestUri.ToString();
                            Successful?.Invoke(doc);
                            return doc;
                        }
                        else
                        {
                            // Process MTConnectError Document (if MTConnectDevices fails)
                            var errorDoc = MTConnectError.Document.Create(xml);
                            if (errorDoc != null)
                            {
                                errorDoc.UserObject = UserObject;
                                errorDoc.Url = response.RequestMessage.RequestUri.ToString();
                                Error?.Invoke(errorDoc);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}

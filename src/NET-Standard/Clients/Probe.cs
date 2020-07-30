// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public class Probe
    {
        private const int DEFAULT_TIMEOUT = 5000;

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        public Probe() { }

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Probe Request</param>
        public Probe(string baseUrl)
        {
            BaseUrl = baseUrl;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Probe Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public Probe(string baseUrl, string deviceName)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// The base URL for the Probe Request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// (Optional) The name of the requested device
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// (Optional) User settable object sent with request and returned in Document on response
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
        /// Raised when an MTConnectDevices Document is received successfully
        /// </summary>
        public event MTConnectDevicesHandler Successful;


        /// <summary>
        /// Execute the Probe Request
        /// </summary>
        public async Task<MTConnectDevices.Document> Execute()
        {
            // Create HTTP Client and Request Data
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(Timeout);
            client.DefaultRequestHeaders.Add("Accept", "application/xml");

            try
            {
                var response = await client.GetAsync(CreateUri());
                response.EnsureSuccessStatusCode();
                return await ProcessResponse(response);
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
            if (!string.IsNullOrEmpty(DeviceName)) uri = new Uri(uri, DeviceName + "/probe");
            else uri = new Uri(uri, "probe");
            return uri;
        }

        private async Task<MTConnectDevices.Document> ProcessResponse(HttpResponseMessage response)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    ConnectionError?.Invoke(new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
                    string xml = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(xml))
                    {
                        // Process MTConnectDevices Document
                        var doc = MTConnectDevices.Document.Create(xml);
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

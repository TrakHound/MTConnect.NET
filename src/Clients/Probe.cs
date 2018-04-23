// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using RestSharp;
using System;

namespace MTConnect.Clients
{
    public class Probe
    {
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
            Timeout = 2000;
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
            Timeout = 2000;
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
        /// Raised when an MTConnectDevices Document is received successfully
        /// </summary>
        public event MTConnectDevicesHandler Successful;


        /// <summary>
        /// Execute the Probe Request Synchronously
        /// </summary>
        public MTConnectDevices.Document Execute()
        {
            // Create HTTP Client and Request Data
            var client = new RestClient(CreateUri());
            client.Timeout = Timeout;
            client.ReadWriteTimeout = Timeout;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return ProcessResponse(response);
        }

        /// <summary>
        /// Execute the Probe Request Asynchronously
        /// </summary>
        public void ExecuteAsync()
        {
            // Create HTTP Client and Request Data
            var client = new RestClient(CreateUri());
            var request = new RestRequest(Method.GET);
            client.ExecuteAsync(request, AsyncCallback);
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

        private MTConnectDevices.Document ProcessResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                if (response.ErrorException != null) ConnectionError?.Invoke(response.ErrorException);
            }
            else if (!string.IsNullOrEmpty(response.Content))
            {
                string xml = response.Content;

                // Process MTConnectStreams Document
                var doc = MTConnectDevices.Document.Create(xml);
                if (doc != null)
                {
                    doc.UserObject = UserObject;
                    doc.Url = response.ResponseUri.ToString();
                    return doc;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
                    var errorDoc = MTConnectError.Document.Create(xml);
                    if (errorDoc != null)
                    {
                        errorDoc.UserObject = UserObject;
                        errorDoc.Url = response.ResponseUri.ToString();
                        Error?.Invoke(errorDoc);
                    }
                }
            }

            return null;
        }

        private void AsyncCallback(IRestResponse response)
        {
            var doc = ProcessResponse(response);
            if (doc != null) Successful?.Invoke(doc);
        }
    }
}

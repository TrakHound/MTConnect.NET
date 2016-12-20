// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
            // Create Uri
            var uri = new Uri(BaseUrl);
            uri = new Uri(uri, "probe");
            if (DeviceName != null) uri = new Uri(uri, DeviceName);

            // Create HTTP Client and Request Data
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return ProcessResponse(response);
        }

        /// <summary>
        /// Execute the Probe Request Asynchronously
        /// </summary>
        public void ExecuteAsync()
        {
            // Create Uri
            var uri = new Uri(BaseUrl);
            if (DeviceName != null) uri = new Uri(uri, DeviceName);
            uri = new Uri(uri, "probe");

            // Create HTTP Client and Request Data
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);
            client.ExecuteAsync(request, AsyncCallback);
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
                    return doc;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
                    var errorDoc = MTConnectError.Document.Create(xml);
                    if (errorDoc != null)
                    {
                        errorDoc.UserObject = UserObject;
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

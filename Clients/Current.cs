// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using RestSharp;
using System;

namespace MTConnect.Clients
{
    public class Current
    {
        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        public Current()
        {
            At = -1;
        }

        /// <summary>
        /// Create a new Current Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Current Request</param>
        public Current(string baseUrl)
        {
            BaseUrl = baseUrl;
            At = -1;
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
        /// Execute the Current Request Synchronously
        /// </summary>
        public MTConnectStreams.Document Execute()
        {
            // Create HTTP Client and Request Data
            var client = new RestClient(CreateUri());
            IRestResponse response = client.Execute(CreateRequest());
            return ProcessResponse(response);
        }

        /// <summary>
        /// Execute the Current Request Asynchronously
        /// </summary>
        public void ExecuteAsync()
        {
            // Create HTTP Client and Request Data
            var client = new RestClient(CreateUri());
            client.ExecuteAsync(CreateRequest(), AsyncCallback);
        }

        private Uri CreateUri()
        {
            // Create Uri
            var uri = new Uri(BaseUrl);
            if (DeviceName != null) uri = new Uri(uri, DeviceName);
            uri = new Uri(uri, "current");        
            return uri;
        }

        private RestRequest CreateRequest()
        {
            var request = new RestRequest(Method.GET);

            // Add 'At' parameter
            if (At > -1) request.AddQueryParameter("at", At.ToString());

            // Add 'Path' parameter
            if (!string.IsNullOrEmpty(Path)) request.AddQueryParameter("path", Path);

            return request;
        }

        private MTConnectStreams.Document ProcessResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                if (response.ErrorException != null) ConnectionError?.Invoke(response.ErrorException);
            }
            else if (!string.IsNullOrEmpty(response.Content))
            {
                string xml = response.Content;

                // Process MTConnectStreams Document
                var doc = MTConnectStreams.Document.Create(xml);
                if (doc != null)
                {
                    return doc;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
                    var errorDoc = MTConnectError.Document.Create(xml);
                    if (errorDoc != null) Error?.Invoke(errorDoc);
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

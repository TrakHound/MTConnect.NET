// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using RestSharp;
using System;

namespace MTConnect.Client
{
    public class Sample
    {
        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        public Sample()
        {
            Init();
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        public Sample(string baseUrl)
        {
            Init();
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public Sample(string baseUrl, string deviceName)
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
        public Sample(string baseUrl, long from)
        {
            Init();
            BaseUrl = baseUrl;
            From = from;
        }

        /// <summary>
        /// Create a new Sample Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Sample Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        /// <param name="from">The sequence to retrieve the sample data from</param>
        public Sample(string baseUrl, string deviceName, long from)
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
        public Sample(string baseUrl, string deviceName, string path)
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
        public Sample(string baseUrl, string deviceName, long from, long to)
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
        /// <param name="from">The sequence to retrieve the sample data from</param>
        /// <param name="to">The sequence to retrieve the sample data to</param>
        public Sample(string baseUrl, string deviceName, string path, long from, long to)
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
            From = 0;
            To = 0;
            Count = 0;
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
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public event MTConnectErrorHandler Error;

        /// <summary>
        /// Raised when an MTConnectStreams Document is received successfully
        /// </summary>
        public event MTConnectStreamsHandler Successful;

        /// <summary>
        /// Execute the Sample Request Synchronously
        /// </summary>
        public MTConnectStreams.Document Execute()
        {
            // Create HTTP Client and Request Data
            var client = new RestClient(CreateUri());
            IRestResponse response = client.Execute(CreateRequest());
            return ProcessResponse(response);
        }

        /// <summary>
        /// Execute the Sample Request Asynchronously
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
            uri = new Uri(uri, "sample");
            if (DeviceName != null) uri = new Uri(uri, DeviceName);
            return uri;
        }

        private RestRequest CreateRequest()
        {
            var request = new RestRequest(Method.GET);

            // Add 'From' parameter
            if (From > 0) request.AddQueryParameter("from", From.ToString());

            // Add 'To' parameter
            if (To > 0) request.AddQueryParameter("to", To.ToString());

            // Add 'Count' parameter
            if (Count > 0) request.AddQueryParameter("count", Count.ToString());

            // Add 'Path' parameter
            if (!string.IsNullOrEmpty(Path)) request.AddQueryParameter("path", Path);

            return request;
        }

        private MTConnectStreams.Document ProcessResponse(IRestResponse response)
        {
            if (response != null && !string.IsNullOrEmpty(response.Content))
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

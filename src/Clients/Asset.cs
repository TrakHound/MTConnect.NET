// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using RestSharp;
using System;

namespace MTConnect.Clients
{
    public class Asset
    {
        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        public Asset()
        {
            Init();
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        public Asset(string baseUrl)
        {
            Init();
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        /// <param name="assetId">The Id of the requested Asset</param>
        public Asset(string baseUrl, string assetId)
        {
            Init();
            BaseUrl = baseUrl;
            AssetId = assetId;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        /// <param name="type">The Type of Assets to retrieve</param>
        /// <param name="count">The maximum Count of Assets to retrieve</param>
        public Asset(string baseUrl, string type, long count)
        {
            Init();
            BaseUrl = baseUrl;
            Type = type;
            Count = count;
        }

        private void Init()
        {
            Count = 0;
        }

        /// <summary>
        /// The base URL for the Asset Request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// (Optional) The Id of the requested Asset
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// (Optional) The Type of Assets to retrieve
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// (Optional) The maximum Count of Assets to retrieve
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// User settable object sent with request and returned in Document on response
        /// </summary>
        public object UserObject { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public event MTConnectErrorHandler Error;

        /// <summary>
        /// Raised when an MTConnectAssets Document is received successfully
        /// </summary>
        public event MTConnectAssetsHandler Successful;


        /// <summary>
        /// Execute the Asset Request Synchronously
        /// </summary>
        public MTConnectAssets.Document Execute()
        {
            // Create HTTP Client and Request Data
            var client = new RestClient(CreateUri());
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return ProcessResponse(response);
        }

        /// <summary>
        /// Execute the Asset Request Asynchronously
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

            var uri = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(AssetId)) uri = new Uri(uri, "assets/" + AssetId);
            else uri = new Uri(uri, "assets");
            return uri;
        }

        private RestRequest CreateRequest()
        {
            var request = new RestRequest(Method.GET);

            // Add 'Type' parameter
            if (!string.IsNullOrEmpty(Type)) request.AddQueryParameter("type", Type);

            // Add 'Count' parameter
            if (Count > 0) request.AddQueryParameter("count", Count.ToString());

            return request;
        }

        private MTConnectAssets.Document ProcessResponse(IRestResponse response)
        {
            if (response != null && !string.IsNullOrEmpty(response.Content))
            {
                string xml = response.Content;

                // Process MTConnectStreams Document
                var doc = MTConnectAssets.Document.Create(xml);
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

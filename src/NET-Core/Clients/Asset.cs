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
    public class Asset
    {
        private const int DEFAULT_TIMEOUT = 5000;

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
            Timeout = DEFAULT_TIMEOUT;
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
        /// Raised when an MTConnectAssets Document is received successfully
        /// </summary>
        public event MTConnectAssetsHandler Successful;


        /// <summary>
        /// Execute the Asset Request
        /// </summary>
        public async Task<MTConnectAssets.Document> Execute()
        {
            return await Execute(new CancellationToken());
        }

        /// <summary>
        /// Execute the Asset Request
        /// </summary>
        public async Task<MTConnectAssets.Document> Execute(CancellationToken cancellationToken)
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

            var uri = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(AssetId)) uri = new Uri(uri, "assets/" + AssetId);
            else uri = new Uri(uri, "assets");
            var builder = new UriBuilder(uri);

            // Add Query Parameters
            var query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'Type' parameter
            if (!string.IsNullOrEmpty(Type)) query["type"] = Type;

            // Add 'Count' parameter
            if (Count > -1) query["count"] = Count.ToString();

            builder.Query = query.ToString();

            return builder.Uri;
        }

        private async Task<MTConnectAssets.Document> ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response != null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    ConnectionError?.Invoke(new Exception(response.ReasonPhrase));
                }
                else if (response.Content != null)
                {
                    var xml = await Task.Run(response.Content.ReadAsStringAsync, cancellationToken);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        // Process MTConnectAssets Document
                        var doc = MTConnectAssets.Document.Create(xml);
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

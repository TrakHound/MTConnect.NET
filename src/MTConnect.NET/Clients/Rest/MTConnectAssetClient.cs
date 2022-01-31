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
    public class MTConnectAssetClient : IMTConnectAssetClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };


        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        public MTConnectAssetClient()
        {
            Init();
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        public MTConnectAssetClient(string baseUrl)
        {
            Init();
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        public MTConnectAssetClient(string baseUrl, DocumentFormat documentFormat)
        {
            Init();
            BaseUrl = baseUrl;
            DocumentFormat = documentFormat;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        /// <param name="assetId">The Id of the requested Asset</param>
        public MTConnectAssetClient(string baseUrl, string assetId)
        {
            Init();
            BaseUrl = baseUrl;
            AssetId = assetId;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        /// <param name="assetId">The Id of the requested Asset</param>
        public MTConnectAssetClient(string baseUrl, string assetId, DocumentFormat documentFormat)
        {
            Init();
            BaseUrl = baseUrl;
            AssetId = assetId;
            DocumentFormat = documentFormat;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        /// <param name="type">The Type of Assets to retrieve</param>
        /// <param name="count">The maximum Count of Assets to retrieve</param>
        public MTConnectAssetClient(string baseUrl, string type, long count)
        {
            Init();
            BaseUrl = baseUrl;
            Type = type;
            Count = count;
        }

        /// <summary>
        /// Create a new Asset Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Asset Request</param>
        /// <param name="type">The Type of Assets to retrieve</param>
        /// <param name="count">The maximum Count of Assets to retrieve</param>
        public MTConnectAssetClient(string baseUrl, string type, long count, DocumentFormat documentFormat)
        {
            Init();
            BaseUrl = baseUrl;
            Type = type;
            Count = count;
            DocumentFormat = documentFormat;
        }


        private void Init()
        {
            Count = 0;
            Timeout = DefaultTimeout;
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
        /// Gets or Sets the Document Format to use
        /// </summary>
        public DocumentFormat DocumentFormat { get; set; }

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
        /// Execute the Asset Request
        /// </summary>
        public Assets.AssetsDocument Get()
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
                    //using (var response = _httpClient.Send(request))
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
        /// Asynchronously execute the Asset Request
        /// </summary>
        public async Task<Assets.AssetsDocument> GetAsync(CancellationToken cancellationToken)
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
            var baseUrl = BaseUrl;

            // Remove Assets command from URL
            var cmd = "assets";
            if (baseUrl.EndsWith(cmd) && baseUrl.Length > cmd.Length)
                baseUrl = baseUrl.Substring(0, baseUrl.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            // Check for http
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://")) baseUrl = "http://" + baseUrl;

            // Create Uri
            var url = baseUrl;
            if (!string.IsNullOrEmpty(AssetId)) url = Url.Combine(url, $"{cmd}/{AssetId}");
            else url = Url.Combine(url, cmd);

            var builder = new UriBuilder(url);

            // Add Query Parameters
            var query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'Type' parameter
            if (!string.IsNullOrEmpty(Type)) query["type"] = Type;

            // Add 'Count' parameter
            if (Count > 0) query["count"] = Count.ToString();

            // Add 'DocumentFormat' parameter
            if (DocumentFormat != DocumentFormat.XML) query["documentFormat"] = DocumentFormat.ToString().ToLower();

            builder.Query = query.ToString();

            return builder.Uri;
        }

        private Assets.AssetsDocument HandleResponse(HttpResponseMessage response)
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

        private async Task<Assets.AssetsDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
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


        private Assets.AssetsDocument ReadDocument(HttpResponseMessage response, string documentString)
        {
            if (!string.IsNullOrEmpty(documentString))
            {
                // Process MTConnectAssets Document
                Assets.AssetsDocument document = null;

                switch (DocumentFormat)
                {
                    case DocumentFormat.XML: document = Assets.AssetsDocument.FromXml(documentString); break;
                    case DocumentFormat.JSON: document = Assets.AssetsDocument.FromJson(documentString); break;
                }

                if (document != null)
                {
                    document.UserObject = UserObject;
                    document.Url = response.RequestMessage.RequestUri.ToString();
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

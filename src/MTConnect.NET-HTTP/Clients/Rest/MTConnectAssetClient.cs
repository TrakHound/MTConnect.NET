// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Errors;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MTConnect.Clients.Rest
{
    /// <summary>
    /// Client that is used to perform a Assets request from an MTConnect Agent using the MTConnect REST Api protocol
    /// </summary>
    public class MTConnectAssetClient : IMTConnectAssetClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };


        /// <summary>
        /// Initializes a new instance of the MTConnectAssetClient class that is used to perform
        /// an Assets request from an MTConnect Agent using the MTConnect REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectAssetClient(string baseUrl, string assetId, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Init();
            Authority = baseUrl;
            AssetId = assetId;
            DocumentFormat = documentFormat;
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectAssetClient class that is used to perform
        /// an Assets request from an MTConnect Agent using the MTConnect REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectAssetClient(string authority, string type = null, long count = -1, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Init();
            Authority = authority;
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
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </summary>
        public string Authority { get; set; }

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
        public string DocumentFormat { get; set; }

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
        public EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }

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
        public IAssetsResponseDocument Get()
        {
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();

                    // Add 'Accept' HTTP Header
                    var contentType = Formatters.ResponseDocumentFormatter.GetContentType(DocumentFormat);
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        request.Headers.Add("Accept", contentType);
                    }

                    // Create Uri and Send Request
#if NET5_0_OR_GREATER
                    using (var response = _httpClient.Send(request))
#else
                    using (var response = _httpClient.SendAsync(request).Result)
#endif
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
        public async Task<IAssetsResponseDocument> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Create Http Request
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = CreateUri();

                    // Add 'Accept' HTTP Header
                    var contentType = Formatters.ResponseDocumentFormatter.GetContentType(DocumentFormat);
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        request.Headers.Add("Accept", contentType);
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
            var baseUrl = Authority;

            // Remove Assets command from URL
            var cmd = "assets";
            if (baseUrl.EndsWith(cmd) && baseUrl.Length > cmd.Length)
                baseUrl = baseUrl.Substring(0, baseUrl.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (baseUrl.Contains("localhost")) baseUrl = baseUrl.Replace("localhost", "127.0.0.1");

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
            if (!string.IsNullOrEmpty(DocumentFormat) && DocumentFormat != MTConnect.DocumentFormat.XML)
            {
                query["documentFormat"] = DocumentFormat.ToString().ToLower();
            }

            builder.Query = query.ToString();

            return builder.Uri;
        }

        private IAssetsResponseDocument HandleResponse(HttpResponseMessage response)
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

        private async Task<IAssetsResponseDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
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


        private IAssetsResponseDocument ReadDocument(HttpResponseMessage response, string documentString)
        {
            if (!string.IsNullOrEmpty(documentString))
            {
                // Process MTConnectAssets Document
                var document = Formatters.ResponseDocumentFormatter.CreateAssetsResponseDocument(DocumentFormat.ToString(), documentString).Document;
                if (document != null)
                {
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectAssets fails)
                    var errorDocument = Formatters.ResponseDocumentFormatter.CreateErrorResponseDocument(DocumentFormat.ToString(), documentString).Document;
                    if (errorDocument != null)
                    {
                        OnMTConnectError?.Invoke(this, errorDocument);
                    }
                }
            }

            return null;
        }
    }
}

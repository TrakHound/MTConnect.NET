// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Errors;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Assets request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
    /// </summary>
    public class MTConnectHttpAssetClient : IMTConnectAssetClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };


        /// <summary>
        /// Initializes a new instance of the MTConnectAssetClient class that is used to perform
        /// an Assets request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="assetId">The Id of the requested Asset</param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpAssetClient(string authority, string assetId, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Init();
            Authority = authority;
            AssetId = assetId;
            DocumentFormat = documentFormat;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectAssetClient class that is used to perform
        /// an Assets request from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="type">The Type of Assets to retrieve</param>
        /// <param name="device">The Device to retrieve Assets for</param>
        /// <param name="count">Specifies the maximum number of MTConnectAssets Response Documents returned in an MTConnectAssets Response Document</param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpAssetClient(string authority, long count = -1, string type = null, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Init();
            Authority = authority;
            Device = device;
            Type = type;
            Count = count;
            DocumentFormat = documentFormat;
            ContentType = MimeTypes.Get(documentFormat);
        }


        private void Init()
        {
            Count = 0;
            Timeout = DefaultTimeout;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
        }

        /// <summary>
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </summary>
        public string Authority { get; }

        /// <summary>
        /// (Optional) The Id of the requested Asset
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// (Optional) The Device to retrieve Assets for
        /// </summary>
        public string Device { get; set; }

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
        /// Gets of Sets the connection timeout for the request
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        public IEnumerable<HttpContentEncoding> ContentEncodings { get; set; }

        /// <summary>
        /// Gets or Sets the Content-type (or MIME-type) to pass to the Accept HTTP Header
        /// </summary>
        public string ContentType { get; set; }

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
                        request.Headers.Add(HttpHeaders.Accept, contentType);
                    }

                    // Add 'Accept-Encoding' HTTP Header 
                    if (!ContentEncodings.IsNullOrEmpty())
                    {
                        foreach (var contentEncoding in ContentEncodings)
                        {
                            request.Headers.Add(HttpHeaders.AcceptEncoding, contentEncoding.ToString().ToLower());
                        }
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
                        request.Headers.Add(HttpHeaders.Accept, contentType);
                    }

                    // Add 'Accept-Encoding' HTTP Header 
                    if (!ContentEncodings.IsNullOrEmpty())
                    {
                        foreach (var contentEncoding in ContentEncodings)
                        {
                            request.Headers.Add(HttpHeaders.AcceptEncoding, contentEncoding.ToString().ToLower());
                        }
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
            if (!string.IsNullOrEmpty(AssetId)) return CreateUriSingle();
            else return CreateUriMultiple();
        }

        private Uri CreateUriMultiple()
        {
            var url = Authority;

            // Remove Assets command from URL
            var cmd = "assets";
            if (url.EndsWith(cmd) && url.Length > cmd.Length)
                url = url.Substring(0, url.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!url.EndsWith("/")) url += "/";
            else if (!string.IsNullOrEmpty(Device)) url += Device + "/";

            // Add Command
            url += cmd;

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;


            // Add 'Type' parameter
            if (!string.IsNullOrEmpty(Type)) url = Url.AddQueryParameter(url, "type", Type);

            // Add 'Count' parameter
            if (Count > 0) url = Url.AddQueryParameter(url, "count", Count);

            // Add 'DocumentFormat' parameter
            if (!string.IsNullOrEmpty(DocumentFormat) && DocumentFormat != MTConnect.DocumentFormat.XML)
            {
                url = Url.AddQueryParameter(url, "documentFormat", DocumentFormat.ToLower());
            }

            return new Uri(url);
        }

        private Uri CreateUriSingle()
        {
            var url = Authority;

            // Remove Assets command from URL
            var cmd = "asset";
            if (url.EndsWith(cmd) && url.Length > cmd.Length)
                url = url.Substring(0, url.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!url.EndsWith("/")) url += "/";

            // Add Command
            url += cmd + "/";

            // Add AssetId
            url += AssetId;

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            // Add 'DocumentFormat' parameter
            if (!string.IsNullOrEmpty(DocumentFormat) && DocumentFormat != MTConnect.DocumentFormat.XML)
            {
                url = Url.AddQueryParameter(url, "documentFormat", DocumentFormat.ToLower());
            }

            return new Uri(url);
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
                    var documentString = response.Content.ReadAsByteArrayAsync().Result;
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
                    var documentString = await response.Content.ReadAsByteArrayAsync(cancel);
#else
                    var documentString = await response.Content.ReadAsByteArrayAsync();
#endif

                    return ReadDocument(response, documentString);
                }
            }

            return null;
        }


        private IAssetsResponseDocument ReadDocument(HttpResponseMessage response, byte[] documentBytes)
        {
            if (documentBytes != null && documentBytes.Length > 0)
            {
                // Handle Compression Encoding
                var contentEncoding = MTConnectHttpResponse.GetContentHeaderValue(response, HttpHeaders.ContentEncoding);
                var bytes = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, documentBytes);

                // Process MTConnectAssets Document
                var document = Formatters.ResponseDocumentFormatter.CreateAssetsResponseDocument(DocumentFormat.ToString(), bytes).Document;
                if (document != null)
                {
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectAssets fails)
                    var errorDocument = Formatters.ResponseDocumentFormatter.CreateErrorResponseDocument(DocumentFormat.ToString(), bytes).Document;
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
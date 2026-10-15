// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.IO;
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
        private const int DefaultTimeout = 15000;
        private static readonly HttpClient _defaultHttpClient;
        private readonly HttpClient _httpClient;

        static MTConnectHttpAssetClient()
        {
            _defaultHttpClient = new HttpClient();
            _defaultHttpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
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
        /// <param name="assetId">The Id of the requested Asset</param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpAssetClient(string authority, string assetId, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = _defaultHttpClient;
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
            _httpClient = _defaultHttpClient;
            Init();
            Authority = authority;
            Device = device;
            Type = type;
            Count = count;
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
        /// <param name="assetId">The Id of the requested Asset</param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpAssetClient(HttpClient httpClient, string authority, string assetId, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = httpClient != null ? httpClient : _defaultHttpClient;
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
        public MTConnectHttpAssetClient(HttpClient httpClient, string authority, long count = -1, string type = null, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = httpClient != null ? httpClient : _defaultHttpClient;
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
        public event EventHandler<IErrorResponseDocument> MTConnectError;

        /// <summary>
        /// Raised when a Document Formatting Error is received
        /// </summary>
        public event EventHandler<IFormatReadResult> FormatError;

        /// <summary>
        /// Raised when an Connection Error occurs
        /// </summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public event EventHandler<Exception> InternalError;


        /// <summary>
        /// Execute the Asset Request
        /// </summary>
        public IAssetsResponseDocument Get()
        {
            // Honours ContentType like GetAsync; runs on the thread pool so a blocked caller cannot deadlock
            return MTConnectHttpDocumentRequest.Send(_httpClient, CreateRequest, DocumentFormat, Timeout, ResponseDocumentFormatter.CreateAssetsResponseDocument, CreateHandlers());
        }

        /// <summary>
        /// Asynchronously execute the Asset Request
        /// </summary>
        public Task<IAssetsResponseDocument> GetAsync(CancellationToken cancellationToken)
        {
            return MTConnectHttpDocumentRequest.SendAsync(_httpClient, CreateRequest, DocumentFormat, Timeout, ResponseDocumentFormatter.CreateAssetsResponseDocument, CreateHandlers(), cancellationToken);
        }

        private HttpRequestMessage CreateRequest()
        {
            return MTConnectHttpDocumentRequest.CreateRequest(CreateUri(), ContentType, DocumentFormat, ContentEncodings);
        }

        private MTConnectHttpDocumentRequest.Handlers CreateHandlers()
        {
            return new MTConnectHttpDocumentRequest.Handlers
            {
                MTConnectError = document => MTConnectError.Raise(this, document, InternalError),
                FormatError = result => FormatError.Raise(this, result, InternalError),
                ConnectionError = exception => ConnectionError.Raise(this, exception, InternalError),
                InternalError = exception => InternalError.Raise(this, exception, InternalError)
            };
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
    }
}
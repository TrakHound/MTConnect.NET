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
    public class MTConnectProbeClient : IMTConnectProbeClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };


        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        public MTConnectProbeClient() { }

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Probe Request</param>
        public MTConnectProbeClient(string baseUrl)
        {
            BaseUrl = baseUrl;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Probe Request</param>
        public MTConnectProbeClient(string baseUrl, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Probe Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public MTConnectProbeClient(string baseUrl, string deviceName)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Create a new Probe Request Client
        /// </summary>
        /// <param name="baseUrl">The base URL for the Probe Request</param>
        /// <param name="deviceName">The name of the requested device</param>
        public MTConnectProbeClient(string baseUrl, string deviceName, DocumentFormat documentFormat)
        {
            BaseUrl = baseUrl;
            DeviceName = deviceName;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
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
        /// Gets or Sets the Document Format to use
        /// </summary>
        public DocumentFormat DocumentFormat { get; set; }

        /// <summary>
        /// (Optional) User settable object sent with request and returned in Document on response
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
        /// Execute the Probe Request
        /// </summary>
        public Devices.DevicesDocument Get()
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
        /// Asyncronously execute the Probe Request
        /// </summary>
        public async Task<Devices.DevicesDocument> GetAsync(CancellationToken cancellationToken)
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
            return CreateUri(BaseUrl, DeviceName, DocumentFormat);
        }

        public static Uri CreateUri(string url, string deviceName, DocumentFormat documentFormat)
        {
            var baseUrl = url;

            // Remove Probe command from URL
            var cmd = "probe";
            if (baseUrl.EndsWith(cmd) && baseUrl.Length > cmd.Length)
                baseUrl = baseUrl.Substring(0, baseUrl.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            if (!string.IsNullOrEmpty(deviceName)) baseUrl += deviceName + "/";

            // Check for http
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://")) baseUrl = "http://" + baseUrl;

            // Create UriBuilder
            var builder = new UriBuilder(Url.Combine(baseUrl, cmd));

            // Add Query Parameters
            var query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'DocumentFormat' parameter
            if (documentFormat != DocumentFormat.XML) query["documentFormat"] = documentFormat.ToString().ToLower();

            builder.Query = query.ToString();

            // Create and Return Url
            return builder.Uri;
        }

        private Devices.DevicesDocument HandleResponse(HttpResponseMessage response)
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

        private async Task<Devices.DevicesDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
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

        private Devices.DevicesDocument ReadDocument(HttpResponseMessage response, string documentString)
        {
            if (!string.IsNullOrEmpty(documentString))
            {
                // Process MTConnectDevices Document
                Devices.DevicesDocument document = null;

                switch (DocumentFormat)
                {
                    case DocumentFormat.XML: document = Devices.DevicesDocument.FromXml(documentString); break;
                    case DocumentFormat.JSON: document = Devices.DevicesDocument.FromJson(documentString); break;
                }

                if (document != null)
                {
                    document.UserObject = UserObject;
                    document.Url = response.RequestMessage.RequestUri.ToString();
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
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

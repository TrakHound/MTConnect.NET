// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Errors;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MTConnect.Clients.Rest
{
    /// <summary>
    /// Client that is used to perform a Probe request from an MTConnect Agent using the MTConnect REST Api protocol
    /// </summary>
    public class MTConnectProbeClient : IMTConnectProbeClient
    {
        private const int DefaultTimeout = 5000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };


        /// <summary>
        /// Initializes a new instance of the MTConnectProbeClient class that is used to perform
        /// a Probe request from an MTConnect Agent using the MTConnect REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectProbeClient(string authority, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Authority = authority;
            Device = device;
            DocumentFormat = documentFormat;
            Timeout = DefaultTimeout;
        }


        /// <summary>
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </summary>
        public string Authority { get; }

        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        public string Device { get; }

        /// <summary>
        /// Gets or Sets the Document Format to return
        /// </summary>
        public string DocumentFormat { get; set; }

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
        /// Execute the Probe Request
        /// </summary>
        public IDevicesResponseDocument Get()
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
        /// Asyncronously execute the Probe Request
        /// </summary>
        public async Task<IDevicesResponseDocument> GetAsync(CancellationToken cancellationToken)
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
            return CreateUri(Authority, Device, DocumentFormat);
        }

        public static Uri CreateUri(string url, string deviceName, string documentFormat)
        {
            var baseUrl = url;

            // Remove Probe command from URL
            var cmd = "probe";
            if (baseUrl.EndsWith(cmd) && baseUrl.Length > cmd.Length)
                baseUrl = baseUrl.Substring(0, baseUrl.Length - cmd.Length);

            // Check for Trailing Forward Slash
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            if (!string.IsNullOrEmpty(deviceName)) baseUrl += deviceName + "/";

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (baseUrl.Contains("localhost")) baseUrl = baseUrl.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://")) baseUrl = "http://" + baseUrl;

            // Create UriBuilder
            var builder = new UriBuilder(Url.Combine(baseUrl, cmd));

            // Add Query Parameters
            var query = HttpUtility.ParseQueryString(builder.Query);

            // Add 'DocumentFormat' parameter
            if (!string.IsNullOrEmpty(documentFormat) && documentFormat != MTConnect.DocumentFormat.XML)
            {
                query["documentFormat"] = documentFormat.ToString().ToLower();
            }

            builder.Query = query.ToString();

            // Create and Return Url
            return builder.Uri;
        }

        private IDevicesResponseDocument HandleResponse(HttpResponseMessage response)
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

        private async Task<IDevicesResponseDocument> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancel)
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

        private IDevicesResponseDocument ReadDocument(HttpResponseMessage response, string documentString)
        {
            if (!string.IsNullOrEmpty(documentString))
            {
                // Process MTConnectDevices Document
                var document = Formatters.ResponseDocumentFormatter.CreateDevicesResponseDocument(DocumentFormat.ToString(), documentString).Document;
                if (document != null)
                {
                    return document;
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
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

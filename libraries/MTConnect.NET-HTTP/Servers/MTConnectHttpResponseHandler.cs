﻿// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MTConnect.Servers.Http
{
    internal abstract class MTConnectHttpResponseHandler : IHttpModule
    {
        protected readonly IMTConnectAgentBroker _mtconnectAgent;
        protected readonly IHttpServerConfiguration _serverConfiguration;
        protected readonly ILogger _logger;


        /// <summary>
        /// Event Handler for when an error occurs with a MTConnectHttpResponse is written to the HTTP Client
        /// </summary>
        public event EventHandler<MTConnectHttpResponse> ResponseSent;

        /// <summary>
        /// Event Handler for when a client makes a request to the server
        /// </summary>
        public event EventHandler<IHttpRequest> ClientConnected;

        /// <summary>
        /// Event Handler for when a client completes a request or disconnects from the server
        /// </summary>
        public event EventHandler<string> ClientDisconnected;

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListenerRequest
        /// </summary>
        public event EventHandler<Exception> ClientException;

        public Func<MTConnectFormatOptionsArgs, List<KeyValuePair<string, string>>> CreateFormatOptionsFunction { get; set; }


        public MTConnectHttpResponseHandler(
            IHttpServerConfiguration serverConfiguration, 
            IMTConnectAgentBroker mtconnectAgent,
            ILogger logger)
        {
            _mtconnectAgent = mtconnectAgent;
            _serverConfiguration = serverConfiguration;
            _logger = logger;
        }


        public async Task<bool> HandleAsync(IHttpContext context, CancellationToken cancellationToken)
        {
            try
            {
                ClientConnected?.Invoke(this, context.Request);

                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(context.Request, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                var mtconnectResponse = await OnRequestReceived(context, cancellationToken);
                mtconnectResponse.WriteDuration = await WriteResponse(mtconnectResponse, context.Response, acceptEncodings);

                ResponseSent?.Invoke(this, mtconnectResponse);

                ClientDisconnected?.Invoke(this, context.Request.RemoteEndPoint?.ToString());

                return true;
            }
            catch (HttpListenerException ex)
            {
                // Ignore Disposed Object Exception (happens when the listener is stopped)
                if (ex.ErrorCode != 995)
                {
                    if (ClientException != null) ClientException.Invoke(this, ex);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                if (ClientException != null) ClientException.Invoke(this, ex);
            }

            return false;
        }

        protected async virtual Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken) { return new MTConnectHttpResponse(); }



        #region "Responses"

        /// <summary>
        /// Write a MTConnectHttpResponse to the HttpListenerResponse Output Stream
        /// </summary>
        protected async Task<double> WriteResponse(MTConnectHttpResponse mtconnectResponse, IHttpResponse httpResponse, IEnumerable<string> acceptEncodings = null)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            if (httpResponse != null)
            {
                try
                {
                    httpResponse.ContentType = mtconnectResponse.ContentType;
                    httpResponse.StatusCode = (Ceen.HttpStatusCode)mtconnectResponse.StatusCode;

                    await WriteToStream(mtconnectResponse.Content, httpResponse, acceptEncodings);
                }
                catch { }
            }

            stpw.Stop();
            return stpw.GetElapsedMilliseconds();
        }

        /// <summary>
        /// Write a string to the HttpListenerResponse Output Stream
        /// </summary>
        protected async Task WriteResponse(string content, IHttpResponse httpResponse, Ceen.HttpStatusCode statusCode, string contentType = MimeTypes.XML, IEnumerable<string> acceptEncodings = null)
        {
            if (httpResponse != null)
            {
                try
                {
                    httpResponse.ContentType = contentType;
                    httpResponse.StatusCode = statusCode;

                    var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    await WriteToStream(contentStream, httpResponse, acceptEncodings);
                }
                catch { }
            }
        }

        protected async Task WriteToStream(Stream inputStream, IHttpResponse httpResponse, IEnumerable<string> acceptEncodings = null)
        {
            if (httpResponse != null && inputStream != null && inputStream.Length > 0)
            {
                if (inputStream.Position > 0) inputStream.Seek(0, SeekOrigin.Begin);
                MemoryStream outputStream;

                try
                {
                    // Gzip
                    if (!_serverConfiguration.ResponseCompression.IsNullOrEmpty() &&
                        _serverConfiguration.ResponseCompression.Contains(HttpResponseCompression.Gzip) &&
                        !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains("gzip"))
                    {
                        httpResponse.AddHeader("Content-Encoding", "gzip");

                        outputStream = new MemoryStream();
                        using (var zip = new GZipStream(outputStream, CompressionMode.Compress, true))
                        {
                            inputStream.CopyTo(zip);
                        }
                        outputStream.Seek(0, SeekOrigin.Begin);
                        await httpResponse.WriteAllAsync(outputStream);
                    }

#if NET5_0_OR_GREATER
                    else if (!_serverConfiguration.ResponseCompression.IsNullOrEmpty() &&
                        _serverConfiguration.ResponseCompression.Contains(HttpResponseCompression.Br) &&
                        !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains("br"))
                    {
                        httpResponse.AddHeader("Content-Encoding", "br");

                        outputStream = new MemoryStream();
                        using (var zip = new BrotliStream(inputStream, CompressionMode.Compress, true))
                        {
                            inputStream.CopyTo(zip);
                        }
                        outputStream.Seek(0, SeekOrigin.Begin);
                        await httpResponse.WriteAllAsync(outputStream);
                    }
#endif

                    else if (!_serverConfiguration.ResponseCompression.IsNullOrEmpty() &&
                        _serverConfiguration.ResponseCompression.Contains(HttpResponseCompression.Deflate) &&
                        !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains("deflate"))
                    {
                        httpResponse.AddHeader("Content-Encoding", "deflate");

                        outputStream = new MemoryStream();
                        using (var zip = new DeflateStream(inputStream, CompressionMode.Compress, true))
                        {
                            inputStream.CopyTo(zip);
                        }
                        outputStream.Seek(0, SeekOrigin.Begin);
                        await httpResponse.WriteAllAsync(outputStream);
                    }

                    else
                    {
                        await httpResponse.WriteAllAsync(inputStream);
                    }
                }
                catch { }
            }
        }

        protected static async Task WriteToResponseStream(Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (responseStream != null)
            {
                await args.Message.CopyToAsync(responseStream);
            }
        }

        protected async Task WriteFromStream(MTConnectHttpServerStream sampleStream, Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (sampleStream != null && responseStream != null)
            {
                try
                {
                    await WriteToResponseStream(responseStream, args);
                }
                catch (Exception ex)
                {
                    if (ClientDisconnected != null) ClientDisconnected.Invoke(this, sampleStream.Id);
                    sampleStream.Stop();
                }
            }
        }

        protected async Task WriteFromStream(MTConnectHttpServerStream sampleStream, IHttpResponse response, Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (sampleStream != null && responseStream != null)
            {
                try
                {
                    await WriteToResponseStream(responseStream, args);

                    await response.FlushHeadersAsync();
                }
                catch (Exception ex)
                {
                    if (ClientDisconnected != null) ClientDisconnected.Invoke(this, sampleStream.Id);
                    sampleStream.Stop();
                }
            }
        }


        protected IEnumerable<string> ProcessAcceptEncodings(IEnumerable<string> acceptEncodings)
        {
            if (!acceptEncodings.IsNullOrEmpty() && !_serverConfiguration.ResponseCompression.IsNullOrEmpty())
            {
                var output = new List<string>();

                if (_serverConfiguration.ResponseCompression.Contains(HttpResponseCompression.Gzip) &&
                    !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains(HttpContentEncodings.Gzip))
                {
                    output.Add(HttpContentEncodings.Gzip);
                }

#if NET5_0_OR_GREATER
                else if (_serverConfiguration.ResponseCompression.Contains(HttpResponseCompression.Br) &&
                    !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains(HttpContentEncodings.Brotli))
                {
                    output.Add(HttpContentEncodings.Brotli);
                }
#endif

                else if (_serverConfiguration.ResponseCompression.Contains(HttpResponseCompression.Deflate) &&
                    !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains(HttpContentEncodings.Deflate))
                {
                    output.Add(HttpContentEncodings.Deflate);
                }

                return output;
            }

            return null;
        }

        protected static void SetupStreamResponse(ref IHttpResponse response, string boundary)
        {
            // Set HTTP Status Code
            response.StatusCode = (Ceen.HttpStatusCode)200;
            response.KeepAlive = true;

            // Set HTTP Response Headers
            response.Headers.Add("Server", "MTConnectAgent");
            response.Headers.Add("Expires", "-1");
            response.Headers.Add("Connection", "close");
            response.Headers.Add("Cache-Control", "no-cache, private, max-age=0");
            response.Headers.Add(HttpHeaders.ContentType, $"multipart/x-mixed-replace;boundary={boundary}");
        }

        #endregion


        protected static string[] GetUriSegments(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var l = new List<string>();

                var segments = uri.Split('/');
                if (uri != null && !segments.IsNullOrEmpty())
                {
                    foreach (var segment in segments)
                    {
                        var x = segment.Trim('/');
                        if (!string.IsNullOrEmpty(x)) l.Add(x);
                    }
                }

                return l.ToArray();
            }

            return null;
        }

        protected static string GetDeviceKey(string url, string requestType)
        {
            var segments = GetUriSegments(url);
            if (!segments.IsNullOrEmpty())
            {
                var max = segments.Length - 1;

                // Work backwards from given index
                for (var i = max; i >= 0; i--)
                {
                    if (segments[i] == requestType)
                    {
                        if (i > 0)
                        {
                            return segments[i - 1].Trim('/');
                        }
                    }
                    else if (requestType == MTConnectRequestType.Probe.ToLower())
                    {
                        return segments[i].Trim('/');
                    }
                }
            }

            return null;
        }

        protected static IEnumerable<string> GetRequestHeaderValues(IHttpRequestInternal request, string name)
        {
            if (request != null && request.Headers != null && !string.IsNullOrEmpty(name))
            {
                request.Headers.TryGetValue(name, out var values);
                if (values != null)
                {
                    // Trim Values (StringSplitOptions.TrimEntities is not compatible with older .NET versions)
                    var x = values.Split(',');
                    var y = new string[x.Length];
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (x[i] != null) y[i] = x[i].Trim();
                    }
                    return y;
                }
            }

            return null;
        }


        protected List<KeyValuePair<string, string>> CreateFormatOptions(string requestType, string documentFormat, Version mtconnectVersion, int validationLevel = 0)
        {
            List<KeyValuePair<string, string>> x = null;

            if (CreateFormatOptionsFunction != null)
            {
                var formatOptions = new MTConnectFormatOptionsArgs();
                formatOptions.RequestType = requestType;
                formatOptions.DocumentFormat = documentFormat;
                formatOptions.MTConnectVersion = mtconnectVersion;
                formatOptions.ValidationLevel = validationLevel;

                x = CreateFormatOptionsFunction(formatOptions);
            }

            return !x.IsNullOrEmpty() ? x : new List<KeyValuePair<string, string>>();
        }
    }
}

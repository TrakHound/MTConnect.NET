// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    internal abstract class MTConnectHttpResponseHandler : IHttpModule
    {
        protected readonly IMTConnectAgentBroker _mtconnectAgent;
        protected readonly IHttpServerConfiguration _serverConfiguration;


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


        public MTConnectHttpResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent)
        {
            _mtconnectAgent = mtconnectAgent;
            _serverConfiguration = serverConfiguration;
        }


        public async Task<bool> HandleAsync(IHttpContext context, CancellationToken cancellationToken)
        {
            try
            {
                ClientConnected.Raise(this, context.Request, ClientException);

                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(context.Request, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                var mtconnectResponse = await OnRequestReceived(context, cancellationToken);
                try
                {
                    mtconnectResponse.WriteDuration = await WriteResponse(mtconnectResponse, context.Response, acceptEncodings, cancellationToken);
                }
                finally
                {
                    // The body (e.g. an open static file) is not needed once it has been written
                    mtconnectResponse.Content?.Dispose();
                }

                ResponseSent.Raise(this, mtconnectResponse, ClientException);

                ClientDisconnected.Raise(this, context.Request.RemoteEndPoint?.ToString(), ClientException);

                return true;
            }
            catch (HttpListenerException ex)
            {
                // Ignore Disposed Object Exception (happens when the listener is stopped)
                if (ex.ErrorCode != 995)
                {
                    ClientException.Raise(this, ex, null);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                ClientException.Raise(this, ex, null);
            }

            return false;
        }

        protected async virtual Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new MTConnectHttpResponse();
        }



        #region "Responses"

        /// <summary>
        /// Write a MTConnectHttpResponse to the HttpListenerResponse Output Stream
        /// </summary>
        protected async Task<double> WriteResponse(MTConnectHttpResponse mtconnectResponse, IHttpResponse httpResponse, IEnumerable<string> acceptEncodings = null, CancellationToken cancellationToken = default)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            if (httpResponse != null)
            {
                try
                {
                    httpResponse.ContentType = mtconnectResponse.ContentType;
                    httpResponse.StatusCode = (Ceen.HttpStatusCode)mtconnectResponse.StatusCode;

                    await WriteToStream(mtconnectResponse.Content, httpResponse, acceptEncodings, cancellationToken);
                }
                catch { }
            }

            stpw.Stop();
            return stpw.GetElapsedMilliseconds();
        }

        /// <summary>
        /// Write a string to the HttpListenerResponse Output Stream
        /// </summary>
        protected async Task WriteResponse(string content, IHttpResponse httpResponse, Ceen.HttpStatusCode statusCode, string contentType = MimeTypes.XML, IEnumerable<string> acceptEncodings = null, CancellationToken cancellationToken = default)
        {
            if (httpResponse != null)
            {
                try
                {
                    httpResponse.ContentType = contentType;
                    httpResponse.StatusCode = statusCode;

                    var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    await WriteToStream(contentStream, httpResponse, acceptEncodings, cancellationToken);
                }
                catch { }
            }
        }

        protected async Task WriteToStream(Stream inputStream, IHttpResponse httpResponse, IEnumerable<string> acceptEncodings = null, CancellationToken cancellationToken = default)
        {
            if (httpResponse != null && inputStream != null && inputStream.Length > 0)
            {
                if (inputStream.Position > 0) inputStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    var contentEncoding = HttpContentEncoder.Select(_serverConfiguration, acceptEncodings);
                    if (contentEncoding != null)
                    {
                        httpResponse.AddHeader("Content-Encoding", contentEncoding);

                        using (var encoded = HttpContentEncoder.Encode(inputStream, contentEncoding))
                        {
                            await httpResponse.WriteAllAsync(encoded, cancellationToken);
                        }
                    }
                    else
                    {
                        await httpResponse.WriteAllAsync(inputStream, cancellationToken);
                    }
                }
                catch { }
            }
        }

        protected static async Task WriteToResponseStream(Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (responseStream != null && args.Message != null)
            {
                using (var message = args.Message)
                {
                    await message.CopyToAsync(responseStream);
                }

                // Push each chunk to the client as it is produced
                await responseStream.FlushAsync();
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
                catch (Exception)
                {
                    ClientDisconnected.Raise(this, sampleStream.Id, ClientException);
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
                catch (Exception)
                {
                    ClientDisconnected.Raise(this, sampleStream.Id, ClientException);
                    sampleStream.Stop();
                }
            }
        }


        protected IEnumerable<string> ProcessAcceptEncodings(IEnumerable<string> acceptEncodings)
        {
            if (!acceptEncodings.IsNullOrEmpty() && !_serverConfiguration.ResponseCompression.IsNullOrEmpty())
            {
                var output = new List<string>();

                var contentEncoding = HttpContentEncoder.Select(_serverConfiguration, acceptEncodings);
                if (contentEncoding != null) output.Add(contentEncoding);

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


        #region "Request Parameters"

        /// <summary>
        /// The response format negotiated for a request: MTConnect version, document format,
        /// validation level, and the formatter options built from them.
        /// </summary>
        protected class RequestFormat
        {
            public Version Version { get; set; }
            public string DocumentFormat { get; set; }
            public int ValidationLevel { get; set; }
            public List<KeyValuePair<string, string>> FormatOptions { get; set; }
        }

        /// <summary>
        /// Reads the version, documentFormat, validationLevel, indentOutput and outputComments parameters
        /// (shared by every document request) and builds the formatter options.
        /// </summary>
        protected RequestFormat GetRequestFormat(IHttpRequestInternal httpRequest, string requestType)
        {
            // Read MTConnectVersion from Query string, then the configured default, then the Agent version
            Version.TryParse(httpRequest.QueryString["version"], out var version);
            if (version == null) Version.TryParse(_serverConfiguration.DefaultVersion, out version);
            if (version == null) version = _mtconnectAgent.MTConnectVersion;

            var documentFormat = GetDocumentFormat(httpRequest);

            // Read ValidationLevel from Query string (number or name)
            var validationLevel = (int)_serverConfiguration.OutputValidationLevel;
            var validationLevelString = httpRequest.QueryString["validationLevel"];
            if (!string.IsNullOrEmpty(validationLevelString))
            {
                if (int.TryParse(validationLevelString, NumberStyles.Integer, CultureInfo.InvariantCulture, out var level)) validationLevel = level;
                else if (Enum.TryParse<OutputValidationLevel>(validationLevelString, true, out var namedLevel)) validationLevel = (int)namedLevel;
            }

            // Set Format Options
            var formatOptions = CreateFormatOptions(requestType, documentFormat, version, validationLevel);
            formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString(CultureInfo.InvariantCulture)));

            // Read IndentOutput from Query string
            var indentOutputString = httpRequest.QueryString["indentOutput"];
            if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
            else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _serverConfiguration.IndentOutput.ToString()));

            // Read OutputComments from Query string
            var outputCommentsString = httpRequest.QueryString["outputComments"];
            if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
            else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _serverConfiguration.OutputComments.ToString()));

            return new RequestFormat
            {
                Version = version,
                DocumentFormat = documentFormat,
                ValidationLevel = validationLevel,
                FormatOptions = formatOptions
            };
        }

        /// <summary>
        /// Resolves the document format from the documentFormat query parameter, then the Accept header,
        /// then the configured default.
        /// </summary>
        protected string GetDocumentFormat(IHttpRequestInternal httpRequest)
        {
            var documentFormat = httpRequest.QueryString["documentFormat"];
            if (string.IsNullOrEmpty(documentFormat))
            {
                httpRequest.Headers.TryGetValue("Accept", out var acceptHeader);
                documentFormat = MatchAcceptHeader(_serverConfiguration.Accept, acceptHeader);
            }
            if (string.IsNullOrEmpty(documentFormat)) documentFormat = _serverConfiguration.DocumentFormat;

            return documentFormat;
        }

        /// <summary>
        /// Maps an Accept header to a configured document format. The whole header is tried first (for
        /// configurations keyed on an exact header), then each media range in order with its parameters
        /// (such as q=0.9) removed, so "application/json, */*;q=0.8" selects the JSON format.
        /// </summary>
        internal static string MatchAcceptHeader(IDictionary<string, string> accept, string acceptHeader)
        {
            if (accept.IsNullOrEmpty() || string.IsNullOrEmpty(acceptHeader)) return null;

            if (accept.TryGetValue(acceptHeader, out var exact)) return exact;

            foreach (var range in acceptHeader.Split(','))
            {
                var mediaType = range.Split(';')[0].Trim();
                if (mediaType.Length == 0) continue;

                foreach (var entry in accept)
                {
                    if (string.Equals(entry.Key, mediaType, StringComparison.OrdinalIgnoreCase)) return entry.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads an optional integer query parameter. Returns <c>false</c> when the parameter is present but
        /// is not an integer, so the caller can reject the request instead of silently using a default.
        /// </summary>
        protected static bool TryGetQueryInteger(IHttpRequestInternal httpRequest, string name, out long? value)
        {
            value = null;

            var s = httpRequest.QueryString[name];
            if (string.IsNullOrEmpty(s)) return true;

            if (long.TryParse(s.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var x))
            {
                value = x;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Builds a 400 INVALID_REQUEST MTConnectError response in the negotiated format.
        /// </summary>
        protected MTConnectHttpResponse CreateInvalidRequestResponse(string message, RequestFormat format)
        {
            var errorDocument = _mtconnectAgent.GetErrorResponseDocument(Errors.ErrorCode.INVALID_REQUEST, message, format.Version);
            return new MTConnectHttpResponse(errorDocument, 400, format.DocumentFormat, 0, format.FormatOptions);
        }

        #endregion


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

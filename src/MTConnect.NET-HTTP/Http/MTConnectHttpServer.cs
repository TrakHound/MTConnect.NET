// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests
    /// </summary>
    public class MTConnectHttpServer
    {
        private const string DefaultServer = "127.0.0.1";
        private const int DefaultPort = 5000;
        private const string EmptyServer = "0.0.0.0";

        private static string DefaultPrefix = "http://" + DefaultServer + ":" + DefaultPort + "/";


        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly HttpAgentConfiguration _configuration;
        private readonly List<string> _prefixes = new List<string>();
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, string> _devicesSchemas = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _streamsSchemas = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _assetsSchemas = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _commonSchemas = new Dictionary<string, string>();

        private CancellationTokenSource _stop;
        private CancellationTokenSource _stopped;


        /// <summary>
        /// Event Handler for when the HttpListener is started
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public EventHandler<string> ListenerStarted { get; set; }

        /// <summary>
        /// Event Handler for when the HttpListener is stopped
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public EventHandler<string> ListenerStopped { get; set; }

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListener
        /// </summary>
        public EventHandler<Exception> ListenerException { get; set; }

        /// <summary>
        /// Event Handler for when a client makes a request to the server
        /// </summary>
        public EventHandler<HttpListenerRequest> ClientConnected { get; set; }

        /// <summary>
        /// Event Handler for when a client completes a request or disconnects from the server
        /// </summary>
        public EventHandler<string> ClientDisconnected { get; set; }

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListenerRequest
        /// </summary>
        public EventHandler<Exception> ClientException { get; set; }

        /// <summary>
        /// Event Handler for when an error occurs with a MTConnectHttpResponse is written to the HTTP Client
        /// </summary>
        public EventHandler<MTConnectHttpResponse> ResponseSent { get; set; }


        public MTConnectHttpServer(
            HttpAgentConfiguration configuration,
            IMTConnectAgent mtconnectAgent,
            IEnumerable<string> prefixes = null,
            int port = 0
            )
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = configuration;

            LoadPrefixes(configuration, prefixes, port);
        }


        private void LoadPrefixes(HttpAgentConfiguration configuration, IEnumerable<string> prefixes = null, int port = 0)
        {
            if (!prefixes.IsNullOrEmpty())
            {
                _prefixes.AddRange(prefixes);
            }
            else if (configuration != null)
            {
                var serverIp = DefaultServer;
                var serverPort = DefaultPort;

                // Configuration Server IP
                if (!string.IsNullOrEmpty(configuration.ServerIp))
                {
                    if (configuration.ServerIp != EmptyServer)
                    {
                        serverIp = configuration.ServerIp;
                    }
                }

                // Set Port (if not overridden in method, read from Configuration)
                if (port > 0)
                {
                    serverPort = port;
                }
                else if (configuration.Port > 0)
                {
                    serverPort = configuration.Port;
                }

                // Construct Prefix URL
                _prefixes.Add("http://" + serverIp + ":" + serverPort + "/");

            }
            else _prefixes.Add(DefaultPrefix);
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(() => Worker(_stop.Token));
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
        }


        /// <summary>
        /// Method run when an Observation is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual async Task<bool> OnObservationInput(string deviceKey, string dataItemKey, string input)
        {
            return false;
        }

        /// <summary>
        /// Method run when an Asset is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual async Task<bool> OnAssetInput(string assetId, string deviceKey, string assetType, byte[] requestBody)
        {
            return false;
        }


        private async Task Worker(CancellationToken cancellationToken)
        {
            do
            {
                HttpListener listener = null;
                bool errorOccurred = false;

                try
                {
                    // (Access Denied - Exception)
                    // Must grant permissions to use URL (for each Prefix) in Windows using the command below
                    // CMD: netsh http add urlacl url = "http://localhost/" user = everyone

                    // (Service Unavailable - HTTP Status)
                    // Multiple urls are configured using netsh that point to the same place

                    listener = new HttpListener();

                    // Add Prefixes
                    foreach (var prefix in _prefixes)
                    {
                        listener.Prefixes.Add(prefix);
                    }

                    // Start Listener
                    listener.Start();

                    // Raise Events to notify when a prefix is being listened on
                    if (ListenerStarted != null)
                    {
                        foreach (var prefix in _prefixes) ListenerStarted.Invoke(this, prefix);
                    }

                    // Listen for Requests
                    while (listener.IsListening && !cancellationToken.IsCancellationRequested)
                    {
                        var result = listener.BeginGetContext(ListenerCallback, listener);
                        result.AsyncWaitHandle.WaitOne(1000);
                    }
                }
                catch (Exception ex)
                {
                    errorOccurred = true;
                    if (ListenerException != null) ListenerException.Invoke(this, ex);
                }
                finally
                {
                    listener.Abort();

                    // Raise Events to notify when a prefix is stopped being listened on
                    if (ListenerStopped != null)
                    {
                        foreach (var prefix in _prefixes) ListenerStopped.Invoke(this, prefix);
                    }
                }

                // Delay 1 second when listener errors (to prevent a reoccurring error from overloading)
                if (errorOccurred) await Task.Delay(1000);

            } while (!cancellationToken.IsCancellationRequested);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var listenerClosure = (HttpListener)result.AsyncState;
                var contextClosure = listenerClosure.EndGetContext(result);

                _= Task.Run(async () =>
                {
                    var request = contextClosure.Request;
                    var response = contextClosure.Response;

                    try
                    {
                        if (ClientConnected != null) ClientConnected.Invoke(this, request);

                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                        response.Headers.Add("Access-Control-Allow-Methods", "POST, PUT, GET, DELETE");

                        var uri = request.Url;
                        var method = request.HttpMethod;

                        if (uri != null)
                        {
                            switch (method)
                            {
                                case "GET":

                                    // Get MTConnect Request Type
                                    switch (GetRequestType(uri))
                                    {
                                        case MTConnectRequestType.Probe: await ProcessProbe(request, response); break;
                                        case MTConnectRequestType.Current: await ProcessCurrent(request, response); break;
                                        case MTConnectRequestType.Sample: await ProcessSample(request, response); break;
                                        case MTConnectRequestType.Assets: await ProcessAssets(request, response); break;
                                        case MTConnectRequestType.Asset: await ProcessAsset(request, response); break;
                                        default: await ProcessStatic(request, response); break;
                                    }

                                    break;

                                case "PUT":

                                    if (_configuration != null && _configuration.AllowPut)
                                    {
                                        await ProcessPut(request, response);
                                    }
                                    else
                                    {
                                        contextClosure.Response.StatusCode = 405;
                                    }

                                    break;

                                case "POST":

                                    if (_configuration != null && _configuration.AllowPut)
                                    {
                                        await ProcessPost(request, response);
                                    }
                                    else
                                    {
                                        contextClosure.Response.StatusCode = 405;
                                    }
                                    break;

                                case "DELETE":

                                    contextClosure.Response.StatusCode = 200;

                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ClientException != null) ClientException.Invoke(this, ex);
                    }
                    finally
                    {
                        response.Close();

                        if (ClientDisconnected != null) ClientDisconnected.Invoke(this, request.LocalEndPoint.ToString());
                    }
                });
            }
            catch (HttpListenerException ex)
            {
                // Ignore Disposed Object Exception (happens when the listener is stopped)
                if (ex.ErrorCode != 995)
                {
                    if (ClientException != null) ClientException.Invoke(this, ex);
                }
            }
            catch (ObjectDisposedException ex) { }
            catch (Exception ex)
            {
                if (ClientException != null) ClientException.Invoke(this, ex);
            }
        }

        private static string GetRequestType(Uri uri)
        {
            var segments = GetUriSegments(uri);
            if (!segments.IsNullOrEmpty())
            {
                if (segments.Length > 1)
                {
                    // Get second to last Uri segment
                    var segment = segments[segments.Length - 2];

                    // Check for single Asset Request
                    if (!string.IsNullOrEmpty(segment) && segment.ToLower() == "asset")
                    {
                        return MTConnectRequestType.Asset;
                    }

                    var path = segments[segments.Length - 1];
                    switch (path.ToLower())
                    {
                        case MTConnectRequestType.Probe: return MTConnectRequestType.Probe;
                        case MTConnectRequestType.Current: return MTConnectRequestType.Current;
                        case MTConnectRequestType.Sample: return MTConnectRequestType.Sample;
                        case MTConnectRequestType.Assets: return MTConnectRequestType.Assets;
                    }
                }
                else
                {
                    var path = segments[segments.Length - 1];
                    switch (path.ToLower())
                    {
                        case MTConnectRequestType.Probe: return MTConnectRequestType.Probe;
                        case MTConnectRequestType.Current: return MTConnectRequestType.Current;
                        case MTConnectRequestType.Sample: return MTConnectRequestType.Sample;
                        case MTConnectRequestType.Assets: return MTConnectRequestType.Assets;
                    }

                    if (!Path.HasExtension(path)) return MTConnectRequestType.Probe;
                }
            }
            else return MTConnectRequestType.Probe;

            return "";
        }

        private static string[] GetUriSegments(Uri uri)
        {
            var segments = new List<string>();

            if (uri != null && !uri.Segments.IsNullOrEmpty())
            {
                foreach (var segment in uri.Segments)
                {
                    var x = segment.Trim('/');
                    if (!string.IsNullOrEmpty(x)) segments.Add(x);
                }
            }

            return segments.ToArray();
        }

        private static string GetDeviceName(Uri url, string requestType)
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


        /// <summary>
        /// Write a MTConnectHttpResponse to the HttpListenerResponse Output Stream
        /// </summary>
        private async Task WriteResponse(MTConnectHttpResponse mtconnectResponse, HttpListenerResponse httpResponse)
        {
            if (httpResponse != null)
            {
                try
                {
                    using (var stream = httpResponse.OutputStream)
                    {
                        httpResponse.ContentType = mtconnectResponse.ContentType;
                        httpResponse.StatusCode = mtconnectResponse.StatusCode;

                        var bytes = Encoding.ASCII.GetBytes(mtconnectResponse.Content);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Write a string to the HttpListenerResponse Output Stream
        /// </summary>
        private async Task WriteResponse(string content, HttpListenerResponse httpResponse, HttpStatusCode statusCode, string contentType = MimeTypes.XML)
        {
            if (httpResponse != null)
            {
                try
                {
                    using (var stream = httpResponse.OutputStream)
                    {
                        httpResponse.ContentType = contentType;
                        httpResponse.StatusCode = (int)statusCode;

                        var bytes = Encoding.ASCII.GetBytes(content);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
                catch { }
            }
        }


        /// <summary>
        /// An Agent responds to a Probe Request with an MTConnectDevices Response Document that contains the 
        /// Equipment Metadata for pieces of equipment that are requested and currently represented in the Agent.
        /// </summary>
        private async Task ProcessProbe(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                // Read DeviceName from URL Path
                var deviceName = GetDeviceName(httpRequest.Url, MTConnectRequestType.Probe);

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.Version;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Probe, documentFormat, version);

                // Read ValidationLevel from Query string
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevelString));
                else formatOptions.Add(new KeyValuePair<string, string>("validationLevel", ((int)_configuration.ValidationLevel).ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                if (!string.IsNullOrEmpty(deviceName))
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    var response = await MTConnectHttpRequests.GetDeviceProbeRequest(_mtconnectAgent, deviceName, version, documentFormat, formatOptions);
                    await WriteResponse(response, httpResponse);
                    ResponseSent?.Invoke(this, response);
                }
                else
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    var response = await MTConnectHttpRequests.GetProbeRequest(_mtconnectAgent, version, documentFormat, formatOptions);
                    await WriteResponse(response, httpResponse);
                    ResponseSent?.Invoke(this, response);
                }
            }
        }

        /// <summary>
        /// An Agent responds to a Current Request with an MTConnectStreams Response Document that contains
        /// the current value of Data Entities associated with each piece of Streaming Data available from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        private async Task ProcessCurrent(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                // Read DeviceName from URL Path
                var deviceName = GetDeviceName(httpRequest.Url, MTConnectRequestType.Current);

                // Read "path" parameter from Query string
                var path = httpRequest.QueryString["path"];

                // Read "at" parameter from Query string
                var at = httpRequest.QueryString["at"].ToLong();

                // Read "interval" parameter from Query string
                var interval = httpRequest.QueryString["interval"].ToInt();

                // Read "heartbeat" parameter from Query string
                var heartbeat = httpRequest.QueryString["heartbeat"].ToInt();

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.Version;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Current, documentFormat, version);

                // Read ValidationLevel from Query string
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevelString));
                else formatOptions.Add(new KeyValuePair<string, string>("validationLevel", ((int)_configuration.ValidationLevel).ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                if (interval > 0)
                {
                    var currentStream = new MTConnectHttpCurrentStream(_mtconnectAgent, deviceName, path, interval, heartbeat, documentFormat, formatOptions);

                    try
                    {
                        using (var responseStream = httpResponse.OutputStream)
                        {
                            // Create Sample Stream
                            currentStream.HeartbeatReceived += async (s, args) => await WriteFromStream(currentStream, responseStream, args);
                            currentStream.DocumentReceived += async (s, args) => await WriteFromStream(currentStream, responseStream, args);

                            // Set HTTP Response Headers
                            httpResponse.Headers.Add("Server", "MTConnectAgent");
                            httpResponse.Headers.Add("Expires", "-1");
                            httpResponse.Headers.Add("Connection", "close");
                            httpResponse.Headers.Add("Cache-Control", "no-cache, private, max-age=0");
                            httpResponse.Headers.Add("Content-Type", $"multipart/x-mixed-replace;boundary={currentStream.Boundary}");

                            // Start the MTConnectHttpStream
                            currentStream.Start(CancellationToken.None);

                            while (true) { await Task.Delay(100); }
                        }
                    }
                    catch { }
                    finally
                    {
                        if (currentStream != null) currentStream.Stop();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(deviceName))
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = await MTConnectHttpRequests.GetDeviceCurrentRequest(_mtconnectAgent, deviceName, path, at, interval, version, documentFormat, formatOptions);
                        await WriteResponse(response, httpResponse);
                        ResponseSent?.Invoke(this, response);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = await MTConnectHttpRequests.GetCurrentRequest(_mtconnectAgent, path, at, interval, version, documentFormat, formatOptions);
                        await WriteResponse(response, httpResponse);
                        ResponseSent?.Invoke(this, response);
                    }
                }
            }
        }


        /// <summary>
        /// An Agent responds to a Sample Request with an MTConnectStreams Response Document that contains a set of values for Data Entities
        /// currently available for Streaming Data from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        private async Task ProcessSample(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                // Read DeviceName from URL Path
                var deviceName = GetDeviceName(httpRequest.Url, MTConnectRequestType.Sample);

                // Read "path" parameter from Query string
                var path = httpRequest.QueryString["path"];

                // Read "from" parameter from Query string
                var from = httpRequest.QueryString["from"].ToLong();

                // Read "to" parameter from Query string
                var to = httpRequest.QueryString["to"].ToLong();
                to = Math.Max(0, to);

                // Read "count" parameter from Query string
                var count = httpRequest.QueryString["count"].ToInt();
                if (count < 1) count = 100;

                // Read "interval" parameter from Query string
                var interval = httpRequest.QueryString["interval"].ToInt();

                // Read "heartbeat" parameter from Query string
                var heartbeat = httpRequest.QueryString["heartbeat"].ToInt();

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.Version;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Sample, documentFormat, version);

                // Read ValidationLevel from Query string
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevelString));
                else formatOptions.Add(new KeyValuePair<string, string>("validationLevel", ((int)_configuration.ValidationLevel).ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                if (interval > 0)
                {
                    var sampleStream = new MTConnectHttpSampleStream(_mtconnectAgent, deviceName, path, from, count, interval, heartbeat, documentFormat, formatOptions);

                    try
                    {
                        using (var responseStream = httpResponse.OutputStream)
                        {
                            // Create Sample Stream
                            sampleStream.HeartbeatReceived += async (s, args) => await WriteFromStream(sampleStream, responseStream, args);
                            sampleStream.DocumentReceived += async (s, args) => await WriteFromStream(sampleStream, responseStream, args);

                            // Set HTTP Response Headers
                            httpResponse.Headers.Add("Server", "MTConnectAgent");
                            httpResponse.Headers.Add("Expires", "-1");
                            httpResponse.Headers.Add("Connection", "close");
                            httpResponse.Headers.Add("Cache-Control", "no-cache, private, max-age=0");
                            httpResponse.Headers.Add("Content-Type", $"multipart/x-mixed-replace;boundary={sampleStream.Boundary}");

                            // Start the MTConnectHttpStream
                            sampleStream.Start(CancellationToken.None);

                            while (true) { await Task.Delay(100); }
                        }
                    }
                    catch { }
                    finally
                    {
                        if (sampleStream != null) sampleStream.Stop();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(deviceName))
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = await MTConnectHttpRequests.GetDeviceSampleRequest(_mtconnectAgent, deviceName, path, from, to, count, version, documentFormat, formatOptions);
                        await WriteResponse(response, httpResponse);
                        ResponseSent?.Invoke(this, response);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = await MTConnectHttpRequests.GetSampleRequest(_mtconnectAgent, path, from, to, count, version, documentFormat, formatOptions);
                        await WriteResponse(response, httpResponse);
                        ResponseSent?.Invoke(this, response);
                    }
                }
            }
        }

        /// <summary>
        /// An Agent responds to an Asset Request with an MTConnectAssets Response Document that contains
        /// information for MTConnect Assets from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        private async Task ProcessAssets(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                // Read Type parameter from Query string
                var type = httpRequest.QueryString["type"];

                // Read Removed parameter from Query string
                var removed = httpRequest.QueryString["removed"].ToBoolean();

                // Read Count parameter from Query string
                var count = httpRequest.QueryString["count"].ToInt();

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.Version;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Assets, documentFormat, version);

                // Read ValidationLevel from Query string
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevelString));
                else formatOptions.Add(new KeyValuePair<string, string>("validationLevel", ((int)_configuration.ValidationLevel).ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                // Get MTConnectAssets document from the MTConnectAgent
                var response = await MTConnectHttpRequests.GetAssetsRequest(_mtconnectAgent, type, removed, count, version, documentFormat, formatOptions);
                await WriteResponse(response, httpResponse);
                ResponseSent?.Invoke(this, response);
            }
        }

        /// <summary>
        /// An Agent responds to an Asset Request with an MTConnectAssets Response Document that contains
        /// information for MTConnect Assets from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        private async Task ProcessAsset(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                var urlSegments = GetUriSegments(httpRequest.Url);

                // Read AssetId from URL Path
                var assetId = httpRequest.Url.LocalPath?.Trim('/');
                if (urlSegments.Length > 1)
                {
                    assetId = urlSegments[urlSegments.Length - 1];
                }

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.Version;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Asset, documentFormat, version);

                // Read ValidationLevel from Query string
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevelString));
                else formatOptions.Add(new KeyValuePair<string, string>("validationLevel", ((int)_configuration.ValidationLevel).ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                // Get MTConnectAssets document from the MTConnectAgent
                var response = await MTConnectHttpRequests.GetAssetRequest(_mtconnectAgent, assetId, version, documentFormat, formatOptions);
                await WriteResponse(response, httpResponse);
                ResponseSent?.Invoke(this, response);
            }
        }


        private async Task ProcessPut(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                if (httpRequest.QueryString != null && httpRequest.QueryString.Count > 0)
                {
                    var urlSegments = GetUriSegments(httpRequest.Url);

                    // Read DeviceKey from URL Path
                    var deviceKey = httpRequest.Url.LocalPath?.Trim('/');
                    if (urlSegments.Length > 1) deviceKey = urlSegments[urlSegments.Length - 1];

                    // Get list of KeyValuePairs from Url Query
                    var items = new List<KeyValuePair<string, string>>();
                    foreach (var key in httpRequest.QueryString.AllKeys)
                    {
                        var urlValue = httpRequest.QueryString[key];
                        if (!string.IsNullOrEmpty(urlValue))
                        {
                            // Decode the input that was read from the URL
                            var value = System.Web.HttpUtility.UrlDecode(urlValue);

                            // Add to list of items
                            items.Add(new KeyValuePair<string, string>(key, value));
                        }
                    }

                    if (!items.IsNullOrEmpty())
                    {
                        var success = false;

                        foreach (var item in items)
                        {
                            // Call the OnObservationInput method that is intended to be overridden by a derived class
                            success = await OnObservationInput(deviceKey, item.Key, item.Value);
                            if (!success) break;
                        }

                        if (success)
                        {
                            // Write the "<success/>" respone to the Http Response Stream
                            // along with a 200 Status Code
                            await WriteResponse("<success/>", httpResponse, HttpStatusCode.OK);
                        }
                        else
                        {
                            // Return MTConnectError Response Document along with a 404 Http Status Code
                            var errorDocument = await _mtconnectAgent.GetErrorAsync(ErrorCode.UNSUPPORTED, $"Cannot find device: {deviceKey}");
                            var mtconnectResponse = new MTConnectHttpResponse(errorDocument, 404, DocumentFormat.XML, 0, null);
                            await WriteResponse(mtconnectResponse, httpResponse);
                        }
                    }
                }
            }
        }

        private async Task ProcessPost(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                var requestBytes = await ReadRequestBytes(httpRequest.InputStream);
                if (!requestBytes.IsNullOrEmpty())
                {
                    var urlSegments = GetUriSegments(httpRequest.Url);

                    // Read AssetId from URL Path
                    var assetId = httpRequest.Url.LocalPath?.Trim('/');
                    if (urlSegments.Length > 1) assetId = urlSegments[urlSegments.Length - 1];

                    if (!string.IsNullOrEmpty(assetId))
                    {
                        // Get Device Key (UUID or Name)
                        var deviceKey = httpRequest.QueryString["device"];

                        // Get the Asset Type
                        var assetType = httpRequest.QueryString["type"];

                        // Call the OnAssetInput method that is intended to be overridden by a derived class
                        var success = await OnAssetInput(assetId, deviceKey, assetType, requestBytes);

                        if (success)
                        {
                            // Write the "<success/>" respone to the Http Response Stream
                            // along with a 200 Status Code
                            await WriteResponse("<success/>", httpResponse, HttpStatusCode.OK);
                        }
                        else
                        {
                            // Return MTConnectError Response Document along with a 404 Http Status Code
                            var errorDocument = await _mtconnectAgent.GetErrorAsync(ErrorCode.UNSUPPORTED, $"Cannot find device: {deviceKey}");
                            var mtconnectResponse = new MTConnectHttpResponse(errorDocument, 404, DocumentFormat.XML, 0, null);
                            await WriteResponse(mtconnectResponse, httpResponse);
                        }
                    }
                }
            }
        }

        private async Task<byte[]> ReadRequestBytes(Stream inputStream)
        {
            if (inputStream != null)
            {
                try
                {
                    var bufferSize = 1048576 * 2; // 2 MB
                    var bytes = new byte[bufferSize];
                    await inputStream.ReadAsync(bytes, 0, bytes.Length);

                    return TrimEnd(bytes);
                }
                catch { }
            }

            return null;
        }

        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }


        private async Task ProcessStatic(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpResponse != null)
            {
                try
                {
                    var statusCode = 404;
                    var contentType = "text/plain";
                    byte[] fileContents = null;
                    var relativePath = httpRequest.Url.LocalPath.Trim('/');

                    // Read MTConnectVersion from Query string
                    var versionString = httpRequest.QueryString["version"];
                    Version.TryParse(versionString, out var version);
                    if (version == null) version = _mtconnectAgent.Configuration.DefaultVersion;

                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                    if (File.Exists(filePath))
                    {
                        if (_configuration.DevicesStyle != null && relativePath == _configuration.DevicesStyle.Location)
                        {
                            fileContents = ReadDevicesStylesheet(filePath, version);
                        }
                        else if (_configuration.StreamsStyle != null && relativePath == _configuration.StreamsStyle.Location)
                        {
                            fileContents = ReadStreamsStylesheet(filePath, version);
                        }
                        else
                        {
                            fileContents = File.ReadAllBytes(filePath);
                        }

                        statusCode = fileContents != null ? 200 : 500;
                        contentType = MimeTypes.GetMimeType(Path.GetExtension(filePath));
                    }

                    // Set HTTP Response Status Code
                    httpResponse.StatusCode = statusCode;
                    httpResponse.ContentType = contentType;

                    // Write File Contents to Response Stream
                    if (fileContents != null)
                    {
                        using (var stream = httpResponse.OutputStream)
                        {
                            await stream.WriteAsync(fileContents, 0, fileContents.Length);
                        }
                    }
                }
                catch { }
            }
        }

        private byte[] ReadDevicesStylesheet(string filePath, Version mtconnectVersion)
        {
            if (filePath != null)
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(fileContents))
                    {
                        string s = fileContents;
                        string pattern = null;
                        string replace = null;

                        try
                        {
                            // Replace Devices Namespace
                            pattern = @"urn:mtconnect\.org:MTConnectDevices:(\d\.\d)";
                            replace = $@"urn:mtconnect.org:MTConnectDevices:{mtconnectVersion}";
                            s = Regex.Replace(fileContents, pattern, replace);
                        }
                        catch { }

                        try
                        {
                            // Replace Streams Namespace
                            pattern = @"urn:mtconnect\.org:MTConnectStreams:(\d\.\d)";
                            replace = $@"urn:mtconnect.org:MTConnectStreams:{mtconnectVersion}";
                            s = Regex.Replace(s, pattern, replace);
                        }
                        catch { }

                        return Encoding.UTF8.GetBytes(s);
                    }
                }
                catch { }
            }

            return null;
        }

        private byte[] ReadStreamsStylesheet(string filePath, Version mtconnectVersion)
        {
            if (filePath != null)
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(fileContents))
                    {
                        if (mtconnectVersion != null)
                        {
                            // Replace Streams Namespace
                            var pattern = @"urn:mtconnect\.org:MTConnectStreams:(\d\.\d)";
                            var replace = $@"urn:mtconnect.org:MTConnectStreams:{mtconnectVersion.Major}.{mtconnectVersion.Minor}";
                            fileContents = Regex.Replace(fileContents, pattern, replace);
                        }

                        return Encoding.UTF8.GetBytes(fileContents);
                    }
                }
                catch { }
            }

            return null;
        }


        private string ReadDevicesSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var versionKey = mtconnectVersion.ToString();
                string schema = null;
                lock (_lock) if (_devicesSchemas.TryGetValue(versionKey, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = $"MTConnectDevices_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
                    var path = Path.Combine(dir, filename);

                    try
                    {
                        schema = File.ReadAllText(path);
                        if (!string.IsNullOrEmpty(schema))
                        {
                            lock (_lock) _devicesSchemas.Add(versionKey, schema);
                        }
                    }
                    catch { }
                }

                return schema;
            }

            return null;
        }

        private string ReadStreamsSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var versionKey = mtconnectVersion.ToString();
                string schema = null;
                lock (_lock) if (_streamsSchemas.TryGetValue(versionKey, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = $"MTConnectStreams_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
                    var path = Path.Combine(dir, filename);

                    try
                    {
                        schema = File.ReadAllText(path);
                        if (!string.IsNullOrEmpty(schema))
                        {
                            lock (_lock) _streamsSchemas.Add(versionKey, schema);
                        }
                    }
                    catch { }
                }

                return schema;
            }

            return null;
        }

        private string ReadAssetsSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var versionKey = mtconnectVersion.ToString();
                string schema = null;
                lock (_lock) if (_assetsSchemas.TryGetValue(versionKey, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = $"MTConnectAssets_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
                    var path = Path.Combine(dir, filename);

                    try
                    {
                        schema = File.ReadAllText(path);
                        if (!string.IsNullOrEmpty(schema))
                        {
                            lock (_lock) _assetsSchemas.Add(versionKey, schema);
                        }
                    }
                    catch { }
                }

                return schema;
            }

            return null;
        }

        private string ReadCommonSchema(Version mtconnectVersion)
        {
            if (mtconnectVersion != null)
            {
                var key = "xlink";
                string schema = null;
                lock (_lock) if (_commonSchemas.TryGetValue(key, out var x)) schema = x;

                if (string.IsNullOrEmpty(schema))
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
                    var filename = "xlink.xsd";
                    var path = Path.Combine(dir, filename);

                    try
                    {
                        schema = File.ReadAllText(path);
                        if (!string.IsNullOrEmpty(schema))
                        {
                            lock (_lock) _commonSchemas.Add(key, schema);
                        }
                    }
                    catch { }
                }

                return schema;
            }

            return null;
        }


        private List<KeyValuePair<string, string>> CreateFormatOptions(string requestType, string documentFormat, Version mtconnectVersion)
        {
            var x = new List<KeyValuePair<string, string>>();

            switch (documentFormat)
            {
                case DocumentFormat.XML:

                    if (_configuration != null)
                    {
                        // Add XSD Schema (xlink)
                        x.Add(new KeyValuePair<string, string>("schema", ReadCommonSchema(mtconnectVersion)));

                        // Add XSD Schema
                        switch (requestType)
                        {
                            case MTConnectRequestType.Probe: x.Add(new KeyValuePair<string, string>("schema", ReadDevicesSchema(mtconnectVersion))); break;
                            case MTConnectRequestType.Current: x.Add(new KeyValuePair<string, string>("schema", ReadStreamsSchema(mtconnectVersion))); break;
                            case MTConnectRequestType.Sample: x.Add(new KeyValuePair<string, string>("schema", ReadStreamsSchema(mtconnectVersion))); break;
                            case MTConnectRequestType.Asset: x.Add(new KeyValuePair<string, string>("schema", ReadAssetsSchema(mtconnectVersion))); break;
                            case MTConnectRequestType.Assets: x.Add(new KeyValuePair<string, string>("schema", ReadAssetsSchema(mtconnectVersion))); break;
                        }                   

                        // Add Devices Stylesheet
                        if (_configuration.DevicesStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("devicesStyle.location", _configuration.DevicesStyle.Location));
                            x.Add(new KeyValuePair<string, string>("devicesStyle.path", _configuration.DevicesStyle.Path));
                        }

                        // Add Streams Stylesheet
                        if (_configuration.StreamsStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("streamsStyle.location", _configuration.StreamsStyle.Location));
                            x.Add(new KeyValuePair<string, string>("streamsStyle.path", _configuration.StreamsStyle.Path));
                        }

                        // Add Assets Stylesheet
                        if (_configuration.AssetsStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("assetsStyle.location", _configuration.AssetsStyle.Location));
                            x.Add(new KeyValuePair<string, string>("assetsStyle.path", _configuration.AssetsStyle.Path));
                        }

                        // Add Error Stylesheet
                        if (_configuration.ErrorStyle != null)
                        {
                            x.Add(new KeyValuePair<string, string>("errorStyle.location", _configuration.ErrorStyle.Location));
                            x.Add(new KeyValuePair<string, string>("errorStyle.path", _configuration.ErrorStyle.Path));
                        }
                    }

                    break;
            }

            return x;
        }


        private static async Task WriteToResponseStream(Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (responseStream != null)
            {
                var bytes = Encoding.ASCII.GetBytes(args.Message);
                await responseStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        private async Task WriteFromStream(MTConnectHttpCurrentStream currentStream, Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (currentStream != null && responseStream != null)
            {
                try
                {
                    await WriteToResponseStream(responseStream, args);
                }
                catch (Exception ex)
                {
                    if (ClientDisconnected != null) ClientDisconnected.Invoke(this, currentStream.Id);
                    currentStream.Stop();
                }
            }
        }

        private async Task WriteFromStream(MTConnectHttpSampleStream sampleStream, Stream responseStream, MTConnectHttpStreamArgs args)
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
    }
}

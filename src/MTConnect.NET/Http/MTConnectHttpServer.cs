﻿// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests
    /// </summary>
    public class MTConnectHttpServer
    {
        private const string DefaultServer = "localhost";
        private const int DefaultPort = 5000;
        private const string EmptyServer = "0.0.0.0";

        private static string DefaultPrefix = "http://" + DefaultServer + ":" + DefaultPort + "/";


        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly MTConnectAgentConfiguration _configuration;
        private readonly List<string> _prefixes = new List<string>();

        private CancellationTokenSource _stop;


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


        public MTConnectHttpServer(IMTConnectAgent mtconnectAgent, IEnumerable<string> prefixes = null)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = mtconnectAgent.Configuration;

            LoadPrefixes(mtconnectAgent.Configuration, prefixes);
        }


        private void LoadPrefixes(MTConnectAgentConfiguration configuration, IEnumerable<string> prefixes = null)
        {
            if (!prefixes.IsNullOrEmpty())
            {
                _prefixes.AddRange(prefixes);
            }
            else if (configuration != null)
            {
                var serverIp = DefaultServer;
                var port = DefaultPort;

                // Configuration Server IP
                if (!string.IsNullOrEmpty(configuration.ServerIp))
                {
                    if (configuration.ServerIp != EmptyServer)
                    {
                        serverIp = configuration.ServerIp;
                    }
                }

                // Configuration Port
                if (configuration.Port > 0)
                {
                    port = configuration.Port;
                }

                // Construct Prefix URL
                _prefixes.Add("http://" + serverIp + ":" + port + "/");

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
                        result.AsyncWaitHandle.WaitOne();
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
                                    }

                                    break;

                                case "PUT":

                                    if (_configuration != null && _configuration.AllowPut)
                                    {
                                        Console.WriteLine("PUT");

                                        contextClosure.Response.StatusCode = 200;
                                    }
                                    else
                                    {
                                        contextClosure.Response.StatusCode = 405;
                                    }

                                    break;

                                case "POST":

                                    if (_configuration != null && _configuration.AllowPut)
                                    {
                                        Console.WriteLine("POST");

                                        contextClosure.Response.StatusCode = 200;
                                    }
                                    else
                                    {
                                        contextClosure.Response.StatusCode = 405;
                                    }
                                    break;

                                case "DELETE":

                                    Console.WriteLine("DELETE");

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
            catch (Exception ex)
            {
                if (ClientException != null) ClientException.Invoke(this, ex);
            }
        }

        private static string GetRequestType(Uri uri)
        {
            var requestType = MTConnectRequestType.Probe;

            if (uri.Segments.Length > 0)
            {
                var path = uri.Segments[uri.Segments.Length - 1];
                switch (path.ToLower())
                {
                    case MTConnectRequestType.Probe: requestType = MTConnectRequestType.Probe; break;
                    case MTConnectRequestType.Current: requestType = MTConnectRequestType.Current; break;
                    case MTConnectRequestType.Sample: requestType = MTConnectRequestType.Sample; break;
                    case MTConnectRequestType.Assets: requestType = MTConnectRequestType.Assets; break;
                    case MTConnectRequestType.Asset: requestType = MTConnectRequestType.Asset; break;
                }
            }

            return requestType;
        }

        private static string GetDeviceName(Uri url, string requestType)
        {
            if (url != null && url.Segments.Length > 0)
            {
                var max = url.Segments.Length - 1;

                // Work backwards from given index
                for (var i = max; i > 0; i--)
                {
                    if (url.Segments[i] == requestType)
                    {
                        if (i > 0)
                        {
                            return url.Segments[i - 1].Trim('/');
                        }
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

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                bool.TryParse(indentOutputString, out var indentOutput);


                if (!string.IsNullOrEmpty(deviceName))
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    var response = await MTConnectHttpRequests.GetDeviceProbeRequest(_mtconnectAgent, deviceName, version, documentFormat, indentOutput);
                    await WriteResponse(response, httpResponse);
                }
                else
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    var response = await MTConnectHttpRequests.GetProbeRequest(_mtconnectAgent, version, documentFormat, indentOutput);
                    await WriteResponse(response, httpResponse);
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

                // Read "at" parameter from Query string
                var at = httpRequest.QueryString["at"].ToLong();

                // Read "interval" parameter from Query string
                var interval = httpRequest.QueryString["interval"].ToInt();

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                bool.TryParse(indentOutputString, out var indentOutput);


                if (!string.IsNullOrEmpty(deviceName))
                {
                    // Get MTConnectStreams document from the MTConnectAgent
                    var response = await MTConnectHttpRequests.GetDeviceCurrentRequest(_mtconnectAgent, deviceName, null, at, interval, version, documentFormat, indentOutput);
                    await WriteResponse(response, httpResponse);
                }
                else
                {
                    // Get MTConnectStreams document from the MTConnectAgent
                    var response = await MTConnectHttpRequests.GetCurrentRequest(_mtconnectAgent, null, at, interval, version, documentFormat, indentOutput);
                    await WriteResponse(response, httpResponse);
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

                // Read "count" parameter from Query string
                var count = httpRequest.QueryString["count"].ToInt();

                // Read "interval" parameter from Query string
                var interval = httpRequest.QueryString["interval"].ToInt();

                // Read "heartbeat" parameter from Query string
                var heartbeat = httpRequest.QueryString["heartbeat"].ToInt();

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                bool.TryParse(indentOutputString, out var indentOutput);


                if (interval > 0)
                {
                    try
                    {
                        using (var responseStream = httpResponse.OutputStream)
                        {
                            // Create Sample Stream
                            var sampleStream = new MTConnectHttpStream(_mtconnectAgent, deviceName, path, from, count, interval, heartbeat, documentFormat, indentOutput);
                            sampleStream.HeartbeatReceived += async (s, args) =>
                            {
                                var bytes = Encoding.ASCII.GetBytes(args.Message);
                                await responseStream.WriteAsync(bytes, 0, bytes.Length);
                            };
                            sampleStream.DocumentReceived += async (s, args) =>
                            {
                                var bytes = Encoding.ASCII.GetBytes(args.Message);
                                await responseStream.WriteAsync(bytes, 0, bytes.Length);
                            };

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
                }
                else
                {
                    if (!string.IsNullOrEmpty(deviceName))
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = await MTConnectHttpRequests.GetDeviceSampleRequest(_mtconnectAgent, deviceName, null, from, to, count, version, documentFormat, indentOutput);
                        await WriteResponse(response, httpResponse);
                    }
                    else
                    {                        
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = await MTConnectHttpRequests.GetSampleRequest(_mtconnectAgent, null, from, to, count, version, documentFormat, indentOutput);
                        await WriteResponse(response, httpResponse);
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

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                bool.TryParse(indentOutputString, out var indentOutput);


                // Get MTConnectAssets document from the MTConnectAgent
                var response = await MTConnectHttpRequests.GetAssetsRequest(_mtconnectAgent, type, removed, count, version, documentFormat, indentOutput);
                await WriteResponse(response, httpResponse);
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
                // Read AssetId from URL Path
                var assetId = httpRequest.Url.LocalPath?.Trim('/');
                if (httpRequest.Url.Segments.Length > 2)
                {
                    assetId = httpRequest.Url.Segments[1]?.Trim('/');
                }

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                bool.TryParse(indentOutputString, out var indentOutput);


                // Get MTConnectAssets document from the MTConnectAgent
                var response = await MTConnectHttpRequests.GetAssetRequest(_mtconnectAgent, assetId, version, documentFormat, indentOutput);
                await WriteResponse(response, httpResponse);
            }
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Errors;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests
    /// </summary>
    public class MTConnectHttpServer : HttpServer
    {
        private const int _minimumHeartbeat = 500; // 500 ms
        private const int _defaultHeartbeat = 10000; // 10 Seconds

        protected readonly IMTConnectAgentBroker _mtconnectAgent;
        protected readonly IHttpAgentConfiguration _configuration;


        /// <summary>
        /// Event Handler for when an error occurs with a MTConnectHttpResponse is written to the HTTP Client
        /// </summary>
        public EventHandler<MTConnectHttpResponse> ResponseSent { get; set; }


        public MTConnectHttpServer(
            IHttpAgentConfiguration configuration,
            IMTConnectAgentBroker mtconnectAgent,
            IEnumerable<string> prefixes = null,
            int port = 0
            ) : base(configuration, prefixes, port)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = configuration;
        }


        /// <summary>
        /// Method run when an Observation is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual bool OnObservationInput(string deviceKey, string dataItemKey, string input)
        {
            return false;
        }

        /// <summary>
        /// Method run when an Asset is attempted to be added to the MTConnect Agent from an HTTP PUT request
        /// </summary>
        /// <returns>Returns False if a Device cannot be found from the specified DeviceKey</returns>
        protected virtual bool OnAssetInput(string assetId, string deviceKey, string assetType, byte[] requestBody, string documentFormat = DocumentFormat.XML)
        {
            return false;
        }


        protected override async Task OnRequestReceived(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            //if (ClientConnected != null) ClientConnected.Invoke(this, request);

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
                            case "Configuration": await ProcessConfiguration(request, response); break;
                            default: await ProcessStatic(request, response); break;
                        }

                        break;

                    case "PUT":

                        if (_configuration != null && _configuration.AllowPut)
                        {
                            bool allow;
                            if (!_configuration.AllowPutFrom.IsNullOrEmpty())
                            {
                                // Check that the remote address of the request is in the AllowPutFrom configuration parameter
                                var address = request.RemoteEndPoint.Address.ToString();
                                allow = _configuration.AllowPutFrom.Contains(address);
                            }
                            else allow = true;

                            if (allow) await ProcessPut(request, response);
                            else response.StatusCode = 403; // Return 403 - Forbidden
                        }
                        else
                        {
                            response.StatusCode = 405; // Return 405 - Method Not Supported
                        }

                        break;

                    case "POST":

                        if (_configuration != null && _configuration.AllowPut)
                        {
                            bool allow;
                            if (!_configuration.AllowPutFrom.IsNullOrEmpty())
                            {
                                // Check that the remote address of the request is in the AllowPutFrom configuration parameter
                                var address = request.RemoteEndPoint.Address.ToString();
                                allow = _configuration.AllowPutFrom.Contains(address);
                            }
                            else allow = true;

                            if (allow) await ProcessPost(request, response);
                            else response.StatusCode = 403; // Return 403 - Forbidden
                        }
                        else
                        {
                            response.StatusCode = 405; // Return 405 - Method Not Supported
                        }
                        break;

                    case "DELETE":

                        response.StatusCode = 405; // Return 405 - Method Not Supported

                        break;
                }
            }
        }

        protected override bool IsStreamRequest(HttpListenerContext context)
        {
            if (context != null && context.Request != null)
            {
                var method = context.Request.HttpMethod;

                if (method == "GET")
                {
                    // Read "interval" parameter from Query string
                    return context.Request.QueryString["interval"].ToInt() >= 0;
                }
            }

            return false;
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
                        case "configuration": return "Configuration";
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

        private static string GetDeviceKey(Uri url, string requestType)
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

        #region "Process Requests"

        /// <summary>
        /// An Agent responds to a Probe Request with an MTConnectDevices Response Document that contains the 
        /// Equipment Metadata for pieces of equipment that are requested and currently represented in the Agent.
        /// </summary>
        private async Task ProcessProbe(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpRequest.Url != null && httpResponse != null)
            {
                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.Url, MTConnectRequestType.Probe);

                // Read Device Type from URL Path
                var deviceType = httpRequest.QueryString["deviceType"];

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.MTConnectVersion;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read ValidationLevel from Query string
                int validationLevel = (int)_configuration.OutputValidationLevel;
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Probe, documentFormat, version, validationLevel);
                formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));


                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                if (!string.IsNullOrEmpty(deviceKey))
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    var response = MTConnectHttpRequests.GetDeviceProbeRequest(_mtconnectAgent, deviceKey, version, documentFormat, formatOptions);
                    response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
                    ResponseSent?.Invoke(this, response);
                }
                else
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    var response = MTConnectHttpRequests.GetProbeRequest(_mtconnectAgent, deviceType, version, documentFormat, formatOptions);
                    response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
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
                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.Url, MTConnectRequestType.Current);

                // Read Device Type from URL Path
                var deviceType = httpRequest.QueryString["deviceType"];

                // Read "path" parameter from Query string
                var path = httpRequest.QueryString["path"];

                // Read "at" parameter from Query string
                var at = httpRequest.QueryString["at"].ToLong();
                if (at < 1) at = 0;

                // Read "interval" parameter from Query string
                var interval = httpRequest.QueryString["interval"].ToInt();

                // Read "heartbeat" parameter from Query string
                var heartbeat = httpRequest.QueryString["heartbeat"].ToInt();
                if (heartbeat < 1) heartbeat = _defaultHeartbeat;
                if (heartbeat < _minimumHeartbeat) heartbeat = _minimumHeartbeat;

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.MTConnectVersion;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read ValidationLevel from Query string
                int validationLevel = (int)_configuration.OutputValidationLevel;
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Current, documentFormat, version, validationLevel);
                formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                if (interval > -1)
                {
                    // Get list of DataItem ID's based on Path (XPath) parameter
                    var dataItemIds = PathProcessor.GetDataItemIds(_mtconnectAgent, path, documentFormat);

                    var sampleStream = new MTConnectHttpServerStream(
                        _configuration,
                        _mtconnectAgent,
                        deviceKey,
                        dataItemIds,
                        0,
                        0,
                        interval,
                        heartbeat,
                        documentFormat,
                        acceptEncodings,
                        formatOptions
                        );

                    try
                    {
                        using (var responseStream = httpResponse.OutputStream)
                        {
                            // Create Sample Stream
                            sampleStream.HeartbeatReceived += async (s, args) => await WriteFromStream(sampleStream, responseStream, args);
                            sampleStream.DocumentReceived += async (s, args) => await WriteFromStream(sampleStream, responseStream, args);

                            // Set HTTP Headers and Settings for Streaming
                            SetupStreamResponse(ref httpResponse, sampleStream.Boundary);

                            // Run the MTConnectHttpStream
                            sampleStream.RunCurrent();
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
                    if (!string.IsNullOrEmpty(deviceKey))
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = MTConnectHttpRequests.GetDeviceCurrentRequest(_mtconnectAgent, deviceKey, path, at, interval, version, documentFormat, formatOptions);
                        response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
                        ResponseSent?.Invoke(this, response);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = MTConnectHttpRequests.GetCurrentRequest(_mtconnectAgent, deviceType, path, at, interval, version, documentFormat, formatOptions);
                        response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
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
                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.Url, MTConnectRequestType.Sample);

                // Read Device Type from URL Path
                var deviceType = httpRequest.QueryString["deviceType"];

                // Read "path" parameter from Query string
                var path = httpRequest.QueryString["path"];

                // Read "from" parameter from Query string
                var from = httpRequest.QueryString["from"].ToLong();
                if (from < 1) from = 0;

                // Read "to" parameter from Query string
                var to = httpRequest.QueryString["to"].ToLong();
                if (to < 1) to = 0;

                // Read "count" parameter from Query string
                var count = httpRequest.QueryString["count"].ToInt();
                if (count < 1) count = 100;

                // Read "interval" parameter from Query string
                var interval = httpRequest.QueryString["interval"].ToInt();

                // Read "heartbeat" parameter from Query string
                var heartbeat = httpRequest.QueryString["heartbeat"].ToInt();
                if (heartbeat < 1) heartbeat = _defaultHeartbeat;
                if (heartbeat < _minimumHeartbeat) heartbeat = _minimumHeartbeat;

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.MTConnectVersion;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read ValidationLevel from Query string
                int validationLevel = (int)_configuration.OutputValidationLevel;
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Sample, documentFormat, version, validationLevel);
                formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                if (interval > -1)
                {
                    // Get list of DataItem ID's based on Path (XPath) parameter
                    var dataItemIds = PathProcessor.GetDataItemIds(_mtconnectAgent, path, documentFormat);

                    var sampleStream = new MTConnectHttpServerStream(
                        _configuration, 
                        _mtconnectAgent, 
                        deviceKey, 
                        dataItemIds, 
                        from,
                        count, 
                        interval, 
                        heartbeat, 
                        documentFormat, 
                        acceptEncodings, 
                        formatOptions
                        );

                    try
                    {
                        using (var responseStream = httpResponse.OutputStream)
                        {
                            // Create Sample Stream
                            sampleStream.HeartbeatReceived += async (s, args) => await WriteFromStream(sampleStream, responseStream, args);
                            sampleStream.DocumentReceived += async (s, args) => await WriteFromStream(sampleStream, responseStream, args);

                            // Set HTTP Headers and Settings for Streaming
                            SetupStreamResponse(ref httpResponse, sampleStream.Boundary);

                            // Run the MTConnectHttpStream
                            sampleStream.RunSample();
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
                    if (!string.IsNullOrEmpty(deviceKey))
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = MTConnectHttpRequests.GetDeviceSampleRequest(_mtconnectAgent, deviceKey, path, from, to, count, version, documentFormat, formatOptions);
                        response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
                        ResponseSent?.Invoke(this, response);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        var response = MTConnectHttpRequests.GetSampleRequest(_mtconnectAgent, deviceType, path, from, to, count, version, documentFormat, formatOptions);
                        response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
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
                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.Url, MTConnectRequestType.Assets);

                // Read Type parameter from Query string
                var type = httpRequest.QueryString["type"];

                // Read Removed parameter from Query string
                var removed = httpRequest.QueryString["removed"].ToBoolean();

                // Read Count parameter from Query string
                var count = httpRequest.QueryString["count"].ToInt();
                if (count < 1) count = 100;

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.MTConnectVersion;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read ValidationLevel from Query string
                int validationLevel = (int)_configuration.OutputValidationLevel;
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Assets, documentFormat, version, validationLevel);
                formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                // Get MTConnectAssets document from the MTConnectAgent
                var response = MTConnectHttpRequests.GetAssetsRequest(_mtconnectAgent, deviceKey, type, removed, count, version, documentFormat, formatOptions);
                response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
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

                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read AssetIds from URL Path
                var assetIdsString = httpRequest.Url.LocalPath?.Trim('/');
                if (urlSegments.Length > 1)
                {
                    assetIdsString = urlSegments[urlSegments.Length - 1];
                }

                // Create list of AssetIds
                IEnumerable<string> assetIds = null;
                if (!string.IsNullOrEmpty(assetIdsString))
                {
                    assetIds = assetIdsString.Split(';');
                    if (assetIds.IsNullOrEmpty()) assetIds = new List<string>() { assetIdsString };
                }

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) version = _mtconnectAgent.MTConnectVersion;

                // Read DocumentFormat from Query string
                var documentFormatString = httpRequest.QueryString["documentFormat"];
                var documentFormat = DocumentFormat.XML;
                if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
                {
                    documentFormat = DocumentFormat.JSON;
                }

                // Read ValidationLevel from Query string
                int validationLevel = (int)_configuration.OutputValidationLevel;
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Asset, documentFormat, version, validationLevel);
                formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


                // Get MTConnectAssets document from the MTConnectAgent
                var response = MTConnectHttpRequests.GetAssetRequest(_mtconnectAgent, assetIds, version, documentFormat, formatOptions);
                response.WriteDuration = await WriteResponse(response, httpResponse, acceptEncodings);
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
                            success = OnObservationInput(deviceKey, item.Key, item.Value);
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
                            var errorDocument = _mtconnectAgent.GetErrorResponseDocument(ErrorCode.UNSUPPORTED, $"Cannot find device: {deviceKey}");
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

                        // Set Document Format
                        var documentFormat = DocumentFormat.XML;
                        if (httpRequest.ContentType == "application/json") documentFormat = DocumentFormat.JSON;

                        // Call the OnAssetInput method that is intended to be overridden by a derived class
                        var success = OnAssetInput(assetId, deviceKey, assetType, requestBytes, documentFormat);

                        if (success)
                        {
                            // Write the "<success/>" respone to the Http Response Stream
                            // along with a 200 Status Code
                            await WriteResponse("<success/>", httpResponse, HttpStatusCode.OK);
                        }
                        else
                        {
                            // Return MTConnectError Response Document along with a 404 Http Status Code
                            var errorDocument = _mtconnectAgent.GetErrorResponseDocument(ErrorCode.UNSUPPORTED, $"Cannot find device: {deviceKey}");
                            var mtconnectResponse = new MTConnectHttpResponse(errorDocument, 404, DocumentFormat.XML, 0, null);
                            await WriteResponse(mtconnectResponse, httpResponse);
                        }
                    }
                }
            }
        }

        private async Task ProcessConfiguration(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            if (httpRequest != null && httpResponse != null)
            {
                try
                {
                    var dir = "D:\\TrakHound\\Source-Code\\MTConnect.NET\\applications\\MTConnect.NET-Applications-Configuration\\bin\\Debug\\net6.0";
                    var cmd = "MTConnect.NET-Applications-Configuration.exe";

                    var startInfo = new ProcessStartInfo("cmd", "/k " + cmd);
                    startInfo.RedirectStandardOutput = false;
                    startInfo.UseShellExecute = true;
                    startInfo.CreateNoWindow = false;
                    startInfo.WorkingDirectory = dir;

                    var stpw = Stopwatch.StartNew();

                    using (var process = new Process())
                    {
                        process.StartInfo = startInfo;
                        process.Start();
                    }

                    await Task.Delay(5000);

                    httpResponse.Redirect("http://localhost:5500");
                }
                catch { }
            }
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
                    var requestedPath = httpRequest.Url.LocalPath.Trim('/');
                    var localPath = requestedPath;

                    // Get File extension to prevent certain files (.exe, .dll, etc) from being accessed
                    // that could cause security concerns
                    var pathExtension = Path.GetExtension(requestedPath);
                    var valid = pathExtension != ".exe" && pathExtension != ".dll";

                    if (valid)
                    {
                        valid = false;

                        // Check to see if the path matches one that is configured
                        if (!_configuration.Files.IsNullOrEmpty())
                        {
                            var resource = Path.GetFileName(requestedPath);
                            contentType = MimeTypes.GetMimeType(resource);
                           
                            var relativePath = Path.GetDirectoryName(requestedPath);
                            if (!string.IsNullOrEmpty(relativePath)) relativePath = relativePath.Replace('\\', '/');
                            else relativePath = resource;

                            if (!string.IsNullOrEmpty(relativePath))
                            {
                                // Find a FileConfiguration whose Location matches the requested Resource
                                var fileConfiguration = _configuration.Files.FirstOrDefault(o => o.Location == resource);
                                if (fileConfiguration != null)
                                {
                                    // Rewrite the localPath to the one that is configured that matches the 'Location' property
                                    localPath = fileConfiguration.Path;
                                    valid = true;
                                }
                                else
                                {
                                    // Find a FileConfiguration whose Location matches the requested Resource
                                    fileConfiguration = _configuration.Files.FirstOrDefault(o => o.Location == relativePath);
                                    if (fileConfiguration != null)
                                    {
                                        // Rewrite the localPath to the one that is configured that matches the 'Location' property
                                        localPath = Path.Combine(fileConfiguration.Path, resource);
                                        localPath = localPath.Replace('/', '\\');
                                        valid = true;
                                    }
                                }
                            }
                        }
                    }

                    if (valid)
                    {
                        // Read MTConnectVersion from Query string
                        var versionString = httpRequest.QueryString["version"];
                        Version.TryParse(versionString, out var version);
                        if (version == null) version = _mtconnectAgent.Configuration.DefaultVersion;

                        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localPath);

                        if (File.Exists(filePath))
                        {
                            // Check Overridden method
                            fileContents = OnProcessStatic(httpRequest, filePath, localPath, version);

                            // If nothing found in the overridden method, then read directly from filePath
                            if (fileContents == null)
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
                            await WriteToStream(fileContents, httpResponse);
                        }
                    }
                }
                catch { }
            }
        }


        protected virtual byte[] OnProcessStatic(HttpListenerRequest httpRequest, string absolutePath, string relativePath, Version version = null) { return null; }

        #endregion

        #region "Requests"

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

        #endregion

        #region "Responses"

        /// <summary>
        /// Write a MTConnectHttpResponse to the HttpListenerResponse Output Stream
        /// </summary>
        private async Task<double> WriteResponse(MTConnectHttpResponse mtconnectResponse, HttpListenerResponse httpResponse, IEnumerable<string> acceptEncodings = null)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            if (httpResponse != null)
            {
                try
                {
                    httpResponse.ContentType = mtconnectResponse.ContentType;
                    httpResponse.StatusCode = mtconnectResponse.StatusCode;

                    await WriteToStream(mtconnectResponse.Content, httpResponse, acceptEncodings);
                }
                catch { }
            }

            stpw.Stop();
            return (double)stpw.ElapsedTicks / 10000;
        }

        /// <summary>
        /// Write a string to the HttpListenerResponse Output Stream
        /// </summary>
        private async Task WriteResponse(string content, HttpListenerResponse httpResponse, HttpStatusCode statusCode, string contentType = MimeTypes.XML, IEnumerable<string> acceptEncodings = null)
        {
            if (httpResponse != null)
            {
                try
                {
                    httpResponse.ContentType = contentType;
                    httpResponse.StatusCode = (int)statusCode;

                    var bytes = Encoding.UTF8.GetBytes(content);
                    await WriteToStream(bytes, httpResponse, acceptEncodings);
                }
                catch { }
            }
        }

        private async Task WriteToStream(byte[] bytes, HttpListenerResponse httpResponse, IEnumerable<string> acceptEncodings = null)
        {
            if (httpResponse != null && !bytes.IsNullOrEmpty())
            {
                try
                {
                    // Gzip
                    if (!_configuration.ResponseCompression.IsNullOrEmpty() && 
                        _configuration.ResponseCompression.Contains(HttpResponseCompression.Gzip) && 
                        !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains("gzip"))
                    {
                        httpResponse.AddHeader("Content-Encoding", "gzip");

                        using (var ms = new MemoryStream())
                        {
                            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                            {
                                zip.Write(bytes, 0, bytes.Length);
                            }
                            bytes = ms.ToArray();
                        }
                    }

#if NET5_0_OR_GREATER
                    else if (!_configuration.ResponseCompression.IsNullOrEmpty() &&
                        _configuration.ResponseCompression.Contains(HttpResponseCompression.Br) &&
                        !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains("br"))
                    {
                        httpResponse.AddHeader("Content-Encoding", "br");

                        using (var ms = new MemoryStream())
                        {
                            using (var zip = new BrotliStream(ms, CompressionMode.Compress, true))
                            {
                                zip.Write(bytes, 0, bytes.Length);
                            }
                            bytes = ms.ToArray();
                        }
                    }
#endif

                    else if (!_configuration.ResponseCompression.IsNullOrEmpty() &&
                        _configuration.ResponseCompression.Contains(HttpResponseCompression.Deflate) &&
                        !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains("deflate"))
                    {
                        httpResponse.AddHeader("Content-Encoding", "deflate");

                        using (var ms = new MemoryStream())
                        {
                            using (var zip = new DeflateStream(ms, CompressionMode.Compress, true))
                            {
                                zip.Write(bytes, 0, bytes.Length);
                            }
                            bytes = ms.ToArray();
                        }
                    }
                  

                    await httpResponse.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch { }
            }
        }


        private static async Task WriteToResponseStream(Stream responseStream, MTConnectHttpStreamArgs args)
        {
            if (responseStream != null)
            {
                await responseStream.WriteAsync(args.Message, 0, args.Message.Length);
            }
        }

        private async Task WriteFromStream(MTConnectHttpServerStream sampleStream, Stream responseStream, MTConnectHttpStreamArgs args)
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


        private IEnumerable<string> ProcessAcceptEncodings(IEnumerable<string> acceptEncodings)
        {
            if (!acceptEncodings.IsNullOrEmpty() && !_configuration.ResponseCompression.IsNullOrEmpty())
            {
                var output = new List<string>();

                // Gzip
                if (_configuration.ResponseCompression.Contains(HttpResponseCompression.Gzip) &&
                    !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains(HttpContentEncodings.Gzip))
                {
                    output.Add(HttpContentEncodings.Gzip);
                }

#if NET5_0_OR_GREATER
                else if (_configuration.ResponseCompression.Contains(HttpResponseCompression.Br) &&
                    !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains(HttpContentEncodings.Brotli))
                {
                    output.Add(HttpContentEncodings.Brotli);
                }
#endif

                else if (_configuration.ResponseCompression.Contains(HttpResponseCompression.Deflate) &&
                    !acceptEncodings.IsNullOrEmpty() && acceptEncodings.Contains(HttpContentEncodings.Deflate))
                {
                    output.Add(HttpContentEncodings.Deflate);
                }

                return output;
            }

            return null;
        }

        private static void SetupStreamResponse(ref HttpListenerResponse response, string boundary)
        {
            // Set HTTP Status Code
            response.StatusCode = 200;
            response.KeepAlive = true;
            response.SendChunked = true;

            // Set HTTP Response Headers
            response.Headers.Add("Server", "MTConnectAgent");
            response.Headers.Add("Expires", "-1");
            response.Headers.Add("Connection", "close");
            response.Headers.Add("Cache-Control", "no-cache, private, max-age=0");
            response.Headers.Add(HttpHeaders.ContentType, $"multipart/x-mixed-replace;boundary={boundary}");
        }

        #endregion


        protected virtual List<KeyValuePair<string, string>> OnCreateFormatOptions(string requestType, string documentFormat, Version mtconnectVersion, int validationLevel = 0) { return null; }

        private List<KeyValuePair<string, string>> CreateFormatOptions(string requestType, string documentFormat, Version mtconnectVersion, int validationLevel = 0)
        {
            var x = OnCreateFormatOptions(requestType, documentFormat, mtconnectVersion, validationLevel);

            return !x.IsNullOrEmpty() ? x : new List<KeyValuePair<string, string>>();
        }
    }
}

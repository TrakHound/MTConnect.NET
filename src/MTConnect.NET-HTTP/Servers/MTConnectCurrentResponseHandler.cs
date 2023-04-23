// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    class MTConnectCurrentResponseHandler : MTConnectResponseHandler
    {
        private const int _minimumHeartbeat = 500; // 500 ms
        private const int _defaultHeartbeat = 10000; // 10 Seconds


        public MTConnectCurrentResponseHandler(IHttpAgentConfiguration configuration, IMTConnectAgentBroker mtconnectAgent) : base(configuration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.OriginalPath != null && httpResponse != null)
            {
                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.OriginalPath, MTConnectRequestType.Current);

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
                        using (var responseStream = httpResponse.GetResponseStream())
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
                        return MTConnectHttpRequests.GetDeviceCurrentRequest(_mtconnectAgent, deviceKey, path, at, interval, version, documentFormat, formatOptions);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        return MTConnectHttpRequests.GetCurrentRequest(_mtconnectAgent, deviceType, path, at, interval, version, documentFormat, formatOptions);
                    }
                }
            }

            return new MTConnectHttpResponse();
        }
    }
}

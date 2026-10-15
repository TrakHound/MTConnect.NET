// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Errors;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// Ceen request handler for the MTConnect Current endpoint
    /// (GET /current and GET /{deviceKey}/current). Returns an
    /// MTConnectStreams Response Document containing the current
    /// observation snapshot for every data item visible to the agent,
    /// optionally filtered by an XPath in the path query parameter and
    /// anchored to a sequence number via at. When interval is supplied,
    /// the handler upgrades the response to a multipart
    /// x-mixed-replace stream and emits a fresh snapshot every interval
    /// milliseconds, interleaved with heartbeats. The response document
    /// format is negotiated via documentFormat (xml | json |
    /// json-cppagent), defaulting to XML; deviceType, version,
    /// indentOutput, and outputComments further shape the payload.
    /// </summary>
    class MTConnectCurrentResponseHandler : MTConnectHttpResponseHandler
    {
        private const int _minimumHeartbeat = 500; // 500 ms
        private const int _defaultHeartbeat = 10000; // 10 Seconds


        public MTConnectCurrentResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
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

                // Read version, documentFormat, validationLevel, indentOutput and outputComments
                var format = GetRequestFormat(httpRequest, MTConnectRequestType.Current);

                // Read "at" parameter from Query string
                if (!TryGetQueryInteger(httpRequest, "at", out var atValue) || atValue < 0)
                {
                    return CreateInvalidRequestResponse(ErrorText.InvalidRequestNegativeAt, format);
                }
                ulong? at = atValue.HasValue ? (ulong?)atValue.Value : null;

                // Read "interval" parameter from Query string (absent = not streaming)
                if (!TryGetQueryInteger(httpRequest, "interval", out var intervalValue) || intervalValue < 0 || intervalValue > int.MaxValue)
                {
                    return CreateInvalidRequestResponse("The interval parameter must be a non-negative integer number of milliseconds.", format);
                }
                var interval = intervalValue.HasValue ? (int)intervalValue.Value : -1;

                // The "at" parameter would make a stream repeat the same data
                if (at.HasValue && interval > -1)
                {
                    return CreateInvalidRequestResponse(ErrorText.InvalidRequestAtIntervalConjunction, format);
                }

                // Read "heartbeat" parameter from Query string
                if (!TryGetQueryInteger(httpRequest, "heartbeat", out var heartbeatValue) || heartbeatValue < 0 || heartbeatValue > int.MaxValue)
                {
                    return CreateInvalidRequestResponse("The heartbeat parameter must be a non-negative integer number of milliseconds.", format);
                }
                var heartbeat = heartbeatValue.HasValue && heartbeatValue.Value > 0 ? (int)heartbeatValue.Value : _defaultHeartbeat;
                if (heartbeat < _minimumHeartbeat) heartbeat = _minimumHeartbeat;


                if (interval > -1)
                {
                    // Get list of DataItem ID's based on Path (XPath) parameter
                    var dataItemIds = PathProcessor.GetDataItemIds(_mtconnectAgent, path, format.DocumentFormat);

                    var sampleStream = new MTConnectHttpServerStream(
                        _serverConfiguration,
                        _mtconnectAgent,
                        deviceKey,
                        dataItemIds,
                        0,
                        0,
                        interval,
                        heartbeat,
                        format.DocumentFormat,
                        acceptEncodings,
                        format.FormatOptions
                        );
                    sampleStream.DeviceType = deviceType;
                    sampleStream.MTConnectVersion = format.Version;

                    try
                    {
                        // Set HTTP Headers and Settings for Streaming, and send them before the first chunk
                        SetupStreamResponse(ref httpResponse, sampleStream.Boundary);
                        await httpResponse.FlushHeadersAsync();

                        using (var responseStream = httpResponse.GetResponseStream())
                        {
                            // Run the MTConnectHttpStream, writing each chunk before the next is produced
                            await sampleStream.RunCurrentAsync(args => WriteFromStream(sampleStream, responseStream, args), cancellationToken);
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
                        return MTConnectHttpRequests.GetDeviceCurrentRequest(_mtconnectAgent, deviceKey, path, at, interval, format.Version, format.DocumentFormat, format.FormatOptions);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        return MTConnectHttpRequests.GetCurrentRequest(_mtconnectAgent, deviceType, path, at, interval, format.Version, format.DocumentFormat, format.FormatOptions);
                    }
                }
            }

            return new MTConnectHttpResponse();
        }
    }
}

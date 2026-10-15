// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Errors;

namespace MTConnect.Servers
{
    /// <summary>
    /// Ceen request handler for the MTConnect Sample endpoint
    /// (GET /sample and GET /{deviceKey}/sample). Returns an
    /// MTConnectStreams Response Document containing a slice of
    /// historical observations bounded by the from / to / count query
    /// parameters, optionally filtered by an XPath in path. When
    /// interval is supplied, the handler upgrades the response to a
    /// multipart x-mixed-replace stream and emits successive sample
    /// windows every interval milliseconds, interleaved with
    /// heartbeats. The response document format is negotiated via
    /// documentFormat (xml | json | json-cppagent), defaulting to XML;
    /// deviceType, version, indentOutput, and outputComments further
    /// shape the payload.
    /// </summary>
    class MTConnectSampleResponseHandler : MTConnectHttpResponseHandler
    {
        private const int _minimumHeartbeat = 500; // 500 ms
        private const int _defaultHeartbeat = 10000; // 10 Seconds
        private const uint _defaultCount = 100;


        public MTConnectSampleResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.OriginalPath, MTConnectRequestType.Sample);

                // Read Device Type from URL Path
                var deviceType = httpRequest.QueryString["deviceType"];

                // Read "path" parameter from Query string
                var path = httpRequest.QueryString["path"];

                // Read version, documentFormat, validationLevel, indentOutput and outputComments
                var format = GetRequestFormat(httpRequest, MTConnectRequestType.Sample);

                // Read "from" parameter from Query string
                if (!TryGetQueryInteger(httpRequest, "from", out var fromValue) || fromValue < 0)
                {
                    return CreateInvalidRequestResponse(ErrorText.InvalidRequestNegativeFrom, format);
                }
                var from = fromValue.HasValue ? (ulong)fromValue.Value : 0;

                // Read "to" parameter from Query string
                if (!TryGetQueryInteger(httpRequest, "to", out var toValue) || toValue < 0)
                {
                    return CreateInvalidRequestResponse(ErrorText.InvalidRequestNegativeTo, format);
                }
                var to = toValue.HasValue ? (ulong)toValue.Value : 0;

                // Read "count" parameter from Query string
                if (!TryGetQueryInteger(httpRequest, "count", out var countValue))
                {
                    return CreateInvalidRequestResponse("The count parameter must be an integer.", format);
                }
                if (countValue < 0)
                {
                    // A negative count asks for the most recent observations, which this Agent does not support;
                    // reject it rather than silently returning the oldest observations instead
                    return CreateInvalidRequestResponse("A negative count (most recent observations) is not supported by this Agent.", format);
                }
                if (countValue > uint.MaxValue)
                {
                    var errorDocument = _mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeCount, format.Version);
                    return new MTConnectHttpResponse(errorDocument, 404, format.DocumentFormat, 0, format.FormatOptions);
                }
                var count = countValue.HasValue ? (uint)countValue.Value : _defaultCount;

                // Read "interval" parameter from Query string (absent = not streaming)
                if (!TryGetQueryInteger(httpRequest, "interval", out var intervalValue) || intervalValue < 0 || intervalValue > int.MaxValue)
                {
                    return CreateInvalidRequestResponse("The interval parameter must be a non-negative integer number of milliseconds.", format);
                }
                var interval = intervalValue.HasValue ? (int)intervalValue.Value : -1;

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
                        from,
                        count,
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
                            await sampleStream.RunSampleAsync(args => WriteFromStream(sampleStream, responseStream, args), cancellationToken);
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
                        return MTConnectHttpRequests.GetDeviceSampleRequest(_mtconnectAgent, deviceKey, path, from, to, count, format.Version, format.DocumentFormat, format.FormatOptions);
                    }
                    else
                    {
                        // Get MTConnectStreams document from the MTConnectAgent
                        return MTConnectHttpRequests.GetSampleRequest(_mtconnectAgent, deviceType, path, from, to, count, format.Version, format.DocumentFormat, format.FormatOptions);
                    }
                }
            }

            return new MTConnectHttpResponse();
        }
    }
}

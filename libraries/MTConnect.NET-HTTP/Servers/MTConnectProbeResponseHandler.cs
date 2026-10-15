// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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
    /// Ceen request handler for the MTConnect Probe endpoint
    /// (GET /probe and GET /{deviceKey}/probe, plus the bare GET /
    /// alias). Returns an MTConnectDevices Response Document describing
    /// the device metadata visible to the agent. The optional leading
    /// path segment selects a single device by name or UUID. The
    /// response document format is negotiated via the documentFormat
    /// query parameter (xml | json | json-cppagent) and defaults to
    /// XML; deviceType, version, validationLevel, indentOutput, and
    /// outputComments further shape the payload.
    /// </summary>
    class MTConnectProbeResponseHandler : MTConnectHttpResponseHandler
    {
        public MTConnectProbeResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.OriginalPath != null && httpResponse != null)
            {
                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.OriginalPath, MTConnectRequestType.Probe);

                // Read Device Type from URL Path
                var deviceType = httpRequest.QueryString["deviceType"];

                // Read version, documentFormat, validationLevel, indentOutput and outputComments
                var format = GetRequestFormat(httpRequest, MTConnectRequestType.Probe);

                if (!string.IsNullOrEmpty(deviceKey))
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    return MTConnectHttpRequests.GetDeviceProbeRequest(_mtconnectAgent, deviceKey, format.Version, format.DocumentFormat, format.FormatOptions);
                }
                else
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    return MTConnectHttpRequests.GetProbeRequest(_mtconnectAgent, deviceType, format.Version, format.DocumentFormat, format.FormatOptions);
                }
            }

            return new MTConnectHttpResponse();
        }
    }
}

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
    /// Ceen request handler for the MTConnect Assets endpoint
    /// (GET /assets and GET /{deviceKey}/assets). Returns an
    /// MTConnectAssets Response Document containing the asset records
    /// known to the agent. The optional leading path segment scopes the
    /// response to assets owned by a specific device; the type, removed,
    /// and count query parameters filter the asset set; documentFormat
    /// (xml | json | json-cppagent, default XML) and indentOutput
    /// negotiate the rendered representation.
    /// </summary>
    class MTConnectAssetsResponseHandler : MTConnectHttpResponseHandler
    {
        public MTConnectAssetsResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.OriginalPath, MTConnectRequestType.Assets);

                // Read Type parameter from Query string
                var type = httpRequest.QueryString["type"];

                // Read Removed parameter from Query string
                var removed = httpRequest.QueryString["removed"].ToBoolean();

                // Read Count parameter from Query string
                var count = httpRequest.QueryString["count"].ToInt();
                if (count < 1) count = 100;

                // Read version, documentFormat, validationLevel, indentOutput and outputComments
                var format = GetRequestFormat(httpRequest, MTConnectRequestType.Assets);

                // Get MTConnectAssets document from the MTConnectAgent
                return MTConnectHttpRequests.GetAssetsRequest(_mtconnectAgent, deviceKey, type, removed, (uint)count, format.Version, format.DocumentFormat, format.FormatOptions);
            }

            return new MTConnectHttpResponse();
        }
    }
}

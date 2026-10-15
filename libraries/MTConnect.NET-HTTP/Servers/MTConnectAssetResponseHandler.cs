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
    /// Ceen request handler for the single-asset MTConnect endpoint
    /// (GET /asset/{assetId}). Returns an MTConnectAssets Response
    /// Document containing the asset identified by the trailing path
    /// segment, or an MTConnectError document with status 404 when no
    /// such asset exists. The response document format is negotiated
    /// via the documentFormat query parameter (xml | json |
    /// json-cppagent) and defaults to XML.
    /// </summary>
    class MTConnectAssetResponseHandler : MTConnectHttpResponseHandler
    {
        public MTConnectAssetResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                var urlSegments = GetUriSegments(httpRequest.OriginalPath);

                // Read AssetIds from URL Path
                var assetIdsString = httpRequest.Path?.Trim('/');
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

                // Read version, documentFormat, validationLevel, indentOutput and outputComments
                var format = GetRequestFormat(httpRequest, MTConnectRequestType.Asset);

                // Get MTConnectAssets document from the MTConnectAgent
                return MTConnectHttpRequests.GetAssetRequest(_mtconnectAgent, assetIds, format.Version, format.DocumentFormat, format.FormatOptions);
            }

            return new MTConnectHttpResponse();
        }
    }
}

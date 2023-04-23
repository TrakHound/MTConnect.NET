// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTConnect.Servers
{
    class MTConnectAssetResponseHandler : MTConnectResponseHandler
    {
        public MTConnectAssetResponseHandler(IHttpAgentConfiguration configuration, IMTConnectAgentBroker mtconnectAgent) : base(configuration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            {
                var urlSegments = GetUriSegments(httpRequest.OriginalPath);

                // Get Accept-Encoding Header (ex. gzip, br)
                var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
                acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

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
                return MTConnectHttpRequests.GetAssetRequest(_mtconnectAgent, assetIds, version, documentFormat, formatOptions);
            }

            return new MTConnectHttpResponse();
        }
    }
}

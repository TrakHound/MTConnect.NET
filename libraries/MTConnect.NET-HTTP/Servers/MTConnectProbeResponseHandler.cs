﻿// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MTConnect.Servers.Http
{
    class MTConnectProbeResponseHandler : MTConnectHttpResponseHandler
    {
        public MTConnectProbeResponseHandler(
            IHttpServerConfiguration serverConfiguration, 
            IMTConnectAgentBroker mtconnectAgent, 
            ILogger logger = null) 
            : base(
                serverConfiguration, 
                mtconnectAgent, 
                logger) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpRequest.OriginalPath != null && httpResponse != null)
            {
                // Read DeviceKey from URL Path
                var deviceKey = GetDeviceKey(httpRequest.OriginalPath, MTConnectRequestType.Probe);

                // Read Device Type from URL Path
                var deviceType = httpRequest.QueryString["deviceType"];

                // Read MTConnectVersion from Query string
                var versionString = httpRequest.QueryString["version"];
                Version.TryParse(versionString, out var version);
                if (version == null) Version.TryParse(_serverConfiguration.DefaultVersion, out version);
                if (version == null) version = _mtconnectAgent.MTConnectVersion;

                // Read DocumentFormat from Query string
                var documentFormat = httpRequest.QueryString["documentFormat"];
                if (string.IsNullOrEmpty(documentFormat) && !_serverConfiguration.Accept.IsNullOrEmpty() && httpRequest.Headers["Accept"] != null)
                {
                    if (_serverConfiguration.Accept.ContainsKey(httpRequest.Headers["Accept"])) documentFormat = _serverConfiguration.Accept[httpRequest.Headers["Accept"]];
                }
                if (string.IsNullOrEmpty(documentFormat)) documentFormat = _serverConfiguration.DocumentFormat;

                // Read ValidationLevel from Query string
                int validationLevel = (int)_serverConfiguration.OutputValidationLevel;
                var validationLevelString = httpRequest.QueryString["validationLevel"];
                if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


                // Set Format Options
                var formatOptions = CreateFormatOptions(MTConnectRequestType.Probe, documentFormat, version, validationLevel);
                formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));

                // Read IndentOutput from Query string
                var indentOutputString = httpRequest.QueryString["indentOutput"];
                if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
                else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _serverConfiguration.IndentOutput.ToString()));

                // Read OutputComments from Query string
                var outputCommentsString = httpRequest.QueryString["outputComments"];
                if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
                else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _serverConfiguration.OutputComments.ToString()));


                if (!string.IsNullOrEmpty(deviceKey))
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    return MTConnectHttpRequests.GetDeviceProbeRequest(_mtconnectAgent, deviceKey, version, documentFormat, formatOptions);
                }
                else
                {
                    // Get MTConnectDevices document from the MTConnectAgent
                    return MTConnectHttpRequests.GetProbeRequest(_mtconnectAgent, deviceType, version, documentFormat, formatOptions);
                }
            }

            return new MTConnectHttpResponse();
        }
    }
}

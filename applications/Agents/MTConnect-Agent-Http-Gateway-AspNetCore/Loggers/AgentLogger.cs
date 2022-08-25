// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Logging;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Applications.Loggers
{
    public class AgentLogger
    {
        private readonly ILogger<AgentLogger> _logger;


        public AgentLogger(ILogger<AgentLogger> logger)
        {
            _logger = logger;
        }


        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }


        public void DevicesRequested(string deviceName)
        {
            _logger.LogInformation($"[Agent] : MTConnectDevices Requested : {deviceName}");
        }

        public void DevicesResponseSent(IDevicesResponseDocument document)
        {
            _logger.LogInformation($"[Agent] : MTConnectDevices Response Document Returned : {document.Header.CreationTime}");
        }


        public void StreamsRequested(string deviceName)
        {
            _logger.LogInformation($"[Agent] : MTConnectStreams Requested : {deviceName}");
        }

        public void StreamsResponseSent(object sender, EventArgs args)
        {
            _logger.LogInformation($"[Agent] : MTConnectStreams Response Document Returned");
        }


        public void AssetsRequested(IEnumerable<string> assetIds)
        {
            var ids = "";
            if (!assetIds.IsNullOrEmpty())
            {
                string.Join(";", assetIds.ToArray());
            }

            _logger.LogDebug($"[Agent] : MTConnectAssets Requested : AssetIds = " + ids);
        }

        public void DeviceAssetsRequested(string deviceUuid)
        {
            _logger.LogDebug($"[Agent] : MTConnectAssets Requested : DeviceUuid = " + deviceUuid);
        }

        public void AssetsResponseSent(IAssetsResponseDocument document)
        {
            _logger.LogInformation($"[Agent] : MTConnectAssets Response Document Returned : {document.Header.CreationTime}");
        }
    }
}

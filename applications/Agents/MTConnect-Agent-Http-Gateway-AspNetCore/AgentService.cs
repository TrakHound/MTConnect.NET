// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Applications.Configuration;
using MTConnect.Applications.Loggers;
using MTConnect.Assets;
using MTConnect.Clients.Rest;
using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Applications
{
    public class AgentService : IHostedService
    {
        private readonly MTConnectAgentRelayConfiguration _relayConfiguration;
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly ILogger<AgentService> _logger;
        private readonly AgentLogger _agentLogger;
        private System.Timers.Timer _metricsTimer;


        public AgentService(
            IMTConnectAgent mtconnectAgent, 
            AgentLogger agentLogger,
            AgentValidationLogger agentValidationLogger,
            ILogger<AgentService> logger
            )
        {
            _relayConfiguration = MTConnectAgentRelayConfiguration.Read();
            _mtconnectAgent = mtconnectAgent;
            _logger = logger;
            _agentLogger = agentLogger;

            _mtconnectAgent.DevicesRequestReceived += agentLogger.DevicesRequested;
            _mtconnectAgent.DevicesResponseSent += agentLogger.DevicesResponseSent;
            _mtconnectAgent.StreamsRequestReceived += agentLogger.StreamsRequested;
            _mtconnectAgent.StreamsResponseSent += agentLogger.StreamsResponseSent;
            _mtconnectAgent.AssetsRequestReceived += agentLogger.AssetsRequested;
            _mtconnectAgent.AssetsResponseSent += agentLogger.AssetsResponseSent;

            _mtconnectAgent.InvalidDataItemAdded += agentValidationLogger.InvalidDataItemAdded;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_mtconnectAgent.Configuration != null)
            {
                // Add Agent Clients
                if (!_relayConfiguration.Clients.IsNullOrEmpty())
                {
                    foreach (var clientConfiguration in _relayConfiguration.Clients)
                    {
                        if (!string.IsNullOrEmpty(clientConfiguration.Address))
                        {
                            string baseUrl = null;
                            var address = clientConfiguration.Address;
                            var port = clientConfiguration.Port;

                            if (clientConfiguration.UseSSL) address = address.Replace("https://", "");
                            else address = address.Replace("http://", "");

                            // Create the MTConnect Agent Base URL
                            if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", AddPort(address, port));
                            else baseUrl = string.Format("http://{0}", AddPort(address, port));

                            var agentClient = new MTConnectClient(baseUrl, clientConfiguration.DeviceName);
                            agentClient.Interval = clientConfiguration.Interval;
                            agentClient.Heartbeat = clientConfiguration.Heartbeat;

                            // Subscribe to the Event handlers to receive the MTConnect documents
                            agentClient.OnProbeReceived += (s, doc) => DevicesDocumentReceived(doc);
                            agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(doc);
                            agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(doc);
                            agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(doc);

                            agentClient.Start();
                        }
                    }
                }
            }

            StartMetrics();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_metricsTimer != null) _metricsTimer.Dispose();
        }


        private void DevicesDocumentReceived(IDevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                foreach (var device in document.Devices)
                {
                    Console.WriteLine(device.Id);
                    _mtconnectAgent.AddDevice(device);
                }
            }
        }

        private void StreamsDocumentReceived(IStreamsResponseDocument document)
        {
            if (document != null && !document.Streams.IsNullOrEmpty())
            {
                foreach (var stream in document.Streams)
                {
                    var observations = stream.Observations;
                    if (!observations.IsNullOrEmpty())
                    {
                        foreach (var observation in observations)
                        {
                            var input = new ObservationInput();
                            input.DataItemKey = observation.DataItemId;
                            input.DeviceKey = stream.Uuid;
                            input.Timestamp = observation.Timestamp.ToUnixTime();
                            input.Values = observation.Values;

                            _mtconnectAgent.AddObservation(stream.Name, input);
                        }
                    }
                }
            }
        }

        private void AssetsDocumentReceived(IAssetsResponseDocument document)
        {
            if (document != null && !document.Assets.IsNullOrEmpty())
            {
                foreach (var asset in document.Assets)
                {
                    _mtconnectAgent.AddAsset(asset.DeviceUuid, asset);
                }
            }
        }


        private void StartMetrics()
        {
            int observationLastCount = 0;
            int observationDelta = 0;
            int assetLastCount = 0;
            int assetDelta = 0;
            var updateInterval = _mtconnectAgent.Metrics.UpdateInterval.TotalSeconds;
            var windowInterval = _mtconnectAgent.Metrics.WindowInterval.TotalMinutes;

            _metricsTimer = new System.Timers.Timer();
            _metricsTimer.Interval = updateInterval * 1000;
            _metricsTimer.Elapsed += (s, e) =>
            {
                // Observations
                var observationCount = _mtconnectAgent.Metrics.GetObservationCount();
                var observationAverage = _mtconnectAgent.Metrics.ObservationAverage;
                observationDelta = observationCount - observationLastCount;

                _agentLogger.LogInformation("[Agent] : Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                _agentLogger.LogInformation("[Agent] : Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                // Assets
                var assetCount = _mtconnectAgent.Metrics.GetAssetCount();
                var assetAverage = _mtconnectAgent.Metrics.AssetAverage;
                assetDelta = assetCount - assetLastCount;

                _agentLogger.LogInformation("[Agent] : Assets - Delta for last " + updateInterval + " seconds: " + assetDelta);
                _agentLogger.LogInformation("[Agent] : Assets - Average for last " + windowInterval + " minutes: " + Math.Round(assetAverage, 5));

                observationLastCount = observationCount;
                assetLastCount = assetCount;
            };
            _metricsTimer.Start();
        }


        private static string AddPort(string url, int port)
        {
            if (!string.IsNullOrEmpty(url) && port > 0)
            {
                if (url.Contains('/'))
                {
                    var p = url.Split('/');
                    if (p.Length > 1)
                    {
                        p[0] = $"{p[0]}:{port}";
                    }

                    return string.Join('/', p);
                }

                return $"{url}:{port}";
            }

            return url;
        }
    }
}

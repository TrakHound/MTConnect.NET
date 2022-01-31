// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Applications.Loggers;
using MTConnect.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Applications
{
    public class AgentService : IHostedService
    {
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly ILogger<AgentService> _logger;
        private readonly AgentLogger _agentLogger;
        private readonly AdapterLogger _adapterLogger;
        private readonly AdapterShdrLogger _adapterShdrLogger;
        private readonly List<ShdrAdapterClient> _adapterClients = new List<ShdrAdapterClient>();
        private System.Timers.Timer _metricsTimer;


        public AgentService(
            IMTConnectAgent mtconnectAgent, 
            AgentLogger agentLogger,
            AgentValidationLogger agentValidationLogger,
            AdapterLogger adapterLogger,
            AdapterShdrLogger adapterShdrLogger,
            ILogger<AgentService> logger
            )
        {
            _mtconnectAgent = mtconnectAgent;
            _logger = logger;
            _agentLogger = agentLogger;
            _adapterLogger = adapterLogger;
            _adapterShdrLogger = adapterShdrLogger;

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
            _adapterClients.Clear();

            if (_mtconnectAgent.Configuration != null)
            {
                // Add Adapter Clients
                if (!_mtconnectAgent.Configuration.Adapters.IsNullOrEmpty())
                {
                    var devices = await Device.FromFileAsync(_mtconnectAgent.Configuration.Devices);
                    if (!devices.IsNullOrEmpty())
                    {
                        // Add Device(s) to Agent
                        foreach (var device in devices)
                        {
                            await _mtconnectAgent.AddDeviceAsync(device);
                        }

                        foreach (var adapterConfiguration in _mtconnectAgent.Configuration.Adapters)
                        {
                            var device = devices.FirstOrDefault(o => o.Name == adapterConfiguration.Device);
                            if (device != null)
                            {
                                var adapterClient = new ShdrAdapterClient(adapterConfiguration, _mtconnectAgent, device);
                                adapterClient.AdapterConnected += _adapterLogger.AdapterConnected;
                                adapterClient.AdapterDisconnected += _adapterLogger.AdapterDisconnected;
                                adapterClient.AdapterConnectionError += _adapterLogger.AdapterConnectionError;
                                adapterClient.PingSent += _adapterLogger.AdapterPingSent;
                                adapterClient.PongReceived += _adapterLogger.AdapterPongReceived;
                                adapterClient.ProtocolReceived += _adapterShdrLogger.AdapterProtocolReceived;
                                adapterClient.Start();
                                _adapterClients.Add(adapterClient);
                            }
                        }
                    }
                }
            }

            StartMetrics();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_adapterClients.IsNullOrEmpty())
            {
                foreach (var adapterClient in _adapterClients)
                {
                    adapterClient.Stop();
                }
            }

            if (_metricsTimer != null) _metricsTimer.Dispose();
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
    }
}

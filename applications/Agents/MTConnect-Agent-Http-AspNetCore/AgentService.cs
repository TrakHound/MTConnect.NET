// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Applications.Loggers;
using MTConnect.Configurations;
using MTConnect.Devices.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Applications
{
    public class AgentService : IHostedService
    {
        private readonly HttpShdrAgentConfiguration _configuration;
        private readonly IMTConnectAgent _mtconnectAgent;
        private IAgentConfigurationFileWatcher _agentConfigurationWatcher;
        private readonly ILogger<AgentService> _logger;
        private readonly AgentLogger _agentLogger;
        private readonly AgentMetricLogger _agentMetricLogger;
        private readonly AdapterLogger _adapterLogger;
        private readonly AdapterShdrLogger _adapterShdrLogger;
        private readonly List<ShdrAdapterClient> _adapterClients = new List<ShdrAdapterClient>();
        private System.Timers.Timer _metricsTimer;


        public AgentService(
            HttpShdrAgentConfiguration configuration,
            IMTConnectAgent mtconnectAgent, 
            AgentLogger agentLogger,
            AgentMetricLogger agentMetricLogger,
            AgentValidationLogger agentValidationLogger,
            AdapterLogger adapterLogger,
            AdapterShdrLogger adapterShdrLogger,
            ILogger<AgentService> logger
            )
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
            _logger = logger;
            _agentLogger = agentLogger;
            _agentMetricLogger = agentMetricLogger;
            _adapterLogger = adapterLogger;
            _adapterShdrLogger = adapterShdrLogger;

            _mtconnectAgent.DevicesRequestReceived += _agentLogger.DevicesRequested;
            _mtconnectAgent.DevicesResponseSent += _agentLogger.DevicesResponseSent;
            _mtconnectAgent.StreamsRequestReceived += _agentLogger.StreamsRequested;
            _mtconnectAgent.StreamsResponseSent += _agentLogger.StreamsResponseSent;
            _mtconnectAgent.AssetsRequestReceived += _agentLogger.AssetsRequested;
            _mtconnectAgent.DeviceAssetsRequestReceived += _agentLogger.DeviceAssetsRequested;
            _mtconnectAgent.AssetsResponseSent += _agentLogger.AssetsResponseSent;

            _mtconnectAgent.InvalidDataItemAdded += agentValidationLogger.InvalidDataItemAdded;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _adapterClients.Clear();

            if (_configuration != null)
            {
                // Add Adapter Clients
                if (!_configuration.Adapters.IsNullOrEmpty())
                {
                    var devices = await DeviceConfiguration.FromFileAsync(_configuration.Devices, DocumentFormat.XML);
                    if (!devices.IsNullOrEmpty())
                    {
                        // Add Device(s) to Agent
                        foreach (var device in devices)
                        {
                            _mtconnectAgent.AddDevice(device);
                        }

                        foreach (var adapterConfiguration in _configuration.Adapters)
                        {
                            var device = devices.FirstOrDefault(o => o.Name == adapterConfiguration.DeviceKey);
                            if (device != null)
                            {
                                var adapterComponent = new ShdrAdapterComponent(adapterConfiguration);

                                // Add Adapter Component to Agent Device
                                _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                                // Create new SHDR Adapter Client to read from SHDR stream
                                var adapterClient = new ShdrAdapterClient(adapterConfiguration, _mtconnectAgent, device);
                                adapterClient.Connected += _adapterLogger.AdapterConnected;
                                adapterClient.Disconnected += _adapterLogger.AdapterDisconnected;
                                adapterClient.ConnectionError += _adapterLogger.AdapterConnectionError;
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
            if (_configuration.EnableMetrics)
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

                    _agentMetricLogger.LogInformation("[Agent] : Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                    _agentMetricLogger.LogInformation("[Agent] : Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                    // Assets
                    var assetCount = _mtconnectAgent.Metrics.GetAssetCount();
                    var assetAverage = _mtconnectAgent.Metrics.AssetAverage;
                    assetDelta = assetCount - assetLastCount;

                    _agentMetricLogger.LogInformation("[Agent] : Assets - Delta for last " + updateInterval + " seconds: " + assetDelta);
                    _agentMetricLogger.LogInformation("[Agent] : Assets - Average for last " + windowInterval + " minutes: " + Math.Round(assetAverage, 5));

                    observationLastCount = observationCount;
                    assetLastCount = assetCount;
                };
                _metricsTimer.Start();
            }
        }
    }
}

﻿// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Applications.Loggers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Applications
{
    public class MTConnectAgentService : IHostedService
    {
        private readonly HttpShdrAgentConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private IAgentConfigurationFileWatcher _agentConfigurationWatcher;
        private readonly AgentLogger _agentLogger;
        private readonly AgentMetricLogger _agentMetricLogger;
        private readonly AdapterLogger _adapterLogger;
        private readonly AdapterShdrLogger _adapterShdrLogger;
        private readonly List<ShdrAdapterClient> _adapterClients = new List<ShdrAdapterClient>();
        private System.Timers.Timer _metricsTimer;


        public MTConnectAgentService(
            HttpShdrAgentConfiguration configuration,
            IMTConnectAgentBroker mtconnectAgent, 
            AgentLogger agentLogger,
            AgentMetricLogger agentMetricLogger,
            AgentValidationLogger agentValidationLogger,
            AdapterLogger adapterLogger,
            AdapterShdrLogger adapterShdrLogger
            )
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
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

                        // Device Specific Adapters (DeviceKey specified)
                        var deviceAdapters = _configuration.Adapters.Where(o => o.DeviceKey != ShdrClientConfiguration.DeviceKeyWildcard);
                        if (!deviceAdapters.IsNullOrEmpty())
                        {
                            foreach (var adapter in deviceAdapters)
                            {
                                // Find Device matching DeviceKey
                                var device = devices?.FirstOrDefault(o => o.Uuid == adapter.DeviceKey || o.Name == adapter.DeviceKey);
                                if (device != null) AddAdapter(adapter, device);
                            }
                        }

                        // Wildcard Adapters (DeviceKey = '*')
                        var wildCardAdapters = _configuration.Adapters.Where(o => o.DeviceKey == ShdrClientConfiguration.DeviceKeyWildcard);
                        if (!wildCardAdapters.IsNullOrEmpty())
                        {
                            foreach (var adapter in wildCardAdapters)
                            {
                                // Add Adapter for each Device (every device reads from the same adapter)
                                foreach (var device in devices) AddAdapter(adapter, device, true, device.Id);
                            }
                        }
                    }
                    else if (_configuration.AllowShdrDevice) // Prevent accidental generic Adapter creation
                    {
                        foreach (var adapter in _configuration.Adapters)
                        {
                            // Add a generic Adapter Client (no Device)
                            // Typically used if the Device Model is sent using SHDR
                            AddAdapter(adapter, null);
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


        private void AddAdapter(IShdrAdapterClientConfiguration configuration, IDevice device, bool initializeDataItems = true, string idSuffix = null)
        {
            if (configuration != null)
            {
                var adapterComponent = new ShdrAdapterComponent(configuration, idSuffix, device, device);

                // Add Adapter Component to Agent Device
                _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                // Create new SHDR Adapter Client to read from SHDR stream
                var adapterClient = new ShdrAdapterClient(configuration, _mtconnectAgent, device);
                adapterClient.Connected += _adapterLogger.AdapterConnected;
                adapterClient.Disconnected += _adapterLogger.AdapterDisconnected;
                adapterClient.ConnectionError += _adapterLogger.AdapterConnectionError;
                adapterClient.PingSent += _adapterLogger.AdapterPingSent;
                adapterClient.PongReceived += _adapterLogger.AdapterPongReceived;
                adapterClient.ProtocolReceived += _adapterShdrLogger.AdapterProtocolReceived;
                _adapterClients.Add(adapterClient);

                adapterClient.Start();
            }
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

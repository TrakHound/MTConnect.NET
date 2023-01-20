// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Adapters.Shdr;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent Application that supports SHDR Adapters and MQTT to publish to an MQTT Broker.
    /// Supports Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectShdrMqttRelayAgentApplication : MTConnectMqttRelayAgentApplication
    {
        private readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");
        private readonly Logger _adapterShdrLogger = LogManager.GetLogger("adapter-shdr-logger");

        private readonly List<ShdrAdapterClient> _adapters = new List<ShdrAdapterClient>();
        private IShdrMqttAgentApplicationConfiguration _configuration;


        public MTConnectShdrMqttRelayAgentApplication()
        {
            if (ConfigurationType == null) ConfigurationType = typeof(ShdrMqttAgentApplicationConfiguration);
        }


        protected override IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            var configuration = AgentConfiguration.Read<ShdrMqttAgentApplicationConfiguration>(configurationPath);
            base.OnAgentConfigurationUpdated(configuration);
            _configuration = configuration;
            return _configuration;
        }

        protected override void OnAgentConfigurationWatcherInitialize(IAgentApplicationConfiguration configuration)
        {
            _agentConfigurationWatcher = new AgentConfigurationFileWatcher<ShdrMqttAgentApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }

        protected override void OnAgentConfigurationUpdated(AgentConfiguration configuration)
        {
            _configuration = configuration as ShdrMqttAgentApplicationConfiguration;
        }


        protected override void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            // Add Adapter Clients
            if (_configuration != null && !_configuration.Adapters.IsNullOrEmpty())
            {
                if (!devices.IsNullOrEmpty())
                {
                    // Device Specific Adapters (DeviceKey specified)
                    var deviceAdapters = _configuration.Adapters.Where(o => o.DeviceKey != ShdrClientConfiguration.DeviceKeyWildcard);
                    if (!deviceAdapters.IsNullOrEmpty())
                    {
                        foreach (var adapter in deviceAdapters)
                        {
                            // Find Device matching DeviceKey
                            var device = devices?.FirstOrDefault(o => o.Uuid == adapter.DeviceKey || o.Name == adapter.DeviceKey);
                            if (device != null) AddAdapter(adapter, device, initializeDataItems);
                        }
                    }

                    // Wildcard Adapters (DeviceKey = '*')
                    var wildCardAdapters = _configuration.Adapters.Where(o => o.DeviceKey == ShdrClientConfiguration.DeviceKeyWildcard);
                    if (!wildCardAdapters.IsNullOrEmpty())
                    {
                        foreach (var adapter in wildCardAdapters)
                        {
                            // Add Adapter for each Device (every device reads from the same adapter)
                            foreach (var device in devices) AddAdapter(adapter, device, initializeDataItems, device.Id);
                        }
                    }
                }
                else if (_configuration.AllowShdrDevice) // Prevent accidental generic Adapter creation
                {
                    foreach (var adapter in _configuration.Adapters)
                    {
                        // Add a generic Adapter Client (no Device)
                        // Typically used if the Device Model is sent using SHDR
                        AddAdapter(adapter, null, initializeDataItems);
                    }
                }
            }

            base.OnStartAgentBeforeLoad(devices, initializeDataItems);
        }

        protected override void OnStopAgent()
        {
            // Stop Adapter Clients
            if (!_adapters.IsNullOrEmpty())
            {
                foreach (var adapter in _adapters)
                {
                    if (_verboseLogging)
                    {
                        adapter.Connected -= AdapterConnected;
                        adapter.Disconnected -= AdapterDisconnected;
                        adapter.ConnectionError -= AdapterConnectionError;
                        adapter.Listening -= AdapterListening;
                        adapter.PingSent -= AdapterPingSent;
                        adapter.PongReceived -= AdapterPongReceived;
                        adapter.ProtocolReceived -= AdapterProtocolReceived;
                    }

                    adapter.Stop();
                }
            }

            _adapters.Clear();

            base.OnStopAgent();
        }


        #region "Adapters"

        private void AddAdapter(IShdrAdapterConfiguration configuration, IDevice device, bool initializeDataItems = true, string idSuffix = null)
        {
            if (configuration != null)
            {
                var adapterComponent = new ShdrAdapterComponent(configuration, idSuffix, device, device);

                // Add Adapter Component to Agent Device
                Agent.Agent.AddAdapterComponent(adapterComponent);

                if (!adapterComponent.DataItems.IsNullOrEmpty())
                {
                    // Initialize Adapter URI Observation
                    var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                    if (adapterUriDataItem != null && initializeDataItems)
                    {
                        Agent.AddObservation(Agent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                    }
                }

                // Create new SHDR Adapter Client to read from SHDR stream
                var adapterClient = new ShdrAdapterClient(configuration, Agent, device, idSuffix);
                _adapters.Add(adapterClient);

                if (_verboseLogging)
                {
                    adapterClient.Connected += AdapterConnected;
                    adapterClient.Disconnected += AdapterDisconnected;
                    adapterClient.ConnectionError += AdapterConnectionError;
                    adapterClient.Listening += AdapterListening;
                    adapterClient.PingSent += AdapterPingSent;
                    adapterClient.PongReceived += AdapterPongReceived;
                    adapterClient.ProtocolReceived += AdapterProtocolReceived;
                }

                // Start the Adapter Client
                adapterClient.Start();
            }
        }

        #endregion

        #region "Logging"

        private void AdapterConnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.ESTABLISHED);

            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterDisconnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.CLOSED);

            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterConnectionError(object sender, Exception exception)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + exception.Message);
        }

        private void AdapterListening(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.LISTEN);

            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterPingSent(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterPongReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterProtocolReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterShdrLogger.Trace($"[Adapter-SHDR] : ID = " + adapterClient.Id + " : " + message);
        }

        #endregion
    }
}
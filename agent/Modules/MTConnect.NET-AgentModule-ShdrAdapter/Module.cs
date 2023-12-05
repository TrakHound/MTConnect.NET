// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Adapters;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "shdr-adapter";

        private readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");
        private readonly Logger _adapterShdrLogger = LogManager.GetLogger("adapter-shdr-logger");
        private readonly ShdrAdapterClientConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly List<ShdrAdapterClient> _adapters = new List<ShdrAdapterClient>();


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<ShdrAdapterClientConfiguration>(configuration);
        }


        protected override void OnStartAfterLoad()
        {
            if (_configuration != null)
            {
                var devices = _mtconnectAgent.GetDevices()?.Where(o => o.Type != Devices.Agent.TypeId);
                if (!devices.IsNullOrEmpty())
                {
                    if (_configuration.DeviceKey != ShdrClientConfiguration.DeviceKeyWildcard)
                    {
                        // Find Device matching DeviceKey
                        var device = devices.FirstOrDefault(o => o.Uuid == _configuration.DeviceKey || o.Name == _configuration.DeviceKey);
                        if (device != null) AddAdapter(_configuration, device);
                        //if (device != null) AddAdapter(_configuration, device, initializeDataItems);
                    }
                    else
                    {
                        // Add Adapter for each Device (every device reads from the same adapter)
                        foreach (var device in devices) AddAdapter(_configuration, device, true, device.Id);
                        //foreach (var device in devices) AddAdapter(adapter, device, initializeDataItems, device.Id);
                    }
                }
                //else if (_configuration.AllowShdrDevice) // Prevent accidental generic Adapter creation
                else
                {
                    // Add a generic Adapter Client (no Device)
                    // Typically used if the Device Model is sent using SHDR
                    AddAdapter(_configuration, null);
                }
            }
        }

        protected override void OnStop()
        {
            // Stop Adapter Clients
            if (!_adapters.IsNullOrEmpty())
            {
                foreach (var adapter in _adapters)
                {
                    adapter.Connected -= AdapterConnected;
                    adapter.Disconnected -= AdapterDisconnected;
                    adapter.ConnectionError -= AdapterConnectionError;
                    adapter.Listening -= AdapterListening;
                    adapter.PingSent -= AdapterPingSent;
                    adapter.PongReceived -= AdapterPongReceived;
                    adapter.ProtocolReceived -= AdapterProtocolReceived;

                    adapter.Stop();
                }
            }

            _adapters.Clear();
        }


        private void AddAdapter(IShdrAdapterClientConfiguration configuration, IDevice device, bool initializeDataItems = true, string idSuffix = null)
        {
            if (configuration != null)
            {
                var adapterComponent = new ShdrAdapterComponent(configuration, idSuffix, device, device);

                // Add Adapter Component to Agent Device
                _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                if (!adapterComponent.DataItems.IsNullOrEmpty())
                {
                    // Initialize Adapter URI Observation
                    var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                    if (adapterUriDataItem != null && initializeDataItems)
                    {
                        _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                    }
                }

                // Create new SHDR Adapter Client to read from SHDR stream
                var adapterClient = new ShdrAdapterClient(configuration, _mtconnectAgent, device, idSuffix);
                _adapters.Add(adapterClient);

                adapterClient.Connected += AdapterConnected;
                adapterClient.Disconnected += AdapterDisconnected;
                adapterClient.ConnectionError += AdapterConnectionError;
                adapterClient.Listening += AdapterListening;
                adapterClient.PingSent += AdapterPingSent;
                adapterClient.PongReceived += AdapterPongReceived;
                adapterClient.ProtocolReceived += AdapterProtocolReceived;

                // Start the Adapter Client
                adapterClient.Start();
            }
        }


        private void AdapterConnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.ESTABLISHED);

            _adapterLogger.Info($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterDisconnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.CLOSED);

            _adapterLogger.Info($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterConnectionError(object sender, Exception exception)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + exception.Message);
        }

        private void AdapterListening(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.LISTEN);

            _adapterLogger.Info($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterPingSent(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterPongReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterProtocolReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterShdrLogger.Trace($"[SHDR-Adapter] : ID = " + adapterClient.Id + " : " + message);
        }
    }
}
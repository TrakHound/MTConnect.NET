// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent Application that reads from other MTConnect Agents and acts as a Gateway or Pass-through broker
    /// </summary>
    public class MTConnectMqttRelayAgentGatewayApplication : MTConnectMqttRelayAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-MQTT-Relay-Gateway";
        private const string DefaultServiceDisplayName = "MTConnect MQTT Relay Gateway Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using MQTT to provide access to device information using the MTConnect Standard";
        private const int ClientInformationUpdateInterval = 5000;

        private readonly List<MTConnectHttpClient> _clients = new List<MTConnectHttpClient>();
        private readonly Logger _clientLogger = LogManager.GetLogger("client-logger");
        private readonly Dictionary<string, MTConnectClientInformation> _clientInformations = new Dictionary<string, MTConnectClientInformation>();
        private readonly object _lock = new object();

        private System.Timers.Timer _clientInformationTimer;
        private MqttAgentGatewayApplicationConfiguration _configuration;


        public MTConnectMqttRelayAgentGatewayApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;

            if (ConfigurationType == null) ConfigurationType = typeof(MqttAgentGatewayApplicationConfiguration);
        }


        protected override IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            var configuration = AgentConfiguration.Read<MqttAgentGatewayApplicationConfiguration>(configurationPath);
            base.OnAgentConfigurationUpdated(configuration);
            _configuration = configuration;
            return _configuration;
        }

        protected override void OnAgentConfigurationWatcherInitialize(IAgentApplicationConfiguration configuration)
        {
            _agentConfigurationWatcher = new AgentConfigurationFileWatcher<MqttAgentGatewayApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }

        protected override void OnAgentConfigurationUpdated(AgentConfiguration configuration)
        {
            _configuration = configuration as MqttAgentGatewayApplicationConfiguration;
        }


        protected override void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            // Add Agent Clients
            if (!_configuration.Clients.IsNullOrEmpty())
            {
                foreach (var clientConfiguration in _configuration.Clients)
                {
                    if (!string.IsNullOrEmpty(clientConfiguration.Address))
                    {
                        var baseUri = HttpClientConfiguration.CreateBaseUri(clientConfiguration);
                        var adapterComponent = new HttpAdapterComponent(clientConfiguration);

                        // Add Adapter Component to Agent Device
                        Agent.Agent.AddAdapterComponent(adapterComponent);

                        if (!adapterComponent.DataItems.IsNullOrEmpty())
                        {
                            // Initialize Adapter URI Observation
                            var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                            if (adapterUriDataItem != null)
                            {
                                Agent.AddObservation(Agent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                            }
                        }


                        var agentClient = new MTConnectHttpClient(baseUri, clientConfiguration.DeviceKey);
                        agentClient.Id = clientConfiguration.Id;
                        agentClient.Interval = clientConfiguration.Interval;
                        agentClient.Heartbeat = clientConfiguration.Heartbeat;
                        agentClient.CurrentOnly = clientConfiguration.CurrentOnly;
                        agentClient.UseStreaming = clientConfiguration.UseStreaming;

                        // Subscribe to the Event handlers to receive the MTConnect documents
                        agentClient.OnClientStarted += (s, e) => AgentClientStarted(agentClient);
                        agentClient.OnClientStopped += (s, e) => AgentClientStopped(agentClient);
                        agentClient.OnStreamStarted += (s, query) => AgentClientStreamStarted(agentClient, query);
                        agentClient.OnStreamStopped += (s, e) => AgentClientStreamStopped(agentClient);
                        agentClient.OnProbeReceived += (s, doc) => DevicesDocumentReceived(agentClient, doc);
                        agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(agentClient, doc);
                        agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(agentClient, doc);
                        agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(agentClient, doc);

                        _clients.Add(agentClient);
                    }
                }
            }

            base.OnStartAgentBeforeLoad(devices, initializeDataItems);
        }

        protected override void OnStartAgentAfterLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            if (!_clients.IsNullOrEmpty())
            {
                foreach (var client in _clients)
                {
                    var clientInformation = MTConnectClientInformation.Read(client.Device);
                    if (clientInformation != null)
                    {
                        lock (_lock)
                        {
                            _clientInformations.Remove(client.Device);
                            _clientInformations.Add(client.Device, clientInformation);
                        }

                        client.StartFromSequence(clientInformation.InstanceId, clientInformation.LastSequence);
                    }
                    else
                    {
                        lock (_lock)
                        {
                            _clientInformations.Remove(client.Device);
                            _clientInformations.Add(client.Device, new MTConnectClientInformation(client.Device));
                        }

                        client.StartFromBuffer();
                    }
                }
            }

            // Start Client Information Updates
            UpdateClientInformations();
            StartClientInformation();

            base.OnStartAgentAfterLoad(devices, initializeDataItems);
        }

        protected override void OnStopAgent()
        {
            // Stop Agent Clients
            if (!_clients.IsNullOrEmpty())
            {
                foreach (var client in _clients) client.Stop();
            }

            if (_clientInformationTimer != null) _clientInformationTimer.Dispose();

            UpdateClientInformations();

            _clientInformations.Clear();
            _clients.Clear();

            base.OnStopAgent();
        }


        #region "Client Event Handlers"

        private void AgentClientStarted(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.LISTEN);

                _clientLogger.Info($"[Http-Adapter] : ID = {client.Id} : Client Started : {client.Authority}");
            }
        }

        private void AgentClientStopped(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.CLOSED);

                _clientLogger.Info($"[Http-Adapter] : ID = {client.Id} : Client Stopped : {client.Authority}");
            }
        }

        private void AgentClientStreamStarted(MTConnectHttpClient client, string query)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.ESTABLISHED);

                _clientLogger.Info($"[Http-Adapter] : ID = {client.Id} : Client Stream Started : {query}");
            }
        }

        private void AgentClientStreamStopped(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.CLOSED);
                Agent.AddObservation(Agent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.LISTEN);

                _clientLogger.Info($"[Http-Adapter] : ID = {client.Id} : Client Stream Stopped");
            }
        }

        private void DevicesDocumentReceived(MTConnectHttpClient client, IDevicesResponseDocument document)
        {
            if (client != null && document != null && !document.Devices.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[Http-Adapter] : ID = {client.Id} : MTConnectDevices Received : " + document.Header.CreationTime);

                foreach (var device in document.Devices)
                {
                    Agent.AddDevice(device);
                }
            }
        }

        private void StreamsDocumentReceived(MTConnectHttpClient client, IStreamsResponseDocument document)
        {
            if (client != null && document != null && !document.Streams.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[Http-Adapter] : ID = {client.Id} : MTConnectStreams Received : " + document.Header.CreationTime);

                foreach (var stream in document.Streams)
                {
                    var observations = stream.Observations;
                    if (!observations.IsNullOrEmpty())
                    {
                        foreach (var observation in observations)
                        {
                            var input = new ObservationInput();
                            input.DeviceKey = stream.Uuid;
                            input.DataItemKey = observation.DataItemId;
                            input.Timestamp = observation.Timestamp.ToUnixTime();
                            input.Values = observation.Values;

                            Agent.AddObservation(stream.Uuid, input);
                        }
                    }
                }
            }
        }

        private void AssetsDocumentReceived(MTConnectHttpClient client, IAssetsResponseDocument document)
        {
            if (client != null && document != null && !document.Assets.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[Http-Adapter] : ID = {client.Id} : MTConnectAssets Received : " + document.Header.CreationTime);

                foreach (var asset in document.Assets)
                {
                    Agent.AddAsset(asset.DeviceUuid, asset);
                }
            }
        }

        #endregion

        #region "Client Information"

        private void StartClientInformation()
        {
            _clientInformationTimer = new System.Timers.Timer();
            _clientInformationTimer.Interval = ClientInformationUpdateInterval;
            _clientInformationTimer.Elapsed += (s, e) =>
            {
                UpdateClientInformations();
            };
            _clientInformationTimer.Start();
        }

        private void UpdateClientInformations()
        {
            if (!_clients.IsNullOrEmpty())
            {
                foreach (var client in _clients)
                {
                    lock (_lock)
                    {
                        _clientInformations.TryGetValue(client.Device, out var clientInformation);
                        if (clientInformation == null) clientInformation = new MTConnectClientInformation(client.Device);

                        if (client.LastInstanceId != clientInformation.InstanceId || client.LastSequence != clientInformation.LastSequence)
                        {
                            clientInformation.InstanceId = client.LastInstanceId;
                            clientInformation.LastSequence = client.LastSequence;

                            clientInformation.Save();
                        }
                    }
                }
            }
        }

        #endregion

    }
}
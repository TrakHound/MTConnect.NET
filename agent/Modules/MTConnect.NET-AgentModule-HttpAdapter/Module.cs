// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using MTConnect.Input;
using MTConnect.Streams;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "http-client";
        private const string ModuleId = "HTTP Adapter";

        private readonly HttpAdapterModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly List<MTConnectHttpClient> _clients = new List<MTConnectHttpClient>();
        private readonly Dictionary<string, MTConnectClientInformation> _clientInformations = new Dictionary<string, MTConnectClientInformation>();
        private System.Timers.Timer _clientInformationTimer;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<HttpAdapterModuleConfiguration>(configuration);
        }


        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            if (_configuration != null && !string.IsNullOrEmpty(_configuration.Address))
            {
                var baseUri = HttpClientConfiguration.CreateBaseUri(_configuration);
                var adapterComponent = new HttpAdapterComponent(_configuration);

                // Add Adapter Component to Agent Device
                if (_mtconnectAgent.Agent != null)
                {
                    _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);
                }

                if (!adapterComponent.DataItems.IsNullOrEmpty())
                {
                    // Initialize Adapter URI Observation
                    var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                    if (adapterUriDataItem != null)
                    {
                        _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                    }
                }

                if (!_configuration.Devices.IsNullOrEmpty())
                {
                    foreach (var deviceMapping in _configuration.Devices)
                    {
                        var inputDevice = deviceMapping.Key;
                        var outputDevice = !string.IsNullOrEmpty(deviceMapping.Value.Uuid) ? deviceMapping.Value.Uuid : deviceMapping.Value.Name;

                        var client = new MTConnectHttpClient(baseUri, inputDevice);
                        client.Id = _configuration.Id;
                        client.Interval = _configuration.Interval;
                        client.Heartbeat = _configuration.Heartbeat;
                        client.CurrentOnly = _configuration.CurrentOnly;
                        client.UseStreaming = _configuration.UseStreaming;

                        // Subscribe to the Events to receive the MTConnect documents
                        client.ClientStarted += (s, e) => AgentClientStarted(client);
                        client.ClientStopped += (s, e) => AgentClientStopped(client);
                        client.StreamStarted += (s, query) => AgentClientStreamStarted(client, query);
                        client.StreamStopped += (s, e) => AgentClientStreamStopped(client);
                        client.CurrentReceived += (s, doc) => StreamsDocumentReceived(client, doc, outputDevice);
                        client.SampleReceived += (s, doc) => StreamsDocumentReceived(client, doc, outputDevice);
                        client.AssetsReceived += (s, doc) => AssetsDocumentReceived(client, doc, outputDevice);

                        if (_mtconnectAgent.GetDevice(client.Device) == null)
                        {
                            client.ProbeReceived += (s, doc) => DevicesDocumentReceived(client, doc, deviceMapping.Value);
                        }

                        client.Start();

                        _clients.Add(client);
                    }
                }
                else
                {
                    var client = new MTConnectHttpClient(baseUri);
                    client.Id = _configuration.Id;
                    client.Interval = _configuration.Interval;
                    client.Heartbeat = _configuration.Heartbeat;
                    client.CurrentOnly = _configuration.CurrentOnly;
                    client.UseStreaming = _configuration.UseStreaming;

                    // Subscribe to the Events to receive the MTConnect documents
                    client.ClientStarted += (s, e) => AgentClientStarted(client);
                    client.ClientStopped += (s, e) => AgentClientStopped(client);
                    client.StreamStarted += (s, query) => AgentClientStreamStarted(client, query);
                    client.StreamStopped += (s, e) => AgentClientStreamStopped(client);
                    client.CurrentReceived += (s, doc) => StreamsDocumentReceived(client, doc);
                    client.SampleReceived += (s, doc) => StreamsDocumentReceived(client, doc);
                    client.AssetsReceived += (s, doc) => AssetsDocumentReceived(client, doc);

                    if (_mtconnectAgent.GetDevice(client.Device) == null)
                    {
                        client.ProbeReceived += (s, doc) => DevicesDocumentReceived(client, doc);
                    }

                    client.Start();

                    _clients.Add(client);
                }
            }
        }

        protected override void OnStop()
        {
            if (!_clients.IsNullOrEmpty()) foreach (var client in _clients) client.Stop();
        }


        #region "Client Event Handlers"

        private void AgentClientStarted(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.LISTEN);

                Log(Logging.MTConnectLogLevel.Information, $"Client ID = {client.Id} : Client Started : {client.Authority}");
            }
        }

        private void AgentClientStopped(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.CLOSED);

                Log(Logging.MTConnectLogLevel.Information, $"Client ID = {client.Id} : Client Stopped : {client.Authority}");
            }
        }

        private void AgentClientStreamStarted(MTConnectHttpClient client, string query)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.ESTABLISHED);

                Log(Logging.MTConnectLogLevel.Debug, $"Client ID = {client.Id} : Client Stream Started : {query}");
            }
        }

        private void AgentClientStreamStopped(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.CLOSED);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.LISTEN);

                Log(Logging.MTConnectLogLevel.Debug, $"Client ID = {client.Id} : Client Stream Stopped");
            }
        }

        private void DevicesDocumentReceived(MTConnectHttpClient client, IDevicesResponseDocument document, DeviceMappingConfiguration deviceMapping = null)
        {
            if (client != null && document != null && !document.Devices.IsNullOrEmpty())
            {
                Log(Logging.MTConnectLogLevel.Debug, $"Client ID = {client.Id} : MTConnectDevices Received : " + document.Header.CreationTime);

                foreach (var device in document.Devices)
                {
                    var outputDevice = Device.Process(device, device.MTConnectVersion);

                    if (deviceMapping != null)
                    {
                        if (!string.IsNullOrEmpty(deviceMapping.Uuid)) outputDevice.Uuid = deviceMapping.Uuid;
                        if (!string.IsNullOrEmpty(deviceMapping.Id)) outputDevice.Id = deviceMapping.Id;
                        if (!string.IsNullOrEmpty(deviceMapping.Name)) outputDevice.Name = deviceMapping.Name;
                    }

                    _mtconnectAgent.AddDevice(outputDevice);
                }
            }
        }

        private void StreamsDocumentReceived(MTConnectHttpClient client, IStreamsResponseDocument document, string outputDeviceKey = null)
        {
            if (client != null && document != null && !document.Streams.IsNullOrEmpty())
            {
                Log(Logging.MTConnectLogLevel.Debug, $"Client ID = {client.Id} : MTConnectStreams Received : " + document.Header.CreationTime);

                foreach (var stream in document.Streams)
                {
                    var deviceKey = !string.IsNullOrEmpty(outputDeviceKey) ? outputDeviceKey : stream.Uuid;

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

                            _mtconnectAgent.AddObservation(deviceKey, input);
                        }
                    }
                }
            }
        }

        private void AssetsDocumentReceived(MTConnectHttpClient client, IAssetsResponseDocument document, string outputDeviceKey = null)
        {
            if (client != null && document != null && !document.Assets.IsNullOrEmpty())
            {
                Log(Logging.MTConnectLogLevel.Debug, $"Client ID = {client.Id} : MTConnectAssets Received : " + document.Header.CreationTime);

                foreach (var asset in document.Assets)
                {
                    var deviceKey = !string.IsNullOrEmpty(outputDeviceKey) ? outputDeviceKey : asset.DeviceUuid;

                    _mtconnectAgent.AddAsset(deviceKey, asset);
                }
            }
        }

        #endregion

        #region "Client Information"

        private void StartClientInformation()
        {
            //_clientInformationTimer = new System.Timers.Timer();
            //_clientInformationTimer.Interval = ClientInformationUpdateInterval;
            //_clientInformationTimer.Elapsed += (s, e) =>
            //{
            //    UpdateClientInformations();
            //};
            //_clientInformationTimer.Start();
        }

        private void UpdateClientInformations()
        {
            //if (!_clients.IsNullOrEmpty())
            //{
            //    lock (_lock)
            //    {
            //        _clientInformations.TryGetValue(client.Device, out var clientInformation);
            //        if (clientInformation == null) clientInformation = new MTConnectClientInformation(client.Device);

            //        if (client.LastInstanceId != clientInformation.InstanceId || client.LastSequence != clientInformation.LastSequence)
            //        {
            //            clientInformation.InstanceId = client.LastInstanceId;
            //            clientInformation.LastSequence = client.LastSequence;

            //            clientInformation.Save();
            //        }
            //    }
            //}
        }

        #endregion

    }
}
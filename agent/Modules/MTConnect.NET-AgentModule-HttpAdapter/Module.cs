// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "http-client";

        private readonly Logger _clientLogger = LogManager.GetLogger("http-adapter-logger");
        private readonly HttpClientConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly Dictionary<string, MTConnectClientInformation> _clientInformations = new Dictionary<string, MTConnectClientInformation>();
        private MTConnectHttpClient _agentClient;
        private System.Timers.Timer _clientInformationTimer;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<HttpClientConfiguration>(configuration);
        }


        protected override void OnStartAfterLoad()
        {
            if (_configuration != null && !string.IsNullOrEmpty(_configuration.Address))
            {
                var baseUri = HttpClientConfiguration.CreateBaseUri(_configuration);
                var adapterComponent = new HttpAdapterComponent(_configuration);

                // Add Adapter Component to Agent Device
                _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                if (!adapterComponent.DataItems.IsNullOrEmpty())
                {
                    // Initialize Adapter URI Observation
                    var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                    if (adapterUriDataItem != null)
                    {
                        _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                    }
                }


                _agentClient = new MTConnectHttpClient(baseUri, _configuration.DeviceKey);
                _agentClient.Id = _configuration.Id;
                _agentClient.Interval = _configuration.Interval;
                _agentClient.Heartbeat = _configuration.Heartbeat;
                _agentClient.CurrentOnly = _configuration.CurrentOnly;
                _agentClient.UseStreaming = _configuration.UseStreaming;

                // Subscribe to the Events to receive the MTConnect documents
                _agentClient.ClientStarted += (s, e) => AgentClientStarted(_agentClient);
                _agentClient.ClientStopped += (s, e) => AgentClientStopped(_agentClient);
                _agentClient.StreamStarted += (s, query) => AgentClientStreamStarted(_agentClient, query);
                _agentClient.StreamStopped += (s, e) => AgentClientStreamStopped(_agentClient);
                _agentClient.CurrentReceived += (s, doc) => StreamsDocumentReceived(_agentClient, doc);
                _agentClient.SampleReceived += (s, doc) => StreamsDocumentReceived(_agentClient, doc);
                _agentClient.AssetsReceived += (s, doc) => AssetsDocumentReceived(_agentClient, doc);

                if (_mtconnectAgent.GetDevice(_agentClient.Device) == null)
                {
                    _agentClient.ProbeReceived += (s, doc) => DevicesDocumentReceived(_agentClient, doc);
                }

                _agentClient.Start();
            }
        }

        protected override void OnStop()
        {
            if (_agentClient != null) _agentClient.Stop();
        }


        #region "Client Event Handlers"

        private void AgentClientStarted(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.LISTEN);

                _clientLogger.Info($"[HTTP-Adapter] : ID = {client.Id} : Client Started : {client.Authority}");
            }
        }

        private void AgentClientStopped(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.CLOSED);

                _clientLogger.Info($"[HTTP-Adapter] : ID = {client.Id} : Client Stopped : {client.Authority}");
            }
        }

        private void AgentClientStreamStarted(MTConnectHttpClient client, string query)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.ESTABLISHED);

                _clientLogger.Info($"[HTTP-Adapter] : ID = {client.Id} : Client Stream Started : {query}");
            }
        }

        private void AgentClientStreamStopped(MTConnectHttpClient client)
        {
            if (client != null)
            {
                var dataItemId = DataItem.CreateId(client.Id, ConnectionStatusDataItem.NameId);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.CLOSED);
                _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.ConnectionStatus.LISTEN);

                _clientLogger.Info($"[HTTP-Adapter] : ID = {client.Id} : Client Stream Stopped");
            }
        }

        private void DevicesDocumentReceived(MTConnectHttpClient client, IDevicesResponseDocument document)
        {
            if (client != null && document != null && !document.Devices.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[HTTP-Adapter] : ID = {client.Id} : MTConnectDevices Received : " + document.Header.CreationTime);

                foreach (var device in document.Devices)
                {
                    _mtconnectAgent.AddDevice(device);
                }
            }
        }

        private void StreamsDocumentReceived(MTConnectHttpClient client, IStreamsResponseDocument document)
        {
            if (client != null && document != null && !document.Streams.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[HTTP-Adapter] : ID = {client.Id} : MTConnectStreams Received : " + document.Header.CreationTime);

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

                            _mtconnectAgent.AddObservation(stream.Uuid, input);
                        }
                    }
                }
            }
        }

        private void AssetsDocumentReceived(MTConnectHttpClient client, IAssetsResponseDocument document)
        {
            if (client != null && document != null && !document.Assets.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[HTTP-Adapter] : ID = {client.Id} : MTConnectAssets Received : " + document.Header.CreationTime);

                foreach (var asset in document.Assets)
                {
                    _mtconnectAgent.AddAsset(asset.DeviceUuid, asset);
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
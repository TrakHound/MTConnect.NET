// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MTConnect.Agents;
using MTConnect.Applications.Loggers;
using MTConnect.Assets;
using MTConnect.Clients.Rest;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Errors;
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
        private readonly AgentGatewayConfiguration _configuration;
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly ILogger<AgentService> _logger;
        private readonly AgentLogger _agentLogger;
        private readonly List<MTConnectClient> _clients = new List<MTConnectClient>();
        private System.Timers.Timer _metricsTimer;


        public AgentService(
            AgentGatewayConfiguration configuration,
            IMTConnectAgent mtconnectAgent, 
            AgentLogger agentLogger,
            AgentValidationLogger agentValidationLogger,
            ILogger<AgentService> logger
            )
        {
            _configuration = configuration;
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
                if (!_configuration.Clients.IsNullOrEmpty())
                {
                    foreach (var clientConfiguration in _configuration.Clients)
                    {
                        if (!string.IsNullOrEmpty(clientConfiguration.Address))
                        {
                            //string baseUrl = null;
                            //var clientAddress = clientConfiguration.Address;
                            //var clientPort = clientConfiguration.Port;

                            //if (clientConfiguration.UseSSL) clientAddress = clientAddress.Replace("https://", "");
                            //else clientAddress = clientAddress.Replace("http://", "");

                            //// Create the MTConnect Agent Base URL
                            //if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", Url.AddPort(clientAddress, clientPort));
                            //else baseUrl = string.Format("http://{0}", Url.AddPort(clientAddress, clientPort));

                            var baseUri = HttpClientConfiguration.CreateBaseUri(clientConfiguration);

                            var adapterComponent = new HttpAdapterComponent(clientConfiguration);

                            // Add Adapter Component to Agent Device
                            _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                            if (!adapterComponent.DataItems.IsNullOrEmpty())
                            {
                                //// Initialize Connection Status Observation
                                //var connectionStatusDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == ConnectionStatusDataItem.TypeId);
                                //if (connectionStatusDataItem != null)
                                //{
                                //    _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, connectionStatusDataItem.Id, Observations.Events.Values.ConnectionStatus.LISTEN);
                                //}

                                // Initialize Adapter URI Observation
                                var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                                if (adapterUriDataItem != null)
                                {
                                    _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                                }
                            }


                            var agentClient = new MTConnectClient(baseUri, clientConfiguration.DeviceKey);
                            agentClient.Interval = clientConfiguration.Interval;
                            agentClient.Heartbeat = clientConfiguration.Heartbeat;
                            _clients.Add(agentClient);

                            // Subscribe to the Event handlers to receive status events
                            agentClient.OnClientStarting += (s, e) => ClientStarting(((MTConnectClient)s).Authority);
                            agentClient.OnClientStarted += (s, e) => ClientStarted(((MTConnectClient)s).Authority);
                            agentClient.OnClientStopping += (s, e) => ClientStopping(((MTConnectClient)s).Authority);
                            agentClient.OnClientStopped += (s, e) => ClientStopped(((MTConnectClient)s).Authority);
                            agentClient.OnStreamStarting += (s, streamUrl) => StreamStarting(streamUrl);
                            agentClient.OnStreamStarted += (s, streamUrl) => StreamStarted(streamUrl);
                            agentClient.OnStreamStopping += (s, streamUrl) => StreamStopping(streamUrl);
                            agentClient.OnStreamStopped += (s, streamUrl) => StreamStopped(streamUrl);

                            // Subscribe to the Event handlers to receive the MTConnect documents
                            agentClient.OnProbeReceived += (s, doc) => DevicesDocumentReceived(doc);
                            agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(doc);
                            agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(doc);
                            agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(doc);

                            agentClient.Start();
                        }

                        //if (!string.IsNullOrEmpty(clientConfiguration.Address))
                        //{
                        //    string baseUrl = null;
                        //    var address = clientConfiguration.Address;
                        //    var port = clientConfiguration.Port;

                        //    if (clientConfiguration.UseSSL) address = address.Replace("https://", "");
                        //    else address = address.Replace("http://", "");

                        //    // Create the MTConnect Agent Base URL
                        //    if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", AddPort(address, port));
                        //    else baseUrl = string.Format("http://{0}", AddPort(address, port));

                        //    var agentClient = new MTConnectClient(baseUrl, clientConfiguration.DeviceKey);
                        //    agentClient.Interval = clientConfiguration.Interval;
                        //    agentClient.Heartbeat = clientConfiguration.Heartbeat;

                        //    // Subscribe to the Event handlers to receive status events
                        //    agentClient.OnClientStarting += (s, e) => ClientStarting(((MTConnectClient)s).Authority);
                        //    agentClient.OnClientStarted += (s, e) => ClientStarted(((MTConnectClient)s).Authority);
                        //    agentClient.OnClientStopping += (s, e) => ClientStopping(((MTConnectClient)s).Authority);
                        //    agentClient.OnClientStopped += (s, e) => ClientStopped(((MTConnectClient)s).Authority);
                        //    agentClient.OnStreamStarting += (s, streamUrl) => StreamStarting(streamUrl);
                        //    agentClient.OnStreamStarted += (s, streamUrl) => StreamStarted(streamUrl);
                        //    agentClient.OnStreamStopping += (s, streamUrl) => StreamStopping(streamUrl);
                        //    agentClient.OnStreamStopped += (s, streamUrl) => StreamStopped(streamUrl);

                        //    // Subscribe to the Event handlers to receive the MTConnect documents
                        //    agentClient.OnProbeReceived += (s, doc) => DevicesDocumentReceived(doc);
                        //    agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(doc);
                        //    agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(doc);
                        //    agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(doc);

                        //    // Subscribe to the Error Handlers
                        //    agentClient.OnMTConnectError += (s, doc) => AgentClientError(doc);
                        //    agentClient.OnConnectionError += (s, ex) => AgentClientConnectionError(ex);
                        //    agentClient.OnInternalError += (s, ex) => AgentClientInternalError(ex);

                        //    // Add to local list (to be able to stop it later)
                        //    _clients.Add(agentClient);

                        //    // Start the Client
                        //    agentClient.Start();
                        //}
                    }
                }
            }

            StartMetrics();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop all of the MTConnectClients
            if (!_clients.IsNullOrEmpty())
            {
                foreach (var client in _clients)
                {
                    if (client != null) client.Stop();
                }
            }

            // Stop the Metrics Timer
            if (_metricsTimer != null) _metricsTimer.Dispose();
        }


        private void AgentClientError(IErrorResponseDocument errorDocument)
        {
            _logger?.LogError($"MTConnect Connection : MTConnect Error Received from MTConnect Agent");
        }

        private void AgentClientConnectionError(Exception ex)
        {
            _logger?.LogError($"MTConnect Connection : Error Connecting to MTConnect Agent : {ex.Message}");
        }

        private void AgentClientInternalError(Exception ex)
        {
            _logger?.LogDebug($"MTConnect Connection : Error in MTConnectClient : {ex.Message}");
        }


        private void ClientStarting(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Client Starting..");
        }

        private void ClientStarted(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Client Started");
        }

        private void ClientStopping(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Client Stopping..");
        }

        private void ClientStopped(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Client Stopped");
        }


        private void StreamStarting(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Stream Starting..");
        }

        private void StreamStarted(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Stream Started");
        }

        private void StreamStopping(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Stream Stopping..");
        }

        private void StreamStopped(string url)
        {
            _logger?.LogInformation($"MTConnect Connection : {url} : MTConnect Stream Stopped");
        }


        private void DevicesDocumentReceived(IDevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                foreach (var device in document.Devices)
                {
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

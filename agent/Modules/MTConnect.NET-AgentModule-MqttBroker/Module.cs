// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Certificates;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Logging;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-broker";
        private const string ModuleId = "MQTT Broker";

        private readonly MqttBrokerModuleConfiguration _configuration;
        private readonly MTConnectMqttDocumentServer _documentServer;
        private readonly MTConnectMqttEntityServer _entityServer;
        private MqttServer _mqttServer;
        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttBrokerModuleConfiguration>(configuration);

            switch (_configuration.TopicStructure)
            {
                case MqttTopicStructure.Document:

                    _documentServer = new MTConnectMqttDocumentServer(mtconnectAgent, _configuration);
                    _documentServer.ProbeReceived += ProbeReceived;
                    _documentServer.CurrentReceived += CurrentReceived;
                    _documentServer.SampleReceived += SampleReceived;
                    _documentServer.AssetReceived += AssetReceived;
                    break;


                case MqttTopicStructure.Entity:

                    _entityServer = new MTConnectMqttEntityServer(_configuration.TopicPrefix, _configuration.DocumentFormat);
                    Agent.DeviceAdded += AgentDeviceAdded;
                    Agent.ObservationAdded += AgentObservationAdded;
                    Agent.AssetAdded += AgentAssetAdded;
                    break;
            }
        }


        protected override void OnStartBeforeLoad(bool initializeDataItems)
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(StartAsync, _stop.Token);
            Task.Delay(_configuration.InitialDelay).Wait();
        }

        protected override void OnStop()
        {
            if (_mqttServer != null) _mqttServer.StopAsync();
        }


        private async Task StartAsync()
        {
            do
            {
                try
                {
                    try
                    {
                        var mqttServerOptions = new MqttServerOptions();

                        // Set the Timeout
                        mqttServerOptions.DefaultCommunicationTimeout = TimeSpan.FromMilliseconds(_configuration.Timeout);

                        // Get the IP Address (in case configuration specifies a Hostname)
                        IPAddress address = null;
                        if (!string.IsNullOrEmpty(_configuration.Server))
                        {
                            var hostEntry = Dns.GetHostEntry(_configuration.Server);
                            if (hostEntry != null && !hostEntry.AddressList.IsNullOrEmpty())
                            {
                                address = hostEntry.AddressList.FirstOrDefault(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                            }
                        }
                        else address = IPAddress.Any;


                        // Add Certificate & Private Key
                        if (_configuration.Tls != null)
                        {
                            mqttServerOptions.TlsEndpointOptions.IsEnabled = true;
                            mqttServerOptions.TlsEndpointOptions.BoundInterNetworkAddress = address;
                            mqttServerOptions.TlsEndpointOptions.Port = _configuration.Port;

                            var certificate = _configuration.Tls.GetCertificate();
                            if (certificate.Success && certificate.Certificate != null)
                            {
                                mqttServerOptions.TlsEndpointOptions.CertificateProvider = new X509CertificateProvider(certificate.Certificate);
                                mqttServerOptions.TlsEndpointOptions.SslProtocol = System.Security.Authentication.SslProtocols.Tls12;
                            }
                        }
                        else
                        {
                            mqttServerOptions.DefaultEndpointOptions.IsEnabled = true;
                            mqttServerOptions.DefaultEndpointOptions.BoundInterNetworkAddress = address;
                            mqttServerOptions.DefaultEndpointOptions.Port = _configuration.Port;
                        }


                        var mqttFactory = new MqttFactory();
                        _mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

                        _mqttServer.ClientConnectedAsync += (args) =>
                        {
                            Log(MTConnectLogLevel.Debug, $"MQTT Server : Client Connected : {args.ClientId} : {args.Endpoint}");
                            return Task.CompletedTask;
                        };
                        _mqttServer.ClientDisconnectedAsync += (args) =>
                        {
                            Log(MTConnectLogLevel.Debug, $"MQTT Server : Client Disconnected : {args.ClientId} : {args.Endpoint}");
                            return Task.CompletedTask;
                        };
                        _mqttServer.ValidatingConnectionAsync += (args) =>
                        {
                            Log(MTConnectLogLevel.Debug, $"MQTT Server : Validating Client Connection : {args.ClientId} : {args.Endpoint} : {args.ReasonString}");
                            return Task.CompletedTask;
                        };

                        // Start MQTT Server
                        await _mqttServer.StartAsync();

                        // Start Document Server (if configured)
                        if (_documentServer != null) _documentServer.Start();

                        // Initialize Entity Server (if configured)
                        if (_entityServer != null)
                        {
                            await PublishDevices();
                            await PublishCurrentObservations();
                            await PublishAssets();
                        }

                        Log(MTConnectLogLevel.Information, "MQTT Server Started..");

                        while (!_stop.Token.IsCancellationRequested && _mqttServer.IsStarted)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"MQTT Server Error : {ex.Message}");
                    }
                    finally
                    {
                        if (_documentServer != null) _documentServer.Stop();
                    }

                    Log(MTConnectLogLevel.Information, $"MQTT Server Stopped");

                    await Task.Delay(_configuration.RestartInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);
        }


        private async void ProbeReceived(IDevice device, IDevicesResponseDocument responseDocument)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.ProbeTopic}/{device.Uuid}";
                    formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    var injectMessage = new InjectedMqttApplicationMessage(message);

                    await _mqttServer.InjectApplicationMessage(injectMessage);
                }
            }
        }

        private async void CurrentReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.CurrentTopic}/{device.Uuid}";
                    formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    var injectMessage = new InjectedMqttApplicationMessage(message);

                    await _mqttServer.InjectApplicationMessage(injectMessage);
                }
            }
        }

        private async void SampleReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.SampleTopic}/{device.Uuid}";
                    formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    var injectMessage = new InjectedMqttApplicationMessage(message);

                    await _mqttServer.InjectApplicationMessage(injectMessage);
                }
            }
        }

        private async void AssetReceived(IDevice device, IAssetsResponseDocument responseDocument)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                foreach (var asset in responseDocument.Assets)
                {
                    var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, asset, x);
                    if (formatResult.Success)
                    {
                        var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.AssetTopic}/{device.Uuid}/{asset.AssetId}";
                        formatResult.Content.Seek(0, SeekOrigin.Begin);

                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                        messageBuilder.WithPayload(formatResult.Content);
                        var message = messageBuilder.Build();

                        var injectMessage = new InjectedMqttApplicationMessage(message);

                        await _mqttServer.InjectApplicationMessage(injectMessage);
                    }
                }
            }
        }


        private async Task PublishDevices()
        {
            if (_entityServer != null)
            {
                var devices = Agent.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    foreach (var device in devices)
                    {
                        await _entityServer.PublishDevice(_mqttServer, device);
                    }
                }
            }
        }

        private async Task PublishCurrentObservations()
        {
            if (_entityServer != null)
            {
                var observations = Agent.GetCurrentObservations();
                await PublishObservations(observations);
            }
        }

        private async Task PublishObservations(IEnumerable<IObservationOutput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var dataItemIds = observations.Select(o => o.DataItemId).Distinct();
                foreach (var dataItemId in dataItemIds)
                {
                    var dataItemObservations = observations.Where(o => o.DataItemId == dataItemId);
                    var dataItemObservation = dataItemObservations.FirstOrDefault();

                    if (dataItemObservation.Category == DataItemCategory.CONDITION)
                    {
                        // Conditions have multiple observations
                        var multipleObservations = new List<IObservation>();
                        foreach (var observation in dataItemObservations)
                        {
                            var x = new Observation();
                            x.DeviceUuid = observation.DeviceUuid;
                            x.DataItemId = observation.DataItemId;
                            x.DataItem = observation.DataItem;
                            x.Name = observation.Name;
                            x.Category = observation.Category;
                            x.Type = observation.Type;
                            x.SubType = observation.SubType;
                            x.Representation = observation.Representation;
                            x.CompositionId = observation.CompositionId;
                            x.InstanceId = observation.InstanceId;
                            x.Sequence = observation.Sequence;
                            x.Timestamp = observation.Timestamp;
                            x.AddValues(observation.Values);

                            multipleObservations.Add(x);
                        }

                        await _entityServer.PublishObservations(_mqttServer, multipleObservations);
                    }
                    else
                    {
                        var observation = dataItemObservations.FirstOrDefault();

                        var x = new Observation();
                        x.DeviceUuid = observation.DeviceUuid;
                        x.DataItemId = observation.DataItemId;
                        x.DataItem = observation.DataItem;
                        x.Name = observation.Name;
                        x.Category = observation.Category;
                        x.Type = observation.Type;
                        x.SubType = observation.SubType;
                        x.Representation = observation.Representation;
                        x.CompositionId = observation.CompositionId;
                        x.InstanceId = observation.InstanceId;
                        x.Sequence = observation.Sequence;
                        x.Timestamp = observation.Timestamp;
                        x.AddValues(observation.Values);

                        await _entityServer.PublishObservation(_mqttServer, x);
                    }
                }
            }
        }


        private async Task PublishAssets()
        {
            if (_entityServer != null)
            {
                var assets = Agent.GetAssets();
                if (!assets.IsNullOrEmpty())
                {
                    foreach (var asset in assets)
                    {
                        await _entityServer.PublishAsset(_mqttServer, asset);
                    }
                }
            }
        }


        private async void AgentDeviceAdded(object sender, IDevice device)
        {
            if (_entityServer != null) await _entityServer.PublishDevice(_mqttServer, device);
        }

        private async void AgentObservationAdded(object sender, IObservation observation)
        {
            if (_entityServer != null)
            {
                if (observation.Category == DataItemCategory.CONDITION)
                {
                    var conditionObservations = Agent.GetCurrentObservations(observation.DeviceUuid, observation.DataItemId);
                    await PublishObservations(conditionObservations);
                }
                else
                {
                    await _entityServer.PublishObservation(_mqttServer, observation);
                }
            }
        }

        private async void AgentAssetAdded(object sender, IAsset asset)
        {
            if (_entityServer != null) await _entityServer.PublishAsset(_mqttServer, asset);
        }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Logging;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using MTConnect.Observations.Output;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-relay";
        private const string ModuleId = "MQTT Relay";

        private readonly MqttRelayModuleConfiguration _configuration;
        private readonly MTConnectMqttDocumentServer _documentServer;
        private readonly MTConnectMqttEntityServer _entityServer;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttRelayModuleConfiguration>(configuration);

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

                    _entityServer = new MTConnectMqttEntityServer(_configuration.TopicPrefix, _configuration.DocumentFormat, _configuration.QoS);
                    Agent.DeviceAdded += AgentDeviceAdded;
                    Agent.ObservationAdded += AgentObservationAdded;
                    Agent.AssetAdded += AgentAssetAdded;
                    break;
            }

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
        }


        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);
        }

        protected override void OnStop()
        {
            _documentServer.Stop();

            if (_stop != null) _stop.Cancel();

            try
            {
                if (_mqttClient != null)
                {
                    _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
                }
            }
            catch { }
        }

        private async Task Worker()
        {
            do
            {
                try
                {
                    try
                    {
                        // Declare new MQTT Client Options with Tcp Server
                        var clientOptionsBuilder = new MqttClientOptionsBuilder();

                        // Add TCP Server
                        clientOptionsBuilder.WithTcpServer(_configuration.Server, _configuration.Port);

                        // Publish Only so use Clean Session = true
                        clientOptionsBuilder.WithCleanSession();

                        // Sets the Timeout
                        clientOptionsBuilder.WithTimeout(TimeSpan.FromMilliseconds(_configuration.Timeout));

                        // Set LWT (Agent Available)
                        clientOptionsBuilder.WithWillTopic(GetAgentAvailableTopic());
                        clientOptionsBuilder.WithWillPayload(System.Text.Encoding.UTF8.GetBytes(Availability.UNAVAILABLE.ToString()));
                        clientOptionsBuilder.WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                        clientOptionsBuilder.WithWillRetain(true);

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_configuration.ClientId))
                        {
                            clientOptionsBuilder.WithClientId(_configuration.ClientId);
                        }

                        // Set TLS Certificate
                        if (_configuration.Tls != null)
                        {
                            var certificateResults = _configuration.Tls.GetCertificate();
                            if (certificateResults.Success && certificateResults.Certificate != null)
                            {
                                var certificateAuthorityResults = _configuration.Tls.GetCertificateAuthority();

                                var certificates = new List<X509Certificate2>();
                                if (certificateAuthorityResults.Certificate != null && _configuration.Tls.OmitCAValidation == false)
                                {
                                    certificates.Add(certificateAuthorityResults.Certificate);
                                }
                                certificates.Add(certificateResults.Certificate);

                                var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();

                                // Set Client Certificate
                                tlsOptionsBuilder.WithClientCertificates(certificates);

                                // Set VerifyClientCertificate option
                                tlsOptionsBuilder.WithAllowUntrustedCertificates(!_configuration.Tls.VerifyClientCertificate);

#if NET5_0_OR_GREATER
                                // Setup CA Certificate
                                if (certificateAuthorityResults.Certificate != null)
                                {
                                    tlsOptionsBuilder.WithCertificateValidationHandler((certContext) =>
                                    {
                                        var chain = new X509Chain();
                                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                                        chain.ChainPolicy.VerificationTime = DateTime.Now;
                                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
                                        chain.ChainPolicy.CustomTrustStore.Add(certificateAuthorityResults.Certificate);
                                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

                                        // convert provided X509Certificate to X509Certificate2
                                        var x5092 = new X509Certificate2(certContext.Certificate);

                                        return chain.Build(x5092);
                                    });
                                }
#endif

                                clientOptionsBuilder.WithTlsOptions(tlsOptionsBuilder.Build());
                            }
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();
                                tlsOptionsBuilder.WithSslProtocols(System.Security.Authentication.SslProtocols.Tls12);
                                clientOptionsBuilder.WithTlsOptions(tlsOptionsBuilder.Build());

                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                            else
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                        }

                        // Build MQTT Client Options
                        var clientOptions = clientOptionsBuilder.Build();

                        await _mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

                        Log(MTConnectLogLevel.Information, $"MQTT Relay Connected to External Broker ({_configuration.Server}:{_configuration.Port})");

                        // Start Document Server (if configured)
                        if (_documentServer != null) _documentServer.Start();

                        // Initialize Entity Server (if configured)
                        if (_entityServer != null)
                        {
                            await PublishDevices();
                            await PublishCurrentObservations();
                            await PublishAssets();
                        }

                        while (!_stop.Token.IsCancellationRequested && _mqttClient.IsConnected)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error : {exception.Message}");

                        var innerException = exception.InnerException;

                        while (innerException != null)
                        {
                            Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error (InnerException) : {innerException.Message}");
                            innerException = innerException.InnerException;
                        }

                        Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error (BaseException) : {exception.GetBaseException().ToString()}");
                    }
                    finally
                    {
                        if (_documentServer != null) _documentServer.Stop();
                    }

                    Log(MTConnectLogLevel.Information, $"MQTT Relay Disconnected from External Broker ({_configuration.Server}:{_configuration.Port})");

                    await Task.Delay(_configuration.ReconnectInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);
        }



        private async void ProbeReceived(IDevice device, IDevicesResponseDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, responseDocument, x);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.ProbeTopic}/{device.Uuid}";
                    if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                        messageBuilder.WithPayload(formatResult.Content);
                        var message = messageBuilder.Build();

                        var publishResult = await _mqttClient.PublishAsync(message);
                        if (publishResult.IsSuccess)
                        {
                            Log(MTConnectLogLevel.Debug, $"Probe : Published to Topic ({topic})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Warning, $"Probe : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Probe : Error Publishing to Topic ({topic}) : {ex.Message}");
                    }


                    // Write Available (for Agent Device)
                    if (device.Type == Devices.Agent.TypeId)
                    {
                        var availableTopic = GetAgentAvailableTopic();
                        var availablePayload = System.Text.Encoding.UTF8.GetBytes(Availability.AVAILABLE.ToString());

                        try
                        {
                            var messageBuilder = new MqttApplicationMessageBuilder();
                            messageBuilder.WithRetainFlag(true);
                            messageBuilder.WithTopic(availableTopic);
                            messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
#if NET5_0_OR_GREATER
                            messageBuilder.WithPayloadSegment(availablePayload);
#else
                            messageBuilder.WithPayload(availablePayload);
#endif
                            var message = messageBuilder.Build();

                            var publishResult = await _mqttClient.PublishAsync(message);
                            if (publishResult.IsSuccess)
                            {
                                Log(MTConnectLogLevel.Debug, $"Agent Available : Published to Topic ({availableTopic})");
                            }
                            else
                            {
                                Log(MTConnectLogLevel.Warning, $"Agent Available : Error Publishing to Topic ({availableTopic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(MTConnectLogLevel.Warning, $"Agent Available : Error Publishing to Topic ({availableTopic}) : {ex.Message}");
                        }
                    }
                }
            }
        }

        private async void CurrentReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument, x);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.CurrentTopic}/{device.Uuid}";
                    if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    //messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    try
                    {
                        var publishResult = await _mqttClient.PublishAsync(message);
                        if (publishResult.IsSuccess)
                        {
                            Log(MTConnectLogLevel.Debug, $"Current : Published to Topic ({topic})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Warning, $"Current : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Current : Error Publishing to Topic ({topic}) : {ex.Message}");
                    }
                }
            }
        }

        private async void SampleReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument, x);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.SampleTopic}/{device.Uuid}";
                    if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    //messageBuilder.WithRetainFlag(true);
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    messageBuilder.WithPayload(formatResult.Content);
                    var message = messageBuilder.Build();

                    try
                    {
                        var publishResult = await _mqttClient.PublishAsync(message);
                        if (publishResult.IsSuccess)
                        {
                            Log(MTConnectLogLevel.Debug, $"Sample : Published to Topic ({topic})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Warning, $"Sample : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"Sample : Error Publishing to Topic ({topic}) : {ex.Message}");
                    }
                }
            }
        }

        private async void AssetReceived(IDevice device, IAssetsResponseDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected && responseDocument != null && !responseDocument.Assets.IsNullOrEmpty())
            {
                var x = new List<KeyValuePair<string, string>>();
                x.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

                foreach (var asset in responseDocument.Assets)
                {
                    var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, asset, x);
                    if (formatResult.Success)
                    {
                        var topic = $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.AssetTopic}/{device.Uuid}/{asset.AssetId}";
                        if (formatResult.Content != null && formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                        messageBuilder.WithPayload(formatResult.Content);
                        var message = messageBuilder.Build();

                        try
                        {
                            var publishResult = await _mqttClient.PublishAsync(message);
                            if (publishResult.IsSuccess)
                            {
                                Log(MTConnectLogLevel.Debug, $"Asset : Published to Topic ({topic})");
                            }
                            else
                            {
                                Log(MTConnectLogLevel.Warning, $"Asset : Error Publishing to Topic ({topic}) : {publishResult.ReasonCode} : {publishResult.ReasonString}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(MTConnectLogLevel.Warning, $"Asset : Error Publishing to Topic ({topic}) : {ex.Message}");
                        }
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
                        await _entityServer.PublishDevice(_mqttClient, device);
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

                        await _entityServer.PublishObservations(_mqttClient, multipleObservations);
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

                        await _entityServer.PublishObservation(_mqttClient, x);
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
                        await _entityServer.PublishAsset(_mqttClient, asset);
                    }
                }
            }
        }


        private async void AgentDeviceAdded(object sender, IDevice device)
        {
            if (_entityServer != null) await _entityServer.PublishDevice(_mqttClient, device);
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
                    await _entityServer.PublishObservation(_mqttClient, observation);
                }
            }
        }

        private async void AgentAssetAdded(object sender, IAsset asset)
        {
            if (_entityServer != null) await _entityServer.PublishAsset(_mqttClient, asset);
        }


        private string GetAgentAvailableTopic()
        {
            if (Agent != null && _configuration != null)
            {
                return $"{_configuration.TopicPrefix}/{MTConnectMqttDocumentServer.ProbeTopic}/{Agent.Uuid}/Available"; ;
            }

            return null;
        }
    }
}

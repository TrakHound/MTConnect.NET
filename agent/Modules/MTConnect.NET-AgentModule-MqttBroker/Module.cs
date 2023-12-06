// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Streams.Output;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-broker";

        private readonly ModuleConfiguration _configuration;
        private readonly MTConnectMqttServer _server;
        private MqttServer _mqttServer;
        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);

            _server = new MTConnectMqttServer(mtconnectAgent, _configuration);
            _server.ProbeReceived += ProbeReceived;
            _server.CurrentReceived += CurrentReceived;
            _server.SampleReceived += SampleReceived;
            _server.AssetReceived += AssetReceived;
        }


        protected override void OnStartBeforeLoad()
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
                        var mqttServerOptionsBuilder = new MqttServerOptionsBuilder().WithDefaultEndpoint();

                        // Add Certificate & Private Key
                        if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
                        {
                            X509Certificate2 certificate = null;

#if NET5_0_OR_GREATER
                            certificate = new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx));
#endif

                            if (certificate != null)
                            {
                                mqttServerOptionsBuilder.WithoutDefaultEndpoint();
                                mqttServerOptionsBuilder.WithEncryptedEndpoint();
                                mqttServerOptionsBuilder.WithEncryptedEndpointPort(_configuration.Port);
                                mqttServerOptionsBuilder.WithEncryptionCertificate(certificate);
                                mqttServerOptionsBuilder.WithEncryptionSslProtocol(System.Security.Authentication.SslProtocols.Tls12);
                            }
                        }

                        var mqttServerOptions = mqttServerOptionsBuilder.Build();

                        var mqttFactory = new MqttFactory();
                        _mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

                        _mqttServer.ClientConnectedAsync += async (args) =>
                        {
                            Log(Logging.MTConnectLogLevel.Information, $"MQTT Server : Client Connected : {args.ClientId} : {args.Endpoint}");
                        };
                        _mqttServer.ClientDisconnectedAsync += async (args) =>
                        {
                            Log(Logging.MTConnectLogLevel.Information, $"MQTT Server : Client Disconnected : {args.ClientId} : {args.Endpoint}");
                        };

                        await _mqttServer.StartAsync();
                        _server.Start();

                        Log(Logging.MTConnectLogLevel.Information, "MQTT Server Started..");

                        while (!_stop.Token.IsCancellationRequested && _mqttServer.IsStarted)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(Logging.MTConnectLogLevel.Warning, $"MQTT Server Error : {ex.Message}");
                    }

                    Log(Logging.MTConnectLogLevel.Information, $"MQTT Server Stopped");

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
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.ProbeTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

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
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.CurrentTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

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
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.SampleTopic}";

                    var message = new MqttApplicationMessage();
                    //message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

                    var injectMessage = new InjectedMqttApplicationMessage(message);

                    await _mqttServer.InjectApplicationMessage(injectMessage);
                }
            }
        }

        private async void AssetReceived(IDevice device, IAssetsResponseDocument responseDocument)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.AssetTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

                    var injectMessage = new InjectedMqttApplicationMessage(message);

                    await _mqttServer.InjectApplicationMessage(injectMessage);
                }
            }
        }


        private static string GetFilePath(string path)
        {
            var x = path;
            if (!Path.IsPathRooted(x))
            {
                x = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, x);
            }

            return x;
        }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Certificates;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Logging;
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
        private readonly MTConnectMqttDocumentServer _server;
        private MqttServer _mqttServer;
        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttBrokerModuleConfiguration>(configuration);

            _server = new MTConnectMqttDocumentServer(mtconnectAgent, _configuration);
            _server.ProbeReceived += ProbeReceived;
            _server.CurrentReceived += CurrentReceived;
            _server.SampleReceived += SampleReceived;
            _server.AssetReceived += AssetReceived;
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

                        await _mqttServer.StartAsync();
                        _server.Start();

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
    }
}
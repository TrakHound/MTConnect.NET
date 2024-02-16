// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
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
        private readonly MTConnectMqttDocumentServer _server;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttRelayModuleConfiguration>(configuration);

            _server = new MTConnectMqttDocumentServer(mtconnectAgent, _configuration);
            _server.ProbeReceived += ProbeReceived;
            _server.CurrentReceived += CurrentReceived;
            _server.SampleReceived += SampleReceived;
            _server.AssetReceived += AssetReceived;

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
            _server.Stop();

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

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_configuration.ClientId))
                        {
                            clientOptionsBuilder.WithClientId(_configuration.ClientId);
                        }

                        var certificates = new List<X509Certificate2>();

                        // Add CA (Certificate Authority)
                        if (!string.IsNullOrEmpty(_configuration.CertificateAuthority))
                        {
                            certificates.Add(new X509Certificate2(GetFilePath(_configuration.CertificateAuthority)));
                        }

                        // Add Client Certificate & Private Key
                        if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
                        {
#if NET5_0_OR_GREATER
                            certificates.Add(new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx)));
#else
                            throw new Exception("PEM Certificates Not Supported in .NET Framework 4.8 or older");
#endif

                            clientOptionsBuilder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                            {
                                UseTls = true,
                                SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                                IgnoreCertificateRevocationErrors = _configuration.AllowUntrustedCertificates,
                                IgnoreCertificateChainErrors = _configuration.AllowUntrustedCertificates,
                                AllowUntrustedCertificates = _configuration.AllowUntrustedCertificates,
                                Certificates = certificates
                            });
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password).WithTls();
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

                        _server.Start();

                        while (!_stop.Token.IsCancellationRequested && _mqttClient.IsConnected)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"MQTT Relay Connection Error : {ex.Message}");
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

                    //var message = new MqttApplicationMessage();
                    //message.Retain = true;
                    //message.Topic = topic;
                    //message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                    //message.Payload = formatResult.Content;

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

                    //var message = new MqttApplicationMessage();
                    ////message.Retain = true;
                    //message.Topic = topic;
                    //message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                    //message.Payload = formatResult.Content;

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

                    //var message = new MqttApplicationMessage();
                    ////message.Retain = true;
                    //message.Topic = topic;
                    //message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                    //message.Payload = formatResult.Content;

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

                        //var message = new MqttApplicationMessage();
                        //message.Retain = true;
                        //message.Topic = topic;
                        //message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                        //message.Payload = formatResult.Content;

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

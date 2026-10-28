// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Input;
using MTConnect.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Modules
{
    /// <summary>
    /// Agent module that subscribes to an upstream MQTT broker and
    /// converts published <c>observations</c>, <c>assets</c>, and
    /// <c>device</c> messages into observations the local agent broker
    /// can serve. Pairs with an upstream agent's MQTT-output / MQTT-
    /// broker module.
    /// </summary>
    public class Module : MTConnectAgentModule
    {
        // Topic prefixes (built once from the configuration)
        private string _observationTopicPrefix;
        private string _assetTopicPrefix;
        private string _deviceTopicPrefix;

        /// <summary>
        /// Token used in <c>agent.config.yaml</c> to bind this module
        /// (<c>type: mqtt-adapter</c>).
        /// </summary>
        public const string ConfigurationTypeId = "mqtt-adapter";

        /// <summary>
        /// The class this module's <c>agent.config.yaml</c> settings are read into.
        /// Lets tools such as the Web UI describe the module's options.
        /// </summary>
        public static readonly System.Type ConfigurationType = typeof(MqttAdapterModuleConfiguration);
        private const string ModuleId = "MQTT Adapter";
        private const string ObservationTopic = "observations";
        private const string AssetTopic = "assets";
        private const string DeviceTopic = "device";

        private readonly MqttAdapterModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        /// <summary>
        /// Initialises the module, creates the MQTTnet client, and binds
        /// the supplied configuration payload to
        /// <see cref="MqttAdapterModuleConfiguration"/>.
        /// </summary>
        /// <param name="mtconnectAgent">Local agent broker the
        /// translated observations are written into.</param>
        /// <param name="configuration">Raw configuration payload.</param>
        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _mtconnectAgent = mtconnectAgent;
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += MessageReceived;

            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttAdapterModuleConfiguration>(configuration);
        }


        /// <summary>
        /// Module lifecycle hook: spawns the MQTT subscription worker on
        /// a background task. Subscriptions are managed by <c>Worker</c>
        /// which reconnects on failure.
        /// </summary>
        /// <param name="initializeDataItems">Inherited flag; unused.</param>
        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            if (_configuration != null)
            {
                _stop = new CancellationTokenSource();

                _ = Task.Run(Worker, _stop.Token);
            }
        }


        /// <summary>
        /// Module lifecycle hook: cancels the subscription worker and
        /// disposes the MQTT client.
        /// </summary>
        protected override void OnStop()
        {
            if (_stop != null) _stop.Cancel();
            if (_mqttClient != null) _mqttClient.Dispose();
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

                        // Set TCP Server
                        clientOptionsBuilder.WithTcpServer(_configuration.Server, _configuration.Port);

                        // Publish Only so use Clean Session = true
                        clientOptionsBuilder.WithCleanSession(_configuration.CleanSession);

                        // Sets the Timeout
                        clientOptionsBuilder.WithTimeout(TimeSpan.FromMilliseconds(_configuration.Timeout));

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_configuration.ClientId))
                        {
                            clientOptionsBuilder.WithClientId(_configuration.ClientId);
                        }
                        else
                        {
                            // Generate default Client ID (Agent UUID & DeviceKey)
                            clientOptionsBuilder.WithClientId($"{Agent.Uuid}::{_configuration.DeviceKey}".ToSHA1Hash());
                        }

                        var certificates = new List<X509Certificate2>();

                        // Add CA (Certificate Authority)
                        if (!string.IsNullOrEmpty(_configuration.CertificateAuthority))
                        {
#if NET9_0_OR_GREATER
                            certificates.Add(X509CertificateLoader.LoadCertificateFromFile(GetFilePath(_configuration.CertificateAuthority)));
#else
                            certificates.Add(new X509Certificate2(GetFilePath(_configuration.CertificateAuthority)));
#endif
                        }

                        // Add Client Certificate & Private Key
                        if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
                        {
#if NET5_0_OR_GREATER

#if NET9_0_OR_GREATER
                            certificates.Add(X509CertificateLoader.LoadCertificate(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx)));
#else
                            certificates.Add(new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx)));
#endif

                            clientOptionsBuilder.WithTlsOptions(b => b
                                .WithSslProtocols(System.Security.Authentication.SslProtocols.Tls12)
                                .WithIgnoreCertificateRevocationErrors(_configuration.AllowUntrustedCertificates)
                                .WithIgnoreCertificateChainErrors(_configuration.AllowUntrustedCertificates)
                                .WithAllowUntrustedCertificates(_configuration.AllowUntrustedCertificates)
                                .WithClientCertificates(certificates));
#else
                            throw new Exception("PEM Certificates Not Supported in .NET Framework 4.8 or older");
#endif
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password).WithTlsOptions(b => { });
                            }
                            else
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                        }

                        // Build MQTT Client Options
                        var clientOptions = clientOptionsBuilder.Build();

                        // Connect to the MQTT Client
                        await _mqttClient.ConnectAsync(clientOptions);

                        Log(MTConnectLogLevel.Information, $"MQTT Adapter Connected to External Broker ({_configuration.Server}:{_configuration.Port})");

                        if (!string.IsNullOrEmpty(_configuration.TopicPrefix))
                        {
                            // Set Qos
                            MqttQualityOfServiceLevel qos;
                            switch (_configuration.Qos)
                            {
                                case 1: qos = MqttQualityOfServiceLevel.AtLeastOnce; break;
                                case 2: qos = MqttQualityOfServiceLevel.ExactlyOnce; break;
                                default: qos = MqttQualityOfServiceLevel.AtMostOnce; break;
                            }

                            // Subscribe to Topic
                            await _mqttClient.SubscribeAsync($"{_configuration.TopicPrefix}/#", qos);

                            Log(MTConnectLogLevel.Information, $"MQTT Adapter Subscribed to ({_configuration.TopicPrefix} @ Qos = {qos})");
                        }
                        else
                        {
                            Log(MTConnectLogLevel.Information, $"MQTT Adapter : TopicPrefix Configuration Parameter Required");
                        }

                        while (_mqttClient.IsConnected && !_stop.IsCancellationRequested)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"MQTT Adapter Connection Error : {ex.Message}");
                    }

                    Log(MTConnectLogLevel.Information, $"MQTT Adapter Disconnected from External Broker ({_configuration.Server}:{_configuration.Port})");

                    await Task.Delay(_configuration.ReconnectInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);


            try
            {
                // Disconnect from the MQTT Client
                if (_mqttClient != null) _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection).Wait();
            }
            catch { }
        }

        private Task MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            // Only the bytes within the segment belong to this message (the backing array may be larger)
            var payload = GetPayload(args.ApplicationMessage);
            if (payload != null && payload.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                if (IsLogEnabled(MTConnectLogLevel.Debug)) Log(MTConnectLogLevel.Debug, $"MQTT Message Received : Topic = {topic}");
                if (IsLogEnabled(MTConnectLogLevel.Trace)) Log(MTConnectLogLevel.Trace, $"MQTT Message Received : Topic = {topic} : {System.Text.Encoding.UTF8.GetString(payload)}");

                if (IsObservationTopic(topic))
                {
                    var observations = ProcessObservationPayload(payload);
                    if (!observations.IsNullOrEmpty())
                    {
                        _mtconnectAgent.AddObservations(_configuration.DeviceKey, observations);
                    }
                }
                else if (IsAssetTopic(topic))
                {
                    var assets = ProcessAssetPayload(payload);
                    if (!assets.IsNullOrEmpty())
                    {
                        _mtconnectAgent.AddAssets(_configuration.DeviceKey, assets);
                    }
                }
                else if (IsDeviceTopic(topic))
                {
                    var device = ProcessDevicePayload(payload);
                    if (device != null && !string.IsNullOrEmpty(device.Uuid) && !string.IsNullOrEmpty(device.Name))
                    {
                        if (!string.IsNullOrEmpty(_configuration.DeviceKey))
                        {
                            if (_configuration.DeviceKey.ToLower() == device.Uuid.ToLower() ||
                                _configuration.DeviceKey.ToLower() == device.Name.ToLower())
                            {
                                _mtconnectAgent.AddDevice(device);
                            }
                        }
                        else
                        {
                            _mtconnectAgent.AddDevice(device);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static byte[] GetPayload(MqttApplicationMessage message)
        {
            var segment = message.PayloadSegment;
            if (segment.Array == null || segment.Count == 0) return null;
            if (segment.Offset == 0 && segment.Count == segment.Array.Length) return segment.Array;

            var payload = new byte[segment.Count];
            Buffer.BlockCopy(segment.Array, segment.Offset, payload, 0, segment.Count);
            return payload;
        }

        private bool IsObservationTopic(string topic)
        {
            if (topic != null)
            {
                if (_observationTopicPrefix == null) _observationTopicPrefix = $"{_configuration.TopicPrefix}/{ObservationTopic}";
                return topic.StartsWith(_observationTopicPrefix, StringComparison.Ordinal);
            }

            return false;
        }

        private bool IsAssetTopic(string topic)
        {
            if (topic != null)
            {
                if (_assetTopicPrefix == null) _assetTopicPrefix = $"{_configuration.TopicPrefix}/{AssetTopic}";
                return topic.StartsWith(_assetTopicPrefix, StringComparison.Ordinal);
            }

            return false;
        }

        private bool IsDeviceTopic(string topic)
        {
            if (topic != null)
            {
                if (_deviceTopicPrefix == null) _deviceTopicPrefix = $"{_configuration.TopicPrefix}/{DeviceTopic}";
                return topic.StartsWith(_deviceTopicPrefix, StringComparison.Ordinal);
            }

            return false;
        }


        private IEnumerable<IObservationInput> ProcessObservationPayload(byte[] payload)
        {
            if (!payload.IsNullOrEmpty())
            {
                try
                {
                    var uncompressedBytes = payload;
                    if (!uncompressedBytes.IsNullOrEmpty())
                    {
                        var readResult = InputFormatter.CreateObservations(_configuration.DocumentFormat, uncompressedBytes);
                        if (readResult.Success)
                        {
                            return readResult.Content;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(MTConnectLogLevel.Error, $"MQTT Observation Payload Error : {ex.Message}");
                }
            }

            return null;
        }

        private IEnumerable<IAsset> ProcessAssetPayload(byte[] payload)
        {
            if (!payload.IsNullOrEmpty())
            {
                try
                {
                    var uncompressedBytes = payload;
                    if (!uncompressedBytes.IsNullOrEmpty())
                    {
                        var readResult = InputFormatter.CreateAssets(_configuration.DocumentFormat, uncompressedBytes);
                        if (readResult.Success)
                        {
                            return readResult.Content;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(MTConnectLogLevel.Error, $"MQTT Asset Payload Error : {ex.Message}");
                }
            }

            return null;
        }

        private IDevice ProcessDevicePayload(byte[] payload)
        {
            if (!payload.IsNullOrEmpty())
            {
                try
                {
                    var uncompressedBytes = payload;
                    if (!uncompressedBytes.IsNullOrEmpty())
                    {
                        var readResult = InputFormatter.CreateDevice(_configuration.DocumentFormat, uncompressedBytes);
                        if (readResult.Success)
                        {
                            return readResult.Content;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(MTConnectLogLevel.Error, $"MQTT Observation Device Error : {ex.Message}");
                }
            }

            return null;
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
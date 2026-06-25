// Copyright(c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Adapters;
using MTConnect.Configurations;
using MTConnect.Formatters;
using MTConnect.Input;
using MTConnect.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect
{
    /// <summary>
    /// Adapter module that publishes observations to a downstream MQTT
    /// broker. Each adapter-side observation is serialised via the
    /// configured <see cref="ModuleConfiguration.DocumentFormat"/> and
    /// published to <see cref="ModuleConfiguration.Topic"/>. Supports
    /// TLS (mutual or CA-only), authentication, and configurable QoS.
    /// </summary>
    public class Module : MTConnectAdapterModule
    {
        /// <summary>
        /// Token used in <c>adapter.config.yaml</c> to bind this
        /// module (<c>type: mqtt</c>).
        /// </summary>
        public const string ConfigurationTypeId = "mqtt";
        private const string ModuleId = "MQTT";

        private readonly ModuleConfiguration _configuration;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        /// <summary>
        /// Initialises the module against the supplied
        /// <paramref name="id"/> and configuration payload. The MQTT
        /// connection itself is opened by the background worker that
        /// <see cref="OnStart"/> spawns.
        /// </summary>
        /// <param name="id">Adapter-side identifier the host passes
        /// through.</param>
        /// <param name="moduleConfiguration">Raw configuration payload
        /// bound to <see cref="ModuleConfiguration"/>; defaults to a
        /// fresh instance when <c>null</c>.</param>
        public Module(string id, object moduleConfiguration) : base(id)
        {
            Id = ModuleId;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            _configuration = AdapterApplicationConfiguration.GetConfiguration<ModuleConfiguration>(moduleConfiguration);
            if (_configuration == null) _configuration = new ModuleConfiguration();
        }


        /// <summary>
        /// Module lifecycle hook: spawns the MQTT publish worker on a
        /// background task. The worker reconnects on transport failure.
        /// </summary>
        protected override void OnStart()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Module lifecycle hook: cancels the publish worker and
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

                        // Set TCP Settings
                        clientOptionsBuilder.WithTcpServer(_configuration.Server, _configuration.Port);

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

                        Log(MTConnectLogLevel.Information, "MQTT Client Connected");

                        while (_mqttClient.IsConnected && !_stop.IsCancellationRequested)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(MTConnectLogLevel.Warning, $"MQTT Client Connection Error : {ex.Message}");
                    }

                    await Task.Delay(_configuration.ReconnectInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Log(MTConnectLogLevel.Error, $"MQTT Module Error : {ex.Message}");
                }

            } while (!_stop.Token.IsCancellationRequested);


            try
            {
                // Disconnect from the MQTT Client
                if (_mqttClient != null) await _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
            }
            catch { }
        }


        /// <summary>
        /// Serialises the supplied observations via the configured
        /// document format and publishes them to
        /// <c>{Topic}/observations</c> with the configured QoS and the
        /// retain flag set.
        /// </summary>
        /// <param name="observations">Observations to publish.</param>
        /// <returns>Always <c>true</c>; transport-level errors are
        /// logged but do not break the adapter pipeline.</returns>
        public override bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            if (_mqttClient != null && !observations.IsNullOrEmpty())
            {
                var formatResult = InputFormatter.Format(_configuration.DocumentFormat, observations);
                if (formatResult.Success)
                {
                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithTopic($"{_configuration.Topic}/observations");
                    messageBuilder.WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.Qos);
                    messageBuilder.WithPayload(formatResult.Content);
                    messageBuilder.WithRetainFlag(true);

                    var message = messageBuilder.Build();
                    _mqttClient.PublishAsync(message);

                    Log(MTConnectLogLevel.Debug, $"MQTT Observations Message Published to {message.Topic}");
                }
            }

            return true;
        }

        /// <summary>
        /// Serialises the supplied assets via the configured document
        /// format and publishes them to <c>{Topic}/assets</c> with the
        /// configured QoS and the retain flag set.
        /// </summary>
        /// <param name="assets">Assets to publish.</param>
        /// <returns>Always <c>true</c>; transport-level errors are
        /// logged but do not break the adapter pipeline.</returns>
        public override bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            if (_mqttClient != null && !assets.IsNullOrEmpty())
            {
                var formatResult = InputFormatter.Format(_configuration.DocumentFormat, assets);
                if (formatResult.Success)
                {
                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithTopic($"{_configuration.Topic}/assets");
                    messageBuilder.WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.Qos);
                    messageBuilder.WithPayload(formatResult.Content);
                    messageBuilder.WithRetainFlag(true);

                    var message = messageBuilder.Build();
                    _mqttClient.PublishAsync(message);

                    Log(MTConnectLogLevel.Debug, $"MQTT Assets Message Published to {message.Topic}");
                }
            }

            return true;
        }

        /// <summary>
        /// Serialises the supplied device descriptors via the configured
        /// document format and publishes them to <c>{Topic}/devices</c>
        /// with the configured QoS and the retain flag set.
        /// </summary>
        /// <param name="devices">Devices to publish.</param>
        /// <returns>Always <c>true</c>; transport-level errors are
        /// logged but do not break the adapter pipeline.</returns>
        public override bool AddDevices(IEnumerable<IDeviceInput> devices)
        {
            if (_mqttClient != null && !devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    var formatResult = InputFormatter.Format(_configuration.DocumentFormat, device);
                    if (formatResult.Success)
                    {
                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithTopic($"{_configuration.Topic}/device");
                        messageBuilder.WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.Qos);
                        messageBuilder.WithPayload(formatResult.Content);
                        messageBuilder.WithRetainFlag(true);

                        var message = messageBuilder.Build();
                        _mqttClient.PublishAsync(message);

                        Log(MTConnectLogLevel.Debug, $"MQTT Device Message Published to {message.Topic}");
                    }
                }
            }

            return true;
        }


        private static byte[] CompressPayload(byte[] payload)
        {
            var bytes = payload;

            using (var ms = new MemoryStream())
            {
                using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(bytes, 0, bytes.Length);
                }
                bytes = ms.ToArray();
            }

            return bytes;
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

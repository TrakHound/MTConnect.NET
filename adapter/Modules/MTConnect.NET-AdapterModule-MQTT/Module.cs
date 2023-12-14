// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
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
    public class Module : MTConnectAdapterModule
    {
        public const string ConfigurationTypeId = "mqtt";

        private readonly ModuleConfiguration _configuration;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        public Module(string id, object moduleConfiguration) : base(id)
        {
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            _configuration = AdapterApplicationConfiguration.GetConfiguration<ModuleConfiguration>(moduleConfiguration);
            if (_configuration == null) _configuration = new ModuleConfiguration();
        }


        protected override void OnStart()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);
        }

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


        public override bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            if (_mqttClient != null && !observations.IsNullOrEmpty())
            {
                var formatResult = InputFormatter.Format(_configuration.DocumentFormat, observations);
                if (formatResult.Success)
                {
                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                    message.Topic = $"{_configuration.Topic}/observations";
                    message.Payload = formatResult.Content;
                    _mqttClient.PublishAsync(message);

                    Log(MTConnectLogLevel.Debug, $"MQTT Observations Message Published to {message.Topic}");
                }
            }

            return true;
        }

        public override bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            if (_mqttClient != null && !assets.IsNullOrEmpty())
            {
                var formatResult = InputFormatter.Format(_configuration.DocumentFormat, assets);
                if (formatResult.Success)
                {
                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                    message.Topic = $"{_configuration.Topic}/assets";
                    message.Payload = formatResult.Content;
                    _mqttClient.PublishAsync(message);

                    Log(MTConnectLogLevel.Debug, $"MQTT Assets Message Published to {message.Topic}");
                }
            }

            return true;
        }

        public override bool AddDevices(IEnumerable<IDeviceInput> devices)
        {
            if (_mqttClient != null && !devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    var formatResult = InputFormatter.Format(_configuration.DocumentFormat, device);
                    if (formatResult.Success)
                    {
                        var message = new MqttApplicationMessage();
                        message.Retain = true;
                        message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)_configuration.QoS;
                        message.Topic = $"{_configuration.Topic}/device";
                        message.Payload = formatResult.Content;
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

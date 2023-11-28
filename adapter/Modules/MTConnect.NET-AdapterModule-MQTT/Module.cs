// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Adapters;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Mqtt;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace MTConnect
{
    public class Module : MTConnectAdapterModule
    {
        public const string ConfigurationTypeId = "mqtt";

        private readonly MqttAdapterModuleConfiguration _configuration;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        public Module(string id, object moduleConfiguration) : base(id)
        {
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            _configuration = AdapterApplicationConfiguration.GetConfiguration<MqttAdapterModuleConfiguration>(moduleConfiguration);
            if (_configuration == null) _configuration = new MqttAdapterModuleConfiguration();
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
                        var clientOptionsBuilder = new MqttClientOptionsBuilder().WithTcpServer(_configuration.Server, _configuration.Port);

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
                        _mqttClient.ConnectAsync(clientOptions).Wait();

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
                if (_mqttClient != null) _mqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection).Wait();
            }
            catch { }
        }


        public override bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            if (_mqttClient != null && !observations.IsNullOrEmpty())
            {
                var mqttModels = new List<MTConnectMqttInputModel>();

                var timestamps = observations.Select(o => o.Timestamp).Distinct();
                foreach (var timestamp in timestamps)
                {
                    var timestampObservations = observations.Where(o => o.Timestamp == timestamp);

                    var mqttModel = new MTConnectMqttInputModel();
                    mqttModel.Timestamp = timestamp.ToDateTime();
                    
                    foreach (var observation in timestampObservations)
                    {
                        var mqttObservation = new MTConnectMqttInputObservation(observation.DataItemKey, observation.Values.ToDictionary(o => o.Key, o => o.Value));
                        mqttModel.Observations.Add(mqttObservation);

                        //mqttModel.Observations.Add(observation.DataItemKey, observation.Values.ToDictionary(o => o.Key, o => o.Value));
                    }

                    mqttModels.Add(mqttModel);
                }


                var json = JsonSerializer.Serialize(mqttModels);
                //Console.WriteLine(json);

                var utf8 = System.Text.Encoding.UTF8.GetBytes(json);
                Console.WriteLine($"JSON = {utf8.Length / 1000}");

                var payload = CompressPayload(utf8);
                Console.WriteLine($"JSON-gzip = {payload.Length / 1000}");

                var message = new MqttApplicationMessage();
                message.Topic = $"{_configuration.Topic}/{_configuration.DeviceKey}/observations-gzip";
                message.Payload = payload;
                _mqttClient.PublishAsync(message);

                //var message2 = new MqttApplicationMessage();
                //message2.Topic = $"{_configuration.Topic}/{_configuration.DeviceKey}/observations";
                //message2.Payload = utf8;
                //_mqttClient.PublishAsync(message2);
            }

            return true;
        }

        public override bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            return true;
        }

        public override bool AddDevices(IEnumerable<IDevice> devices)
        {
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

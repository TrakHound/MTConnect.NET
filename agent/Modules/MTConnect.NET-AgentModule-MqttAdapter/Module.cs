// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Mqtt;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-adapter";
        private const string ModuleId = "MQTT Adapter";

        private readonly Logger _clientLogger = LogManager.GetLogger("mqtt-adapter-logger");
        private readonly ModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;

        private readonly string _server;
        private readonly int _port;
        private readonly int _qos;
        private readonly int _interval;
        private readonly int _retryInterval;
        private readonly string _username;
        private readonly string _password;
        private readonly string _clientId;
        private readonly string _caCertPath;
        private readonly string _pemClientCertPath;
        private readonly string _pemPrivateKeyPath;
        private readonly bool _allowUntrustedCertificates;
        private readonly bool _useTls;
        private readonly string _topic;
        private readonly string _deviceKey;

        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _mtconnectAgent = mtconnectAgent;
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += MessageReceived;

            _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
            if (_configuration != null)
            {
                _server = _configuration.Server;
                _port = _configuration.Port;
                _interval = _configuration.Interval;
                _retryInterval = _configuration.RetryInterval;
                _qos = _configuration.QoS;
                _username = _configuration.Username;
                _password = _configuration.Password;
                _clientId = _configuration.ClientId;
                _caCertPath = _configuration.CertificateAuthority;
                _pemClientCertPath = _configuration.PemCertificate;
                _pemPrivateKeyPath = _configuration.PemPrivateKey;
                _allowUntrustedCertificates = _configuration.AllowUntrustedCertificates;
                _useTls = _configuration.UseTls;
                _topic = _configuration.Topic;
                _deviceKey = _configuration.DeviceKey;
            }
        }


        protected override void OnStartAfterLoad()
        {
            if (_configuration != null)
            {
                _stop = new CancellationTokenSource();

                _ = Task.Run(Worker, _stop.Token);
            }
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
                        var clientOptionsBuilder = new MqttClientOptionsBuilder().WithTcpServer(_server, _port);

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_clientId))
                        {
                            clientOptionsBuilder.WithClientId(_clientId);
                        }

                        var certificates = new List<X509Certificate2>();

                        // Add CA (Certificate Authority)
                        if (!string.IsNullOrEmpty(_caCertPath))
                        {
                            certificates.Add(new X509Certificate2(GetFilePath(_caCertPath)));
                        }

                        // Add Client Certificate & Private Key
                        if (!string.IsNullOrEmpty(_pemClientCertPath) && !string.IsNullOrEmpty(_pemPrivateKeyPath))
                        {

#if NET5_0_OR_GREATER
                            certificates.Add(new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_pemClientCertPath), GetFilePath(_pemPrivateKeyPath)).Export(X509ContentType.Pfx)));
#else
                    throw new Exception("PEM Certificates Not Supported in .NET Framework 4.8 or older");
#endif

                            clientOptionsBuilder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                            {
                                UseTls = true,
                                SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                                IgnoreCertificateRevocationErrors = _allowUntrustedCertificates,
                                IgnoreCertificateChainErrors = _allowUntrustedCertificates,
                                AllowUntrustedCertificates = _allowUntrustedCertificates,
                                Certificates = certificates
                            });
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                        {
                            if (_useTls)
                            {
                                clientOptionsBuilder.WithCredentials(_username, _password).WithTls();
                            }
                            else
                            {
                                clientOptionsBuilder.WithCredentials(_username, _password);
                            }
                        }

                        // Build MQTT Client Options
                        var clientOptions = clientOptionsBuilder.Build();

                        // Connect to the MQTT Client
                        _mqttClient.ConnectAsync(clientOptions).Wait();


                        if (!string.IsNullOrEmpty(_topic))
                        {
                            await _mqttClient.SubscribeAsync($"{_topic}/#");
                        }
                        else
                        {
                            // ERROR ?
                        }


                        //ClientStarted?.Invoke(this, new EventArgs());

                        while (_mqttClient.IsConnected && !_stop.IsCancellationRequested)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        //if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    await Task.Delay(_retryInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    //InternalError?.Invoke(this, ex);
                }

            } while (!_stop.Token.IsCancellationRequested);


            try
            {
                // Disconnect from the MQTT Client
                if (_mqttClient != null) _mqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection).Wait();
            }
            catch { }
        }

        private async Task MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.Payload != null && args.ApplicationMessage.Payload.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                var dataModels = ProcessPayload(args.ApplicationMessage.Payload);
                if (!dataModels.IsNullOrEmpty())
                {
                    //Console.WriteLine(topic);
                    //Console.WriteLine(JsonSerializer.Serialize(dataModels));

                    foreach (var dataModel in dataModels)
                    {
                        _mtconnectAgent.AddObservations(_deviceKey, dataModel.ToObservationInputs());
                    }
                }

                //try
                //{
                //    //var json = System.Text.Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
                //    //if (!string.IsNullOrEmpty(json))
                //    //{
                //    //    //var dataModels = JsonSerializer.Deserialize<IEnumerable<DataModel>>(json);
                //    //    //if (!dataModels.IsNullOrEmpty())
                //    //    //{
                //    //    //    Console.WriteLine(topic);
                //    //    //    Console.WriteLine(JsonSerializer.Serialize(dataModels));

                //    //    //    foreach (var dataModel in dataModels)
                //    //    //    {
                //    //    //        _mtconnectAgent.AddObservations(_deviceKey, dataModel.ToObservationInputs());
                //    //    //    }
                //    //    //}
                //    //}
                //}
                //catch (Exception ex)
                //{

                //}
            }
        }

        public static IEnumerable<MTConnectMqttInputModel> ProcessPayload(byte[] payload)
        {
            if (!payload.IsNullOrEmpty())
            {
                try
                {
                    // Decompress from gzip
                    byte[] uncompressedBytes;
                    using (var inputStream = new MemoryStream(payload))
                    using (var outputStream = new MemoryStream())
                    using (var encodingStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                    {
                        encodingStream.CopyTo(outputStream);
                        uncompressedBytes = outputStream.ToArray();
                    }

                    if (!uncompressedBytes.IsNullOrEmpty())
                    {
                        // Convert JSON bytes to Mqtt PayloadModel
                        IEnumerable<MTConnectMqttInputModel> payloadModel = null;
                        using (var inputStream2 = new MemoryStream(uncompressedBytes))
                        {
                            payloadModel = JsonSerializer.Deserialize<IEnumerable<MTConnectMqttInputModel>>(inputStream2);
                        }

                        if (payloadModel != null)
                        {
                            return payloadModel;
                        }
                    }
                }
                catch (Exception ex)
                { 

                }
            }

            return null;
        }

        //public static byte[] CreatePayload(TrakHoundEntityCollection collection)
        //{
        //    if (collection != null)
        //    {
        //        try
        //        {
        //            // Convert to Mqtt Collection (serializable to Protobuf)
        //            var mqttCollection = new TrakHoundMqttEntityCollection(collection);

        //            // Convert to Protobuf
        //            byte[] protobufBytes;
        //            using (var inputStream1 = new MemoryStream())
        //            {
        //                Serializer.Serialize(inputStream1, mqttCollection);
        //                protobufBytes = inputStream1.ToArray();
        //            }

        //            // Compress to gzip
        //            byte[] compressedBytes;
        //            using (var inputStream2 = new MemoryStream())
        //            {
        //                using (var zip = new GZipStream(inputStream2, CompressionMode.Compress, true))
        //                {
        //                    zip.Write(protobufBytes, 0, protobufBytes.Length);
        //                }
        //                compressedBytes = inputStream2.ToArray();
        //            }

        //            return compressedBytes;
        //        }
        //        catch { }
        //    }

        //    return null;
        //}


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
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Formatters;
using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Modules
{
    public class Module : MTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-adapter";
        private const string ModuleId = "MQTT Adapter";

        private readonly ModuleConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _mtconnectAgent = mtconnectAgent;
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += MessageReceived;

            _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
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


                        if (!string.IsNullOrEmpty(_configuration.Topic))
                        {
                            await _mqttClient.SubscribeAsync($"{_configuration.Topic}/#");
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

                    await Task.Delay(_configuration.RetryInterval, _stop.Token);
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

        private Task MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.Payload != null && args.ApplicationMessage.Payload.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                var observations = ProcessPayload(args.ApplicationMessage.Payload);
                if (!observations.IsNullOrEmpty())
                {
                    _mtconnectAgent.AddObservations(_configuration.DeviceKey, observations);
                }
            }

            return Task.CompletedTask;
        }

        private IEnumerable<IObservationInput> ProcessPayload(byte[] payload)
        {
            if (!payload.IsNullOrEmpty())
            {
                try
                {
                    //// Decompress from gzip
                    //byte[] uncompressedBytes;
                    //using (var inputStream = new MemoryStream(payload))
                    //using (var outputStream = new MemoryStream())
                    //using (var encodingStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                    //{
                    //    encodingStream.CopyTo(outputStream);
                    //    uncompressedBytes = outputStream.ToArray();
                    //}

                    var uncompressedBytes = payload;
                    if (!uncompressedBytes.IsNullOrEmpty())
                    {
                        var readResult = InputFormatter.CreateObservations(_configuration.DocumentFormat, uncompressedBytes);
                        if (readResult.Success)
                        {
                            return readResult.Content;
                        }

                        //// Convert JSON bytes to Mqtt PayloadModel
                        //IEnumerable<MTConnectMqttInputObservations> payloadModel = null;
                        //using (var inputStream2 = new MemoryStream(uncompressedBytes))
                        //{
                        //    payloadModel = JsonSerializer.Deserialize<IEnumerable<MTConnectMqttInputObservations>>(inputStream2);
                        //}

                        //if (payloadModel != null)
                        //{
                        //    return payloadModel;
                        //}
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
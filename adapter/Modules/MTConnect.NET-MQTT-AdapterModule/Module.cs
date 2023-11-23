// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Adapters;
using MTConnect.Input;
using MTConnect.Mqtt;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace MTConnect
{
    public class Module : IMTConnectAdapterModule
    {
        public const string ConfigurationTypeId = "mqtt";

        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;
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


        public string Id { get; set; }

        public string Description { get; set; }


        public Module(string id, object configuration)
        {
            Id = id;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            _server = "localhost";
            _port = 1883;
            _topic = "OKUMA-Lathe";
            _deviceKey = "OKUMA-Lathe";
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);
        }

        public void Stop()
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


                        //if (!string.IsNullOrEmpty(_topic))
                        //{
                        //    await _mqttClient.SubscribeAsync(_topic);
                        //}
                        //else
                        //{
                        //    // ERROR ?
                        //}


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


        public bool WriteObservations(IEnumerable<IObservationInput> observations)
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
                Console.WriteLine(json);

                _mqttClient.PublishStringAsync($"{_topic}/observations", json);
            }

            return true;
        }

        public bool WriteConditionObservations(IEnumerable<IConditionObservationInput> conditionObservations)
        {
            return true;
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

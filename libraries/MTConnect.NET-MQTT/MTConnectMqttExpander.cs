// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Mqtt;
using MTConnect.Observations;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public class MTConnectMqttExpander : IDisposable
    {
        private const string _defaultTopicPrefix = "MTConnect";
        private const string _defaultDocumentFormat = "json";

        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly string _server;
        private readonly int _port;
        private readonly int _qos;
        private readonly int _interval;
        private readonly string _deviceUuid;
        private readonly string _topicPrefix;
        private readonly string _documentFormat;
        private readonly string _username;
        private readonly string _password;
        private readonly string _clientId;
        private readonly string _caCertPath;
        private readonly string _pemClientCertPath;
        private readonly string _pemPrivateKeyPath;
        private readonly bool _allowUntrustedCertificates;
        private readonly bool _useTls;

        private CancellationTokenSource _stop;
        private MTConnectMqttConnectionStatus _connectionStatus;

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int ReconnectionInterval { get; set; }

        public string Server => _server;

        public int Port => _port;

        public int QoS => _qos;

        public int Interval => _interval;

        public string TopicPrefix => _topicPrefix;

        public MTConnectMqttConnectionStatus ConnectionStatus => _connectionStatus;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler<MTConnectMqttConnectionStatus> ConnectionStatusChanged;

        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when any Response from the Client is received
        /// </summary>
        public event EventHandler ResponseReceived;

        /// <summary>
        /// Raised when the Client is Starting
        /// </summary>
        public event EventHandler ClientStarting;

        /// <summary>
        /// Raised when the Client is Started
        /// </summary>
        public event EventHandler ClientStarted;

        /// <summary>
        /// Raised when the Client is Stopping
        /// </summary>
        public event EventHandler ClientStopping;

        /// <summary>
        /// Raised when the Client is Stopeed
        /// </summary>
        public event EventHandler ClientStopped;


        public MTConnectMqttExpander(string server, int port = 1883, string topicPrefix = _defaultTopicPrefix, string documentFormat = _defaultDocumentFormat, string clientId = null, int qos = 1)
        {
            ReconnectionInterval = 10000;

            _server = server;
            _port = port;
            _topicPrefix = topicPrefix;
            _documentFormat = documentFormat;
            _clientId = clientId;
            _qos = qos;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            ClientStarting?.Invoke(this, new EventArgs());

            _ = Task.Run(Worker, _stop.Token);
        }

        public void Stop()
        {
            ClientStopping?.Invoke(this, new EventArgs());

            if (_stop != null) _stop.Cancel();
        }

        public void Dispose()
        {
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

                        clientOptionsBuilder.WithCleanSession(false);

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

                        //if (!string.IsNullOrEmpty(_deviceUuid))
                        //{
                        //    // Start protocol for a single Device
                        //    StartDeviceProtocol(_deviceUuid).Wait();
                        //}
                        //else
                        //{
                        //    // Start protocol for all devices
                        //    StartAllDevicesProtocol().Wait();
                        //}

                        ClientStarted?.Invoke(this, new EventArgs());

                        while (_mqttClient.IsConnected && !_stop.IsCancellationRequested)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    await Task.Delay(ReconnectionInterval, _stop.Token);
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


            ClientStopped?.Invoke(this, new EventArgs());
        }


        public async Task PublishDevice(IDevice device)
        {
            if (_mqttClient != null && _mqttClient.IsConnected && device != null)
            {
                var topic = $"{_topicPrefix}/Devices/{device.Uuid}/Device";

                var output = EntityFormatter.Format(_documentFormat, device);
                if (output != null)
                {
                    var message = new MqttApplicationMessage();
                    message.Topic = topic;
                    message.Payload = System.Text.Encoding.UTF8.GetBytes(output);
                    message.Retain = true;

                    var result = await _mqttClient.PublishAsync(message);
                    if (result.IsSuccess)
                    {

                    }
                    else
                    {

                    }
                }
            }
        }

        public async Task PublishCurrent(IEnumerable<IObservation> observations)
        {
            if (_mqttClient != null && _mqttClient.IsConnected && observations != null)
            {
                var deviceUuids = observations.Select(o => o.DeviceUuid).Distinct();
                foreach (var deviceUuid in deviceUuids)
                {
                    var dataItemIds = observations.Select(o => o.DataItemId).Distinct();
                    foreach (var dataItemId in dataItemIds)
                    {
                        var dataItemObservations = observations.Where(o => o.DataItemId == dataItemId);

                        var topic = $"{_topicPrefix}/Devices/{deviceUuid}/Observations/{dataItemId}/current";

                        var output = EntityFormatter.Format(_documentFormat, dataItemObservations);
                        if (output != null)
                        {
                            var message = new MqttApplicationMessage();
                            message.Topic = topic;
                            message.Payload = System.Text.Encoding.UTF8.GetBytes(output);
                            message.Retain = true;

                            var result = await _mqttClient.PublishAsync(message);
                            if (result.IsSuccess)
                            {

                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
        }

        public async Task PublishSample(IEnumerable<IObservation> observations)
        {
            if (_mqttClient != null && _mqttClient.IsConnected && observations != null)
            {
                var deviceUuids = observations.Select(o => o.DeviceUuid).Distinct();
                foreach (var deviceUuid in deviceUuids)
                {
                    var dataItemIds = observations.Select(o => o.DataItemId).Distinct();
                    foreach (var dataItemId in dataItemIds)
                    {
                        var dataItemObservations = observations.Where(o => o.DataItemId == dataItemId);

                        var topic = $"{_topicPrefix}/Devices/{deviceUuid}/Observations/{dataItemId}/sample";

                        var output = EntityFormatter.Format(_documentFormat, dataItemObservations);
                        if (output != null)
                        {
                            var message = new MqttApplicationMessage();
                            message.Topic = topic;
                            message.Payload = System.Text.Encoding.UTF8.GetBytes(output);
                            message.Retain = true;

                            var result = await _mqttClient.PublishAsync(message);
                            if (result.IsSuccess)
                            {

                            }
                            else
                            {

                            }
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
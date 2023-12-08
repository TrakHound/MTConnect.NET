// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Observations;
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
        private readonly IMTConnectMqttExpanderConfiguration _configuration;
        private readonly IMTConnectEntityClient _inputClient;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;
        private MTConnectMqttConnectionStatus _connectionStatus;

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int ReconnectionInterval { get; set; }

        public string Server => _configuration.Server;

        public int Port => _configuration.Port;

        public int QoS => _configuration.QoS;

        public int Interval => _configuration.Interval;

        public string TopicPrefix => _configuration.TopicPrefix;

        public string ExpandedTopicPrefix => _configuration.ExpandedTopicPrefix;

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


        public MTConnectMqttExpander(IMTConnectMqttExpanderConfiguration configuration, IMTConnectEntityClient inputClient)
        {
            ReconnectionInterval = 10000;

            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttExpanderConfiguration();

            _inputClient = inputClient;
            if (_inputClient != null)
            {
                _inputClient.DeviceReceived += DeviceReceived;
                _inputClient.ObservationReceived += ObservationReceived;
                _inputClient.AssetReceived += AssetReceived;
            }

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
                        var clientOptionsBuilder = new MqttClientOptionsBuilder().WithTcpServer(_configuration.Server, _configuration.Port);

                        clientOptionsBuilder.WithCleanSession(false);

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


        public async void DeviceReceived(object sender, IDevice device)
        {
            await PublishDevice(device);
        }

        public async Task PublishDevice(IDevice device)
        {
            if (_mqttClient != null && _mqttClient.IsConnected && device != null)
            {
                var topic = $"{_configuration.TopicPrefix}/Devices/{device.Uuid}/Device";

                var formatResponse = EntityFormatter.Format(_configuration.DocumentFormat, device);
                if (formatResponse.Success && formatResponse.Content != null)
                {
                    var message = new MqttApplicationMessage();
                    message.Topic = topic;
                    message.Payload = formatResponse.Content;
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


        public async void ObservationReceived(object sender, IObservation observation)
        {
            await PublishObservation(observation);
        }

        public async Task PublishObservation(IObservation observation)
        {
            //if (_mqttClient != null && _mqttClient.IsConnected && observation != null)
            //{
            //    var topic = $"{_configuration.TopicPrefix}/Devices/{device.Uuid}/Device";

            //    var output = EntityFormatter.Format(_configuration.DocumentFormat, device);
            //    if (output != null)
            //    {
            //        var message = new MqttApplicationMessage();
            //        message.Topic = topic;
            //        message.Payload = System.Text.Encoding.UTF8.GetBytes(output);
            //        message.Retain = true;

            //        var result = await _mqttClient.PublishAsync(message);
            //        if (result.IsSuccess)
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }
            //}
        }


        public async void AssetReceived(object sender, IAsset asset)
        {

        }

        public async Task PublishAsset(IAsset asset)
        {
            //if (_mqttClient != null && _mqttClient.IsConnected && asset != null)
            //{
            //    var topic = $"{_configuration.TopicPrefix}/Devices/{device.Uuid}/Device";

            //    var output = EntityFormatter.Format(_configuration.DocumentFormat, device);
            //    if (output != null)
            //    {
            //        var message = new MqttApplicationMessage();
            //        message.Topic = topic;
            //        message.Payload = System.Text.Encoding.UTF8.GetBytes(output);
            //        message.Retain = true;

            //        var result = await _mqttClient.PublishAsync(message);
            //        if (result.IsSuccess)
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }
            //}
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
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttRelay : IDisposable
    {
        private const string _documentFormat = "JSON";


        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly IMTConnectMqttClientConfiguration _configuration;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly object _lock = new object();
        private CancellationTokenSource _stop;

        private readonly List<int> _observationIntervals = new List<int>();
        private readonly List<System.Timers.Timer> _observationIntervalTimers = new List<System.Timers.Timer>();
        private readonly Dictionary<int, Dictionary<string, IObservation>> _observationBuffers = new Dictionary<int, Dictionary<string, IObservation>>();

        private readonly int _heartbeatInterval;
        private readonly System.Timers.Timer _heartbeatTimer = new System.Timers.Timer();


        public string Server => _configuration.Server;

        public int Port => _configuration.Port;

        public int QoS => _configuration.QoS;

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int RetryInterval => _configuration.RetryInterval;

        public int HeartbeatInterval => _heartbeatInterval;

        public MTConnectMqttFormat Format { get; set; }

        public string TopicPrefix => _configuration.TopicPrefix;

        public bool RetainMessages { get; set; }

        public bool AllowUntrustedCertificates => _configuration.AllowUntrustedCertificates;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler<string> MessageSent;

        public event EventHandler<Exception> ConnectionError;

        public event EventHandler<Exception> PublishError;


        public MTConnectMqttRelay(IMTConnectAgent mtconnectAgent, IMTConnectMqttClientConfiguration configuration, IEnumerable<int> observationIntervals = null, int heartbeatInterval = 1000)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.ObservationAdded += ObservationAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;

            // Set Configuration (MQTT client to external broker)
            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttClientConfiguration();

            // Set Observation Intervals
            if (!observationIntervals.IsNullOrEmpty())
            {
                _observationIntervals.AddRange(observationIntervals);
            }

            Format = MTConnectMqttFormat.Hierarchy;
            RetainMessages = true;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            // Set Heartbeat Timer
            _heartbeatInterval = heartbeatInterval;
            _heartbeatTimer.Interval = _heartbeatInterval;
            _heartbeatTimer.Elapsed += HeartbeatTimerElapsed;
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);

            _heartbeatTimer.Start();

            if (!_observationIntervals.IsNullOrEmpty())
            {
                var timerIntervals = _observationIntervals.Where(o => o > 0);
                if (!timerIntervals.IsNullOrEmpty())
                {
                    foreach (var interval in timerIntervals)
                    {
                        var timer = new System.Timers.Timer();
                        timer.Interval = interval;
                        timer.Elapsed += ObservationIntervalTimerElapsed;
                        lock (_lock) _observationIntervalTimers.Add(timer);

                        timer.Start();
                    }
                }
            }
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

            if (_heartbeatTimer != null) _heartbeatTimer.Stop();

            if (!_observationIntervalTimers.IsNullOrEmpty())
            {
                foreach (var timer in _observationIntervalTimers) timer.Dispose();
                _observationIntervalTimers.Clear();
            }

            try
            {
                if (_mqttClient != null)
                {
                    _mqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection);
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

                            clientOptionsBuilder.WithCleanSession();
                            clientOptionsBuilder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                            {
                                UseTls = true,
                                SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                                IgnoreCertificateRevocationErrors = AllowUntrustedCertificates,
                                IgnoreCertificateChainErrors = AllowUntrustedCertificates,
                                AllowUntrustedCertificates = AllowUntrustedCertificates,
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

                        if (Connected != null) Connected.Invoke(this, new EventArgs());

                        // Publish MTConnect Agent Information
                        await PublishAgent(_mtconnectAgent);

                        // Add Agent Devices
                        var devices = _mtconnectAgent.GetDevices();
                        if (!devices.IsNullOrEmpty())
                        {
                            foreach (var device in devices)
                            {
                                await PublishDevice(device);
                            }
                        }

                        // Add Current Observations (to Initialize each DataItem on the MQTT broker)
                        var observations = _mtconnectAgent.GetCurrentObservations();
                        if (!observations.IsNullOrEmpty())
                        {
                            foreach (var observationOutput in observations)
                            {
                                var observation = Observation.Create(observationOutput.DataItem);
                                observation.DeviceUuid = observationOutput.DeviceUuid;
                                observation.DataItem = observationOutput.DataItem;
                                observation.InstanceId = observationOutput.InstanceId;
                                observation.Timestamp = observationOutput.Timestamp;
                                observation.AddValues(observationOutput.Values);

                                await PublishObservation(observation);
                            }
                        }

                        while (!_stop.Token.IsCancellationRequested && _mqttClient.IsConnected)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    if (Disconnected != null) Disconnected.Invoke(this, new EventArgs());

                    await Task.Delay(RetryInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { }

            } while (!_stop.Token.IsCancellationRequested);
        }

        public void Dispose()
        {
            if (_mqttClient != null) _mqttClient.Dispose();
        }


        private async void DeviceAdded(object sender, IDevice device)
        {
            await PublishDevice(device);
        }

        private async void ObservationAdded(object sender, IObservation observation)
        {
            await PublishObservation(observation);
        }

        private async void AssetAdded(object sender, IAsset asset)
        {
            await PublishAsset(asset);
        }


        private async Task PublishAgent(IMTConnectAgent agent)
        {
            var messages = MTConnectMqttMessage.Create(agent, _observationIntervals, _heartbeatInterval, RetainMessages);
            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    if (message != null && message.Payload != null)
                    {
                        await Publish(message);
                    }
                }
            }
        }

        private async Task PublishDevice(IDevice device)
        {
            var messages = MTConnectMqttMessage.Create(device, _mtconnectAgent.Uuid, _documentFormat, RetainMessages);
            if (!messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    if (message != null && message.Payload != null)
                    {
                        await Publish(message);
                    }
                }
            }
        }

        private async Task PublishObservation(IObservation observation)
        {
            if (!_observationIntervals.IsNullOrEmpty())
            {
                foreach (var interval in _observationIntervals)
                {
                    if (interval > 0)
                    {
                        var bufferKey = CreateBufferKey(observation.DeviceUuid, observation.DataItemId, interval);
                        if (!string.IsNullOrEmpty(bufferKey))
                        {
                            lock (_lock)
                            {
                                _observationBuffers.TryGetValue(interval, out var buffer);
                                if (buffer == null)
                                {
                                    buffer = new Dictionary<string, IObservation>();
                                    _observationBuffers.Add(interval, buffer);
                                }

                                buffer.Remove(bufferKey);
                                buffer.Add(bufferKey, observation);
                            }
                        }
                    }
                    else
                    {
                        await PublishObservation(observation, 0);
                    }
                }
            }
            else
            {
                await PublishObservation(observation, 0);
            }
        }

        private async Task PublishObservation(IObservation observation, int interval)
        {
            if (observation.Category != Devices.DataItemCategory.CONDITION)
            {
                var message = MTConnectMqttMessage.Create(observation, Format, _documentFormat, RetainMessages, interval);
                if (message != null && message.Payload != null) await Publish(message);
            }
            else
            {
                var observations = _mtconnectAgent.GetCurrentObservations(observation.DeviceUuid);
                if (!observations.IsNullOrEmpty())
                {
                    var dataItemObservations = observations.Where(o => o.DataItemId == observation.DataItemId);
                    if (!dataItemObservations.IsNullOrEmpty())
                    {
                        var x = new List<IObservation>();
                        foreach (var dataItemObservation in dataItemObservations)
                        {
                            var y = Observation.Create(dataItemObservation.DataItem);
                            y.DeviceUuid = dataItemObservation.DeviceUuid;
                            y.DataItem = dataItemObservation.DataItem;
                            y.InstanceId = dataItemObservation.InstanceId;
                            y.Timestamp = dataItemObservation.Timestamp;
                            y.AddValues(dataItemObservation.Values);
                            x.Add(y);
                        }

                        var message = MTConnectMqttMessage.Create(x, Format, _documentFormat, RetainMessages, interval);
                        if (message != null && message.Payload != null) await Publish(message);
                    }
                }
            }
        }

        private async Task PublishAsset(IAsset asset)
        {
            var messages = MTConnectMqttMessage.Create(asset, _documentFormat, RetainMessages);
            await Publish(messages);
        }


        private async Task Publish(MqttApplicationMessage message)
        {
            try
            {
                if (_mqttClient != null && _mqttClient.IsConnected)
                {
                    // Set the Topic Prefix
                    if (!string.IsNullOrEmpty(TopicPrefix)) message.Topic = $"{TopicPrefix}/{message.Topic}";

                    // Set QoS for Message
                    message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS;

                    await _mqttClient.PublishAsync(message);
                }
            }
            catch (Exception ex)
            {
                if (PublishError != null) PublishError.Invoke(this, ex);
            }
        }

        private async Task Publish(IEnumerable<MqttApplicationMessage> messages)
        {
            try
            {
                if (_mqttClient != null && !messages.IsNullOrEmpty())
                {
                    foreach (var message in messages)
                    {
                        // Set the Topic Prefix
                        if (!string.IsNullOrEmpty(TopicPrefix)) message.Topic = $"{TopicPrefix}/{message.Topic}";

                        // Set QoS for Message
                        message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS;

                        await _mqttClient.PublishAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (PublishError != null) PublishError.Invoke(this, ex);
            }
        }


        private async void HeartbeatTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await Publish(MTConnectMqttMessage.CreateHeartbeat(_mtconnectAgent, UnixDateTime.Now));
        }

        private async void ObservationIntervalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (sender != null)
            {
                var timer = (System.Timers.Timer)sender;
                var interval = (int)timer.Interval;

                Dictionary<string, IObservation> buffer;
                lock (_lock)
                {
                    _observationBuffers.TryGetValue(interval, out buffer);
                    _observationBuffers.Remove(interval);
                }

                if (!buffer.IsNullOrEmpty())
                {
                    foreach (var observation in buffer.Values)
                    {
                        await PublishObservation(observation, interval);
                    }
                }
            }
        }



        private static string CreateBufferKey(string deviceUuid, string dataItemId, int interval)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(dataItemId) && interval > 0)
            {
                return $"{deviceUuid}::{dataItemId}::{interval}";
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
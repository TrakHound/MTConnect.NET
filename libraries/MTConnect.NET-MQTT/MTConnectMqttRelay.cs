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
    /// <summary>
    /// Connects an in-process <see cref="IMTConnectAgent"/> to a remote MQTT broker, forwarding
    /// device, observation, and asset additions as publishes under the configured topic prefix.
    /// Unlike <see cref="MTConnectMqttBroker"/> the relay does not host its own broker; it owns
    /// an <see cref="IMqttClient"/> and reconnects with exponential back-off (controlled by
    /// <see cref="RetryInterval"/>) if the broker drops. The relay also drives a heartbeat timer
    /// and optional observation-interval buckets so consumers can subscribe to coarse-grained
    /// snapshots in addition to per-observation publishes.
    /// </summary>
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


        /// <summary>The remote MQTT broker hostname or IP the relay connects to.</summary>
        public string Server => _configuration.Server;

        /// <summary>The remote MQTT broker TCP port the relay connects to.</summary>
        public int Port => _configuration.Port;

        /// <summary>MQTT Quality of Service level applied to every publish from the relay.</summary>
        public int Qos => _configuration.Qos;

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int RetryInterval => _configuration.RetryInterval;

        /// <summary>Heartbeat publish cadence in milliseconds; the relay emits the agent's <c>HeartbeatTimestamp</c> publish at this rate.</summary>
        public int HeartbeatInterval => _heartbeatInterval;

        /// <summary>Topic layout (flat or hierarchy) used for observation publishes; defaults to <see cref="MTConnectMqttFormat.Hierarchy"/>.</summary>
        public MTConnectMqttFormat Format { get; set; }

        /// <summary>Topic prefix used for all publishes (sourced from the client configuration).</summary>
        public string TopicPrefix => _configuration.TopicPrefix;

        /// <summary>When true, publishes carry the MQTT retain flag so consumers see the most recent values on connect.</summary>
        public bool RetainMessages { get; set; }

        /// <summary>When true, broker TLS certificates that fail validation are still accepted; sourced from the client configuration.</summary>
        public bool AllowUntrustedCertificates => _configuration.AllowUntrustedCertificates;

        /// <summary>Raised once the relay establishes its session with the remote broker.</summary>
        public event EventHandler Connected;

        /// <summary>Raised when the broker session is dropped (either by the relay, the broker, or a transport failure).</summary>
        public event EventHandler Disconnected;

        #pragma warning disable CS0067 // event is part of the public API surface, raised by subclasses
        /// <summary>Raised after each successful publish; the argument is the topic name.</summary>
        public event EventHandler<string> MessageSent;
        #pragma warning restore CS0067

        /// <summary>Raised when establishing or maintaining the broker session throws.</summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>Raised when an individual publish throws after the session has been established.</summary>
        public event EventHandler<Exception> PublishError;


        /// <summary>
        /// Wires the relay to <paramref name="mtconnectAgent"/>'s add events and prepares the
        /// MQTT client and heartbeat/observation-interval timers. Construction is non-blocking;
        /// the broker connection is opened on <see cref="Start"/>.
        /// </summary>
        /// <param name="mtconnectAgent">The agent whose additions are forwarded to MQTT.</param>
        /// <param name="configuration">Broker connection settings; null falls back to a fresh <see cref="MTConnectMqttClientConfiguration"/>.</param>
        /// <param name="observationIntervals">Optional list of interval-bucket lengths in milliseconds for batched observation publishes.</param>
        /// <param name="heartbeatInterval">Heartbeat publish cadence in milliseconds; defaults to one second.</param>
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


        /// <summary>
        /// Opens the broker session on a background task, starts the heartbeat timer, and
        /// schedules any observation-interval bucket timers. The method returns immediately;
        /// <see cref="Connected"/> is raised when the broker session is up.
        /// </summary>
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

        /// <summary>
        /// Cancels the relay worker, stops the heartbeat timer, disposes the observation-interval
        /// bucket timers, and tears down the broker session. <see cref="Disconnected"/> is raised
        /// once the session has been closed.
        /// </summary>
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
                    _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
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

                            clientOptionsBuilder.WithCleanSession();
                            clientOptionsBuilder.WithTlsOptions(b => b
                                .WithSslProtocols(System.Security.Authentication.SslProtocols.Tls12)
                                .WithIgnoreCertificateRevocationErrors(AllowUntrustedCertificates)
                                .WithIgnoreCertificateChainErrors(AllowUntrustedCertificates)
                                .WithAllowUntrustedCertificates(AllowUntrustedCertificates)
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
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);
        }

        /// <summary>Disposes the underlying <see cref="IMqttClient"/>. Call <see cref="Stop"/> first to release the heartbeat and bucket timers.</summary>
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
                    if (message != null && message.HasPayload())
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
                    if (message != null && message.HasPayload())
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
                if (message != null && message.HasPayload()) await Publish(message);
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
                        if (message != null && message.HasPayload()) await Publish(message);
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

                    // Set Qos for Message
                    message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)Qos;

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

                        // Set Qos for Message
                        message.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)Qos;

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
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Formatters;
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
    /// <summary>
    /// Client that implements the full MTConnect MQTT Protocol (Probe, Current, Sample, and Assets)
    /// </summary>
    public class MTConnectMqttClient : IMTConnectClient, IMTConnectEntityClient, IDisposable
    {
        private const string _defaultTopicPrefix = "MTConnect";
        private const string _defaultDocumentFormat = "json-cppagent-mqtt";

        private const string _defaultProbeTopicPrefix = "Probe";
        private const string _defaultCurrentTopicPrefix = "Current";
        private const string _defaultSampleTopicPrefix = "Sample";
        private const string _defaultAssetTopicPrefix = "Asset";

        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly IMTConnectMqttClientConfiguration _configuration;
        private readonly string _documentFormat;

        private readonly Dictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();
        private readonly Dictionary<string, ulong> _deviceLastSequence = new Dictionary<string, ulong>();
        private readonly Dictionary<string, ulong> _deviceLastCurrentSequence = new Dictionary<string, ulong>();
        private readonly Dictionary<string, ulong> _deviceInstanceId = new Dictionary<string, ulong>();
        private readonly object _lock = new object();


        private CancellationTokenSource _stop;
        private MTConnectMqttConnectionStatus _connectionStatus;
        private long _lastResponse;


        public delegate void MTConnectMqttEventHandler(string topic, byte[] payload);

        public delegate void MTConnectMqttEventHandler<T>(string deviceUuid, T item);

        /// <summary>
        /// Gets the Client Configuration
        /// </summary>
        public IMTConnectMqttClientConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent
        /// </summary>
        public long LastResponse => _lastResponse;

        /// <summary>
        /// Gets the status of the connection to the MQTT broker
        /// </summary>
        public MTConnectMqttConnectionStatus ConnectionStatus => _connectionStatus;

        /// <summary>
        /// Raised when the connection to the MQTT broker is established
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Raised when the connection to the MQTT broker is disconnected 
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Raised when the status of the connection to the MQTT broker has changed
        /// </summary>
        public event EventHandler<MTConnectMqttConnectionStatus> ConnectionStatusChanged;

        /// <summary>
        /// Raised when an error occurs during connection to the MQTT broker
        /// </summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public event EventHandler<Exception> InternalError;

        /// <summary>
        /// Raised when a Device is received
        /// </summary>
        public event EventHandler<IDevice> DeviceReceived;

        /// <summary>
        /// Raised when an Observation is received
        /// </summary>
        public event EventHandler<IObservation> ObservationReceived;

        /// <summary>
        /// Raised when an Asset is received
        /// </summary>
        public event EventHandler<IAsset> AssetReceived;

        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        public event EventHandler<IDevicesResponseDocument> ProbeReceived;

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        public event EventHandler<IStreamsResponseDocument> CurrentReceived;

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        public event EventHandler<IStreamsResponseDocument> SampleReceived;

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        public event EventHandler<IAssetsResponseDocument> AssetsReceived;

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public event EventHandler<IErrorResponseDocument> MTConnectError;

        /// <summary>
        /// Raised when any MQTT Message is received
        /// </summary>
        public event MTConnectMqttEventHandler MessageReceived;

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


        /// <summary>
        /// Initializes a new instance of the MTConnectMqttClient class that is used to perform
        /// the full protocol from an MTConnect Agent using the MTConnect MQTT Api protocol
        /// </summary>
        public MTConnectMqttClient(string server, int port = 1883, string deviceUuid = null, string topicPrefix = _defaultTopicPrefix, string documentFormat = _defaultDocumentFormat, string clientId = null, int qos = 1)
        {
            var configuration = new MTConnectMqttClientConfiguration();
            configuration.Server = server;
            configuration.Port = port;
            configuration.DeviceUuid = deviceUuid;
            configuration.TopicPrefix = topicPrefix;
            configuration.ClientId = clientId;
            configuration.QoS = qos;
            _configuration = configuration;
            _documentFormat = documentFormat;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += ProcessMessage;
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectMqttClient class that is used to perform
        /// the full protocol from an MTConnect Agent using the MTConnect MQTT Api protocol
        /// </summary>
        public MTConnectMqttClient(IMTConnectMqttClientConfiguration configuration, string documentFormat = _defaultDocumentFormat)
        {
            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttClientConfiguration();
            _documentFormat = documentFormat;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += ProcessMessage;
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

                        if (_configuration.Tls != null)
                        {
                            var certificateResults = _configuration.Tls.GetCertificate();
                            if (certificateResults.Success && certificateResults.Certificate != null)
                            {
                                var certificateAuthorityResults = _configuration.Tls.GetCertificateAuthority();

                                var certificates = new List<X509Certificate2>();
                                if (certificateAuthorityResults.Certificate != null)
                                {
                                    certificates.Add(certificateAuthorityResults.Certificate);
                                }
                                certificates.Add(certificateResults.Certificate);

                                var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();

                                // Set Client Certificate
                                tlsOptionsBuilder.WithClientCertificates(certificates);

                                // Set VerifyClientCertificate option
                                tlsOptionsBuilder.WithAllowUntrustedCertificates(!_configuration.Tls.VerifyClientCertificate);

#if NET5_0_OR_GREATER
                                if (certificateAuthorityResults.Certificate != null)
                                {
                                    tlsOptionsBuilder.WithCertificateValidationHandler((certContext) =>
                                    {
                                        var chain = new X509Chain();
                                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                                        chain.ChainPolicy.VerificationTime = DateTime.Now;
                                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
                                        chain.ChainPolicy.CustomTrustStore.Add(certificateAuthorityResults.Certificate);

                                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

                                        // convert provided X509Certificate to X509Certificate2
                                        var x5092 = new X509Certificate2(certContext.Certificate);

                                        return chain.Build(x5092);
                                    });
                                }
#endif

                                clientOptionsBuilder.WithTlsOptions(tlsOptionsBuilder.Build());
                            }
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();
                                tlsOptionsBuilder.WithSslProtocols(System.Security.Authentication.SslProtocols.Tls12);
                                clientOptionsBuilder.WithTlsOptions(tlsOptionsBuilder.Build());

                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
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

                        if (!string.IsNullOrEmpty(_configuration.DeviceUuid))
                        {
                            // Start protocol for a single Device
                            await StartDeviceProtocol(_configuration.DeviceUuid);
                        }
                        else
                        {
                            // Start protocol for all devices
                            await StartAllDevicesProtocol();
                        }

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

                    await Task.Delay(_configuration.RetryInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    InternalError?.Invoke(this, ex);
                }

            } while (!_stop.Token.IsCancellationRequested);


            try
            {
                // Disconnect from the MQTT Client
                if (_mqttClient != null) await _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
            }
            catch { }


            ClientStopped?.Invoke(this, new EventArgs());
        }


        private async Task StartAllDevicesProtocol()
        {
            await _mqttClient.SubscribeAsync("MTConnect/Probe/#");
            await _mqttClient.SubscribeAsync("MTConnect/Current/#");
            await _mqttClient.SubscribeAsync("MTConnect/Sample/#");
            await _mqttClient.SubscribeAsync("MTConnect/Asset/#");
        }

        private async Task StartDeviceProtocol(string deviceUuid)
        {
            await _mqttClient.SubscribeAsync($"MTConnect/Probe/{deviceUuid}");
            await _mqttClient.SubscribeAsync($"MTConnect/Current/{deviceUuid}");
            await _mqttClient.SubscribeAsync($"MTConnect/Sample/{deviceUuid}");
            await _mqttClient.SubscribeAsync($"MTConnect/Asset/{deviceUuid}/#");
        }


        private Task ProcessMessage(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.PayloadSegment != null && args.ApplicationMessage.PayloadSegment.Array != null && args.ApplicationMessage.PayloadSegment.Array.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                if (MessageReceived != null) MessageReceived.Invoke(topic, args.ApplicationMessage.PayloadSegment.Array);

                if (IsSampleTopic(topic))
                {
                    ProcessSampleMessage(args.ApplicationMessage);
                }
                else if (IsCurrentTopic(topic))
                {
                    ProcessCurrentMessage(args.ApplicationMessage);
                }
                else if (IsAssetTopic(topic))
                {
                    ProcessAssetMessage(args.ApplicationMessage);
                }
                else if (IsProbeTopic(topic))
                {
                    ProcessProbeMessage(args.ApplicationMessage);
                }
            }

            return Task.CompletedTask;
        }


        private bool IsProbeTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_configuration.TopicPrefix}/{_defaultProbeTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }

        private bool IsCurrentTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_configuration.TopicPrefix}/{_defaultCurrentTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }

        private bool IsSampleTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_configuration.TopicPrefix}/{_defaultSampleTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }

        private bool IsAssetTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_configuration.TopicPrefix}/{_defaultAssetTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }


        private void ProcessProbeMessage(MqttApplicationMessage message)
        {
            var result = EntityFormatter.CreateDevice(_documentFormat, new MemoryStream(message.Payload));
            if (result.Success)
            {
                var device = result.Content;
                if (device != null && device.Uuid != null)
                {
                    // Add to cached list
                    lock (_lock)
                    {
                        _devices.Remove(device.Uuid);
                        _devices.Add(device.Uuid, device);
                    }

                    DeviceReceived?.Invoke(this, device);
                }
            }
        }

        private void ProcessCurrentMessage(MqttApplicationMessage message)
        {
            if (!message.Retain)
            {
                var result = ResponseDocumentFormatter.CreateStreamsResponseDocument(_documentFormat, new MemoryStream(message.Payload));
                if (result.Success)
                {
                    ProcessCurrentDocument(result.Content);
                }
            }
        }

        private void ProcessSampleMessage(MqttApplicationMessage message)
        {
            if (!message.Retain)
            {
                var result = ResponseDocumentFormatter.CreateStreamsResponseDocument(_documentFormat, new MemoryStream(message.Payload));
                if (result.Success)
                {
                    ProcessSampleDocument(result.Content);
                }
            }
        }

        private void ProcessAssetMessage(MqttApplicationMessage message)
        {
            var result = ResponseDocumentFormatter.CreateAssetsResponseDocument(_documentFormat, new MemoryStream(message.Payload));
            if (result.Success)
            {
                ProcessAssetsDocument(result.Content);
            }
        }


        private void ProcessCurrentDocument(IStreamsResponseDocument document)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                if (!document.Streams.IsNullOrEmpty())
                {
                    IDeviceStream deviceStream = null;

                    // Get the DeviceStream for the Device or default to the first
                    if (!string.IsNullOrEmpty(_configuration.DeviceUuid)) deviceStream = document.Streams.FirstOrDefault(o => o.Uuid == _configuration.DeviceUuid);
                    else deviceStream = document.Streams.FirstOrDefault();

                    var observations = deviceStream.Observations;
                    if (deviceStream != null && deviceStream.Uuid != null && !observations.IsNullOrEmpty())
                    {
                        ulong lastSequence;
                        ulong lastInstanceId;
                        lock (_lock)
                        {
                            _deviceLastSequence.TryGetValue(deviceStream.Uuid, out lastSequence);
                            _deviceInstanceId.TryGetValue(deviceStream.Uuid, out lastInstanceId);
                        }

                        // Recreate Response Document (to set DataItem property for Observations)
                        var response = new StreamsResponseDocument();
                        response.Header = document.Header;

                        if (lastInstanceId < 1 || response.Header.InstanceId != lastInstanceId)
                        {
                            var deviceStreams = new List<IDeviceStream>();
                            foreach (var stream in document.Streams)
                            {
                                deviceStreams.Add(ProcessDeviceStream(stream));
                            }
                            response.Streams = deviceStreams;

                            //CheckAssetChanged(deviceStream.Observations, cancel);

                            CurrentReceived?.Invoke(this, response);

                            observations = response.GetObservations();
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    if (observation.Sequence > lastSequence)
                                    {
                                        ObservationReceived?.Invoke(this, observation);
                                    }
                                }

                                var maxSequence = observations.Max(o => o.Sequence);

                                // Save the most recent Sequence that was read
                                lock (_lock)
                                {
                                    _deviceLastCurrentSequence.Remove(deviceStream.Uuid);
                                    _deviceLastCurrentSequence.Add(deviceStream.Uuid, maxSequence);

                                    _deviceLastSequence.Remove(deviceStream.Uuid);
                                    _deviceLastSequence.Add(deviceStream.Uuid, maxSequence);

                                    _deviceInstanceId.Remove(deviceStream.Uuid);
                                    _deviceInstanceId.Add(deviceStream.Uuid, response.Header.InstanceId);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ProcessSampleDocument(IStreamsResponseDocument document)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                // Set Agent Instance ID
                //if (document.Header != null) _lastInstanceId = document.Header.InstanceId;

                if (!document.Streams.IsNullOrEmpty())
                {
                    IDeviceStream deviceStream = null;

                    // Get the DeviceStream for the Device or default to the first
                    if (!string.IsNullOrEmpty(_configuration.DeviceUuid)) deviceStream = document.Streams.FirstOrDefault(o => o.Uuid == _configuration.DeviceUuid);
                    else deviceStream = document.Streams.FirstOrDefault();

                    if (deviceStream != null && deviceStream.Observations != null && deviceStream.Observations.Count() > 0)
                    {
                        ulong lastCurrentSequence;
                        ulong lastSequence;
                        lock (_lock)
                        {
                            _deviceLastCurrentSequence.TryGetValue(deviceStream.Uuid, out lastCurrentSequence);
                            _deviceLastSequence.TryGetValue(deviceStream.Uuid, out lastSequence);
                        }

                        if (lastCurrentSequence > 0)
                        {
                            // Recreate Response Document (to set DataItem property for Observations)
                            var response = new StreamsResponseDocument();
                            response.Header = document.Header;

                            var deviceStreams = new List<IDeviceStream>();
                            foreach (var stream in document.Streams)
                            {
                                deviceStreams.Add(ProcessDeviceStream(stream));
                            }
                            response.Streams = deviceStreams;

                            //CheckAssetChanged(deviceStream.Observations, cancel);

                            SampleReceived?.Invoke(this, response);

                            var observations = response.GetObservations();
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    if (observation.Sequence > lastSequence && observation.Sequence > lastCurrentSequence)
                                    {
                                        ObservationReceived?.Invoke(this, observation);
                                    }
                                }

                                // Save the most recent Sequence that was read
                                var maxSequence = observations.Max(o => o.Sequence);

                                // Save the most recent Sequence that was read
                                lock (_lock)
                                {
                                    _deviceLastSequence.Remove(deviceStream.Uuid);
                                    _deviceLastSequence.Add(deviceStream.Uuid, maxSequence);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ProcessAssetsDocument(IAssetsResponseDocument document)
        {
            if (document != null && !document.Assets.IsNullOrEmpty())
            {
                AssetsReceived?.Invoke(this, document);

                foreach (var asset in document.Assets)
                {
                    AssetReceived?.Invoke(this, asset);
                }
            }
        }


        private IDeviceStream ProcessDeviceStream(IDeviceStream inputDeviceStream)
        {
            var outputDeviceStream = new DeviceStream();
            outputDeviceStream.Name = inputDeviceStream.Name;
            outputDeviceStream.Uuid = inputDeviceStream.Uuid;

            var componentStreams = new List<IComponentStream>();
            if (!inputDeviceStream.ComponentStreams.IsNullOrEmpty())
            {
                foreach (var componentStream in inputDeviceStream.ComponentStreams)
                {
                    componentStreams.Add(ProcessComponentStream(outputDeviceStream.Uuid, componentStream));
                }
            }
            outputDeviceStream.ComponentStreams = componentStreams;

            return outputDeviceStream;
        }

        private IComponentStream ProcessComponentStream(string deviceUuid, IComponentStream inputComponentStream)
        {
            var outputComponentStream = new ComponentStream();
            outputComponentStream.Name = inputComponentStream.Name;
            outputComponentStream.NativeName = inputComponentStream.NativeName;
            outputComponentStream.Uuid = inputComponentStream.Uuid;
            outputComponentStream.Component = GetCachedComponent(deviceUuid, inputComponentStream.ComponentId);

            var observations = new List<IObservation>();
            if (!inputComponentStream.Observations.IsNullOrEmpty())
            {
                foreach (var inputObservation in inputComponentStream.Observations)
                {
                    var dataItem = GetCachedDataItem(deviceUuid, inputObservation.DataItemId);
                    if (dataItem != null)
                    {
                        var outputObservation = Observation.Create(dataItem);
                        outputObservation.DeviceUuid = deviceUuid;
                        outputObservation.DataItemId = inputObservation.DataItemId;
                        outputObservation.DataItem = GetCachedDataItem(deviceUuid, inputObservation.DataItemId);
                        outputObservation.CompositionId = inputObservation.CompositionId;
                        outputObservation.Category = inputObservation.Category;
                        outputObservation.Representation = inputObservation.Representation;
                        outputObservation.Type = inputObservation.Type;
                        outputObservation.SubType = inputObservation.SubType;
                        outputObservation.Name = inputObservation.Name;
                        outputObservation.Sequence = inputObservation.Sequence;
                        outputObservation.Timestamp = inputObservation.Timestamp;
                        outputObservation.AddValues(inputObservation.Values);
                        observations.Add(outputObservation);
                    }
                }
            }
            outputComponentStream.Observations = observations;

            return outputComponentStream;
        }


        private IDevice GetCachedDevice(string deviceUuid)
        {
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                lock (_lock)
                {
                    _devices.TryGetValue(deviceUuid, out var device);
                    return device;
                }
            }

            return null;
        }

        private IComponent GetCachedComponent(string deviceUuid, string componentId)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(componentId))
            {
                lock (_lock)
                {
                    _devices.TryGetValue(deviceUuid, out var device);
                    if (device != null && !device.Components.IsNullOrEmpty())
                    {
                        return device.Components.FirstOrDefault(o => o.Id == componentId);
                    }
                }
            }

            return null;
        }

        private IDataItem GetCachedDataItem(string deviceUuid, string dataItemId)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    _devices.TryGetValue(deviceUuid, out var device);
                    if (device != null)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty())
                        {
                            return dataItems.FirstOrDefault(o => o.Id == dataItemId);
                        }
                    }
                }
            }

            return null;
        }

    }
}
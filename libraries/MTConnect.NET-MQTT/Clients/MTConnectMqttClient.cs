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
    public class MTConnectMqttClient : IMTConnectEntityClient, IDisposable
    {
        private const string _defaultTopicPrefix = "MTConnect";
        private const string _defaultDocumentFormat = "json-cppagent-mqtt";

        private const string _defaultProbeTopicPrefix = "Probe";
        private const string _defaultCurrentTopicPrefix = "Current";
        private const string _defaultSampleTopicPrefix = "Sample";
        private const string _defaultAssetTopicPrefix = "Asset";

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
        private readonly Dictionary<string, MTConnectMqttAgentInformation> _agents = new Dictionary<string, MTConnectMqttAgentInformation>(); // AgentUuid > AgentInformation
        private readonly Dictionary<string, string> _deviceAgentUuids = new Dictionary<string, string>(); // DeviceUuid > AgentUuid
        private readonly Dictionary<string, long> _agentInstanceIds = new Dictionary<string, long>(); // AgentUuid > InstanceId
        private readonly Dictionary<string, long> _agentHeartbeatTimestamps = new Dictionary<string, long>(); // AgentUuid > Last Heartbeat received (Unix milliseconds)
        private readonly Dictionary<string, System.Timers.Timer> _connectionTimers = new Dictionary<string, System.Timers.Timer>();
        private readonly Dictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();
        private readonly Dictionary<string, long> _deviceLastSequence = new Dictionary<string, long>();
        private readonly Dictionary<string, long> _deviceLastCurrentSequence = new Dictionary<string, long>();
        private readonly object _lock = new object();


        private CancellationTokenSource _stop;
        private MTConnectMqttConnectionStatus _connectionStatus;
        //private long _lastInstanceId;
        //private long _lastCurrentSequence;
        //private long _lastSequence;
        private long _lastResponse;


        public delegate void MTConnectMqttEventHandler<T>(string deviceUuid, T item);


        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int ReconnectionInterval { get; set; }

        public string Server => _server;

        public int Port => _port;

        public int QoS => _qos;

        public int Interval => _interval;

        public string TopicPrefix => _topicPrefix;

        ///// <summary>
        ///// Gets the Last Instance ID read from the MTConnect Agent
        ///// </summary>
        //public long LastInstanceId => _lastInstanceId;

        ///// <summary>
        ///// Gets the Last Sequence read from the MTConnect Agent
        ///// </summary>
        //public long LastSequence => _lastSequence;

        /// <summary>
        /// Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent
        /// </summary>
        public long LastResponse => _lastResponse;

        public MTConnectMqttConnectionStatus ConnectionStatus => _connectionStatus;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler<MTConnectMqttConnectionStatus> ConnectionStatusChanged;

        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public event EventHandler<Exception> InternalError;

        public event EventHandler<IDevice> DeviceReceived;

        public event EventHandler<IObservation> ObservationReceived;

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


        public MTConnectMqttClient(string server, int port = 1883, string deviceUuid = null, string topicPrefix = _defaultTopicPrefix, string documentFormat = _defaultDocumentFormat, string clientId = null, int qos = 1)
        {
            ReconnectionInterval = 10000;

            _server = server;
            _port = port;
            _deviceUuid = deviceUuid;
            _topicPrefix = topicPrefix;
            _documentFormat = documentFormat;
            _clientId = clientId;
            _qos = qos;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += MessageReceived;
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

                        if (!string.IsNullOrEmpty(_deviceUuid))
                        {
                            // Start protocol for a single Device
                            StartDeviceProtocol(_deviceUuid).Wait();
                        }
                        else
                        {
                            // Start protocol for all devices
                            StartAllDevicesProtocol().Wait();
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

                    await Task.Delay(ReconnectionInterval, _stop.Token);
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
                if (_mqttClient != null) _mqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection).Wait();
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
            await _mqttClient.SubscribeAsync($"MTConnect/Asset/{deviceUuid}");
        }


        private async Task MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.Payload != null && args.ApplicationMessage.Payload.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                Console.WriteLine($"Message Received : {topic} : {args.ApplicationMessage.Payload.Length}");

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
        }


        private bool IsProbeTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_topicPrefix}/{_defaultProbeTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }

        private bool IsCurrentTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_topicPrefix}/{_defaultCurrentTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }

        private bool IsSampleTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_topicPrefix}/{_defaultSampleTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }

        private bool IsAssetTopic(string topic)
        {
            if (topic != null)
            {
                var prefix = $"{_topicPrefix}/{_defaultAssetTopicPrefix}/";
                return topic.StartsWith(prefix);
            }

            return false;
        }


        private void ProcessProbeMessage(MqttApplicationMessage message)
        {
            var result = EntityFormatter.CreateDevice(_documentFormat, message.Payload);
            if (result.Success)
            {
                var device = result.Entity;
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
                var result = ResponseDocumentFormatter.CreateStreamsResponseDocument(_documentFormat, message.Payload);
                if (result.Success)
                {
                    ProcessCurrentDocument(result.Document);
                }
            }
            else
            {
                Console.WriteLine("STALE CURRENT!!!");
            }
        }

        private void ProcessSampleMessage(MqttApplicationMessage message)
        {
            if (!message.Retain)
            {
                var result = ResponseDocumentFormatter.CreateStreamsResponseDocument(_documentFormat, message.Payload);
                if (result.Success)
                {
                    ProcessSampleDocument(result.Document);
                }
            }
            else
            {
                Console.WriteLine("STALE SAMPLE!!!");
            }
        }

        private void ProcessAssetMessage(MqttApplicationMessage message)
        {
            var result = ResponseDocumentFormatter.CreateAssetsResponseDocument(_documentFormat, message.Payload);
            if (result.Success)
            {
                ProcessAssetsDocument(result.Document);
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
                    if (!string.IsNullOrEmpty(_deviceUuid)) deviceStream = document.Streams.FirstOrDefault(o => o.Uuid == _deviceUuid);
                    else deviceStream = document.Streams.FirstOrDefault();

                    var observations = deviceStream.Observations;
                    if (deviceStream != null && deviceStream.Uuid != null && !observations.IsNullOrEmpty())
                    {
                        long lastSequence;
                        lock (_lock) _deviceLastSequence.TryGetValue(deviceStream.Uuid, out lastSequence);

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
                    if (!string.IsNullOrEmpty(_deviceUuid)) deviceStream = document.Streams.FirstOrDefault(o => o.Uuid == _deviceUuid);
                    else deviceStream = document.Streams.FirstOrDefault();

                    if (deviceStream != null && deviceStream.Observations != null && deviceStream.Observations.Count() > 0)
                    {
                        long lastCurrentSequence;
                        long lastSequence;
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

                                //// Save the most recent Sequence that was read
                                //_lastSequence = observations.Max(o => o.Sequence);

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


        private static string GetFilePath(string path)
        {
            var x = path;
            if (!Path.IsPathRooted(x))
            {
                x = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, x);
            }

            return x;
        }

        #region "Cache"

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

        #endregion

    }
}
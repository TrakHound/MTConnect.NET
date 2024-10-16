// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Assets;
using MTConnect.Assets.Json;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Formatters;
using MTConnect.Mqtt;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public class MTConnectMqttExpandedClient : IMTConnectEntityClient, IDisposable
    {
        private const string _defaultTopic = "MTConnect/#";
        private const string _defaultAgentsTopic = "MTConnect/Agents/#";
        private const string _defaultAgentTopicPattern = "MTConnect\\/Agents\\/([^\\/]*)\\/([^\\/]*)";
        private const string _deviceUuidTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)";
        private const string _deviceTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Device";
        private const string _deviceAgentUuidTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/AgentUuid";
        private const string _observationsTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Observations";
        private const string _conditionsTopicPattern = "MTConnect\\/Devices\\/(.*)\\/Observations\\/.*\\/Conditions";
        private const string _assetTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Assets";

        private static readonly Regex _agentRegex = new Regex(_defaultAgentTopicPattern);
        private static readonly Regex _deviceUuidRegex = new Regex(_deviceUuidTopicPattern);
        private static readonly Regex _deviceAgentUuidRegex = new Regex(_deviceAgentUuidTopicPattern);
        private static readonly Regex _deviceRegex = new Regex(_deviceTopicPattern);
        private static readonly Regex _observationsRegex = new Regex(_observationsTopicPattern);
        private static readonly Regex _conditionsRegex = new Regex(_conditionsTopicPattern);
        private static readonly Regex _assetRegex = new Regex(_assetTopicPattern);

        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly string _server;
        private readonly int _port;
        private readonly int _qos;
        private readonly int _interval;
        private readonly string _deviceUuid;
        private readonly string _username;
        private readonly string _password;
        private readonly string _clientId;
        private readonly string _caCertPath;
        private readonly string _pemClientCertPath;
        private readonly string _pemPrivateKeyPath;
        private readonly bool _allowUntrustedCertificates;
        private readonly bool _useTls;
        private readonly IEnumerable<string> _topics;
        private readonly Dictionary<string, MTConnectMqttAgentInformation> _agents = new Dictionary<string, MTConnectMqttAgentInformation>(); // AgentUuid > AgentInformation
        private readonly Dictionary<string, string> _deviceAgentUuids = new Dictionary<string, string>(); // DeviceUuid > AgentUuid
        private readonly Dictionary<string, ulong> _agentInstanceIds = new Dictionary<string, ulong>(); // AgentUuid > InstanceId
        private readonly Dictionary<string, long> _agentHeartbeatTimestamps = new Dictionary<string, long>(); // AgentUuid > Last Heartbeat received (Unix milliseconds)
        private readonly Dictionary<string, System.Timers.Timer> _connectionTimers = new Dictionary<string, System.Timers.Timer>();
        private readonly object _lock = new object();


        private CancellationTokenSource _stop;
        private MTConnectMqttConnectionStatus _connectionStatus;


        public delegate void MTConnectMqttEventHandler<T>(string deviceUuid, T item);


        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int ReconnectionInterval { get; set; }

        public string Server => _server;

        public int Port => _port;

        public int Qos => _qos;

        public int Interval => _interval;

        public IEnumerable<string> Topics => _topics;

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


        public MTConnectMqttExpandedClient(string server, int port = 1883, int interval = 0, string deviceUuid = null, IEnumerable<string> topics = null, int qos = 1)
        {
            ReconnectionInterval = 10000;

            _server = server;
            _port = port;
            _topics = !topics.IsNullOrEmpty() ? topics : new List<string> { _defaultTopic };
            _qos = qos;
            _interval = interval;
            _deviceUuid = deviceUuid;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            if (!string.IsNullOrEmpty(_deviceUuid))
            {
                _mqttClient.ApplicationMessageReceivedAsync += DeviceMessageReceived;
            }
            else
            {
                _mqttClient.ApplicationMessageReceivedAsync += AllDevicesMessageReceived;
            }
        }

        public MTConnectMqttExpandedClient(IMTConnectMqttClientConfiguration configuration, IEnumerable<string> topics = null)
        {
            ReconnectionInterval = 10000;

            if (configuration != null)
            {
                _server = configuration.Server;
                _port = configuration.Port;
                _interval = configuration.Interval;
                _deviceUuid = configuration.DeviceUuid;
                _qos = configuration.Qos;
                _username = configuration.Username;
                _password = configuration.Password;
                _clientId = configuration.ClientId;
                _caCertPath = configuration.CertificateAuthority;
                _pemClientCertPath = configuration.PemCertificate;
                _pemPrivateKeyPath = configuration.PemPrivateKey;
                _allowUntrustedCertificates = configuration.AllowUntrustedCertificates;
                _useTls = configuration.UseTls;
            }

            _topics = !topics.IsNullOrEmpty() ? topics : new List<string> { _defaultTopic };

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();

            if (!string.IsNullOrEmpty(_deviceUuid))
            {
                _mqttClient.ApplicationMessageReceivedAsync += DeviceMessageReceived;
            }
            else
            {
                _mqttClient.ApplicationMessageReceivedAsync += AllDevicesMessageReceived;
            }
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
                if (_mqttClient != null) await _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
            }
            catch { }


            ClientStopped?.Invoke(this, new EventArgs());
        }


        private async Task StartAllDevicesProtocol()
        {
            // Clear any previous subscriptions
            await _mqttClient.UnsubscribeAsync("#");

            // Subscribe to all Agents
            await _mqttClient.SubscribeAsync("MTConnect/Agents/#");
        }

        private async Task StartDeviceProtocol(string deviceUuid)
        {
            // Clear any previous subscriptions
            await _mqttClient.UnsubscribeAsync("#");

            // Subscribe to the Agent UUID for the Device
            await _mqttClient.SubscribeAsync($"MTConnect/Devices/{deviceUuid}/AgentUuid");
        }

        private async Task SubscribeToDeviceAgent(string agentUuid)
        {
            // Subscribe to both Agent Information and Heartbeat
            await _mqttClient.SubscribeAsync($"MTConnect/Agents/{agentUuid}/#");
        }

        private async Task SubscribeToDeviceModel(string deviceUuid)
        {
            // Subscribe to the Device Model
            await _mqttClient.SubscribeAsync($"MTConnect/Devices/{deviceUuid}/Device");
        }

        private async Task SubscribeToDevice(string deviceUuid, int interval = 0)
        {
            if (interval > 0)
            {
                // Subscribe to the Device Observations for the specified Interval
                await _mqttClient.SubscribeAsync($"MTConnect/Devices/{deviceUuid}/Observations[{interval}]/#");
            }
            else
            {
                // Subscribe to the Device "Realtime" Observations
                await _mqttClient.SubscribeAsync($"MTConnect/Devices/{deviceUuid}/Observations/#");
            }

            // Subscribe to the Device Assets
            await _mqttClient.SubscribeAsync($"MTConnect/Devices/{deviceUuid}/Assets/#");
        }


        private async Task AllDevicesMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.PayloadSegment != null && args.ApplicationMessage.PayloadSegment.Array != null && args.ApplicationMessage.PayloadSegment.Array.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                if (_conditionsRegex.IsMatch(topic))
                {
                    ProcessObservations(args.ApplicationMessage);
                }
                else if (_observationsRegex.IsMatch(topic))
                {
                    ProcessObservation(args.ApplicationMessage);
                }
                else if (_assetRegex.IsMatch(topic))
                {
                    ProcessAsset(args.ApplicationMessage);
                }
                else if (_deviceRegex.IsMatch(topic))
                {
                    ProcessDevice(args.ApplicationMessage);
                }
                else if (_deviceAgentUuidRegex.IsMatch(topic))
                {
                    await ProcessDeviceAgentUuid(args.ApplicationMessage);
                }
                else if (_agentRegex.IsMatch(topic))
                {
                    await ProcessAgent(args.ApplicationMessage, SubscribeToAllDevices);
                }
            }
        }

        private async Task DeviceMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.PayloadSegment != null && args.ApplicationMessage.PayloadSegment.Array != null && args.ApplicationMessage.PayloadSegment.Array.Length > 0)
            {
                var topic = args.ApplicationMessage.Topic;

                if (_conditionsRegex.IsMatch(topic))
                {
                    ProcessObservations(args.ApplicationMessage);
                }
                else if (_observationsRegex.IsMatch(topic))
                {
                    ProcessObservation(args.ApplicationMessage);
                }
                else if (_assetRegex.IsMatch(topic))
                {
                    ProcessAsset(args.ApplicationMessage);
                }
                else if (_deviceRegex.IsMatch(topic))
                {
                    ProcessDevice(args.ApplicationMessage);
                }
                else if (_deviceAgentUuidRegex.IsMatch(topic))
                {
                    await ProcessDeviceAgentUuid(args.ApplicationMessage);
                }
                else if (_agentRegex.IsMatch(topic))
                {
                    await ProcessAgent(args.ApplicationMessage, SubscribeToDevice);
                }
            }
        }


        private async void ProcessObservation(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[1].Value;

                // Deserialize JSON to Observation
                var jsonObservation = JsonSerializer.Deserialize<JsonObservation>(message.Payload);
                if (jsonObservation != null)
                {
                    var observation = new Observation();
                    observation.DeviceUuid = deviceUuid;
                    observation.InstanceId = jsonObservation.InstanceId;
                    observation.DataItemId = jsonObservation.DataItemId;
                    observation.Category = jsonObservation.Category.ConvertEnum<DataItemCategory>();
                    observation.Name = jsonObservation.Name;
                    observation.Type = jsonObservation.Type;
                    observation.SubType = jsonObservation.SubType;
                    observation.Sequence = jsonObservation.Sequence;
                    observation.Timestamp = jsonObservation.Timestamp;
                    observation.CompositionId = jsonObservation.CompositionId;
                    //observation.Representation = jsonObservation.Representation.ConvertEnum<DataItemRepresentation>();

                    // Set Result
                    if (jsonObservation.Result != null)
                    {
                        observation.AddValue(ValueKeys.Result, jsonObservation.Result);
                    }

                    // Get stored Agent Uuid for Device
                    string agentUuid;
                    lock (_lock) _deviceAgentUuids.TryGetValue(deviceUuid, out agentUuid);
                    if (!string.IsNullOrEmpty(agentUuid))
                    {
                        // Verify Agent InstanceId
                        ulong agentInstanceId;
                        lock (_lock) _agentInstanceIds.TryGetValue(agentUuid, out agentInstanceId);

                        if (observation.InstanceId == agentInstanceId)
                        {
                            if (ObservationReceived != null)
                            {
                                ObservationReceived.Invoke(deviceUuid, observation);
                            }
                        }
                        else
                        {
                            // If InstanceId has changed, then restart protocol
                            if (!string.IsNullOrEmpty(_deviceUuid))
                            {
                                await StartDeviceProtocol(_deviceUuid);
                            }
                            else
                            {
                                await StartAllDevicesProtocol();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void ProcessObservations(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[1].Value;

                // Deserialize JSON to Observation
                var jsonObservations = JsonSerializer.Deserialize<IEnumerable<JsonObservation>>(message.Payload);
                if (!jsonObservations.IsNullOrEmpty())
                {
                    foreach (var jsonObservation in jsonObservations)
                    {
                        var observation = new Observation();
                        observation.DeviceUuid = deviceUuid;
                        observation.InstanceId = jsonObservation.InstanceId;
                        observation.DataItemId = jsonObservation.DataItemId;
                        observation.Category = jsonObservation.Category.ConvertEnum<DataItemCategory>();
                        observation.Name = jsonObservation.Name;
                        observation.Type = jsonObservation.Type;
                        observation.SubType = jsonObservation.SubType;
                        observation.Sequence = jsonObservation.Sequence;
                        observation.Timestamp = jsonObservation.Timestamp;
                        observation.CompositionId = jsonObservation.CompositionId;
                        //observation.Representation = jsonObservation.Representation.ConvertEnum<DataItemRepresentation>();

                        // Set Result
                        if (jsonObservation.Result != null)
                        {
                            observation.AddValue(ValueKeys.Result, jsonObservation.Result);
                        }

                        if (ObservationReceived != null)
                        {
                            ObservationReceived.Invoke(deviceUuid, observation);
                        }
                    }
                }
            }
            catch { }
        }

        private async Task ProcessAgent(MqttApplicationMessage message, Func<MTConnectMqttAgentInformation, Task> onConnectedFunction)
        {
            try
            {
                var match = _agentRegex.Match(message.Topic);
                if (match.Success && match.Groups.Count > 2)
                {
                    // Read Agent UUID
                    var agentUuid = match.Groups[1].Value;

                    // Read Agent Property
                    var property = match.Groups[2].Value;

                    if (!string.IsNullOrEmpty(agentUuid) && !string.IsNullOrEmpty(property))
                    {
                        // Decode UTF8 bytes to string
                        var value = Encoding.UTF8.GetString(message.Payload);

                        switch (property.ToLower())
                        {
                            case "information":

                                var agentInformation = JsonSerializer.Deserialize<MTConnectMqttAgentInformation>(value);
                                if (agentInformation != null)
                                {
                                    lock (_lock)
                                    {
                                        // Update the stored Agent Information
                                        _agents.Remove(agentUuid);
                                        _agents.Add(agentUuid, agentInformation);

                                        // Update the stored Agent InstanceId
                                        _agentInstanceIds.Remove(agentUuid);
                                        _agentInstanceIds.Add(agentUuid, agentInformation.InstanceId);

                                        // Stop the existing Connection Timer
                                        _connectionTimers.TryGetValue(agentUuid, out var connectionTimer);
                                        if (connectionTimer != null) connectionTimer.Stop();

                                        // Start new Connection Timer
                                        _connectionTimers.Remove(agentUuid);
                                        connectionTimer = new System.Timers.Timer();
                                        connectionTimer.Interval = agentInformation.HeartbeatInterval;
                                        connectionTimer.Elapsed += (s, a) => ConnectionTimerElapsed(agentUuid);
                                        _connectionTimers.Add(agentUuid, connectionTimer);
                                        connectionTimer.Start();
                                    }
                                }

                                break;


                            case "heartbeattimestamp":

                                var previousConnectionStatus = _connectionStatus;
                                _connectionStatus = MTConnectMqttConnectionStatus.Connected;

                                lock (_lock)
                                {
                                    _agentHeartbeatTimestamps.Remove(agentUuid);
                                    _agentHeartbeatTimestamps.Add(agentUuid, value.ToLong());
                                }

                                if (previousConnectionStatus == MTConnectMqttConnectionStatus.Disconnected)
                                {
                                    if (ConnectionStatusChanged != null) ConnectionStatusChanged.Invoke(this, _connectionStatus);

                                    if (_agents.TryGetValue(agentUuid, out var agent))
                                    {
                                        if (onConnectedFunction != null) await onConnectedFunction(agent);
                                    }
                                }

                                break;
                        }
                    }
                }
            }
            catch { }
        }

        private async void ProcessDevice(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[1].Value;

                // Deserialize JSON to Device
                var jsonDevice = JsonSerializer.Deserialize<JsonDevice>(message.Payload);
                if (jsonDevice != null)
                {
                    var device = jsonDevice.ToDevice();
                    if (device != null)
                    {
                        if (DeviceReceived != null)
                        {
                            DeviceReceived.Invoke(deviceUuid, device);
                        }

                        await SubscribeToDevice(deviceUuid, _interval);
                    }
                }
            }
            catch { }
        }

        private async Task ProcessDeviceAgentUuid(MqttApplicationMessage message)
        {
            try
            {
                var agentUuid = Encoding.UTF8.GetString(message.Payload);

                await SubscribeToDeviceAgent(agentUuid);
            }
            catch { }
        }

        private void ProcessAsset(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[1].Value;

                var stream = new MemoryStream(message.Payload);

                // Deserialize JSON to Device
                var jsonAsset = JsonSerializer.Deserialize<JsonAsset>(stream);
                if (jsonAsset != null)
                {
                    var response = EntityFormatter.CreateAsset(DocumentFormat.JSON, jsonAsset.Type, stream);
                    if (response.Success)
                    {
                        if (AssetReceived != null)
                        {
                            AssetReceived.Invoke(deviceUuid, response.Content);
                        }
                    }
                }
            }
            catch { }
        }


        private async Task SubscribeToAllDevices(MTConnectMqttAgentInformation agent)
        {
            // Subscribe to Devices
            if (!agent.Devices.IsNullOrEmpty())
            {
                foreach (var deviceUuid in agent.Devices)
                {
                    lock (_lock)
                    {
                        _deviceAgentUuids.Remove(deviceUuid);
                        _deviceAgentUuids.Add(deviceUuid, agent.Uuid);
                    }

                    await SubscribeToDeviceModel(deviceUuid);
                }
            }
        }

        private async Task SubscribeToDevice(MTConnectMqttAgentInformation agent)
        {
            lock (_lock)
            {
                _deviceAgentUuids.Remove(_deviceUuid);
                _deviceAgentUuids.Add(_deviceUuid, agent.Uuid);
            }

            // Subscribe to Device
            await SubscribeToDeviceModel(_deviceUuid);
        }


        private void ConnectionTimerElapsed(string agentUuid)
        {
            MTConnectMqttAgentInformation agentInformation;
            long timestamp = 0;
            var now = UnixDateTime.Now / 10000;

            lock (_lock)
            {
                _agents.TryGetValue(agentUuid, out agentInformation);
                _agentHeartbeatTimestamps.TryGetValue(agentUuid, out timestamp);
            }

            if (agentInformation != null && timestamp > 0)
            {
                var diff = now - timestamp;

                if (_connectionStatus == MTConnectMqttConnectionStatus.Connected && diff > agentInformation.HeartbeatInterval * 3)
                {
                    // Set Connection Status to Disconnected
                    _connectionStatus = MTConnectMqttConnectionStatus.Disconnected;

                    if (ConnectionStatusChanged != null) ConnectionStatusChanged.Invoke(this, _connectionStatus);
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
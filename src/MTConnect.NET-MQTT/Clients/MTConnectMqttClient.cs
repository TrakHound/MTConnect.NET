// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.Json;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTConnect.Clients.Mqtt
{
    public class MTConnectMqttClient : IDisposable
    {
        private const string _defaultTopic = "MTConnect/#";
        private const string _deviceUuidTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)";
        private const string _deviceTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Device";
        private const string _observationsTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Observations";
        private const string _conditionsTopicPattern = "MTConnect\\/Devices\\/(.*)\\/Observations\\/.*\\/Conditions";
        //private const string _conditionsTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Observations\\/([^\\/]*)\\/Conditions";
        private const string _assetTopicPattern = "MTConnect\\/Devices\\/([^\\/]*)\\/Assets";

        private static readonly Regex _deviceUuidRegex = new Regex(_deviceUuidTopicPattern);
        private static readonly Regex _deviceRegex = new Regex(_deviceTopicPattern);
        private static readonly Regex _observationsRegex = new Regex(_observationsTopicPattern);
        private static readonly Regex _conditionsRegex = new Regex(_conditionsTopicPattern);
        private static readonly Regex _assetRegex = new Regex(_assetTopicPattern);

        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly string _server;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useTls;
        private readonly IEnumerable<string> _topics;

        public delegate void MTConnectMqttEventHandler<T>(string deviceUuid, T item);


        public string Server => _server;

        public int Port => _port;

        public IEnumerable<string> Topics => _topics;

        public EventHandler Connected { get; set; }

        public EventHandler Disconnected { get; set; }

        public EventHandler<Exception> ConnectionError { get; set; }

        public MTConnectMqttEventHandler<IDevice> DeviceReceived { get; set; }

        public MTConnectMqttEventHandler<IObservation> ObservationReceived { get; set; }

        public MTConnectMqttEventHandler<IAsset> AssetReceived { get; set; }


        public MTConnectMqttClient(string server, int port = 1883, IEnumerable<string> topics = null)
        {
            _server = server;
            _port = port;
            _topics = !topics.IsNullOrEmpty() ? topics : new List<string> { _defaultTopic };

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += MessageReceived;
        }

        public MTConnectMqttClient(MTConnectMqttClientConfiguration configuration, IEnumerable<string> topics = null)
        {
            if (configuration != null)
            {
                _server = configuration.Server;
                _port = configuration.Port; ;
                _username = configuration.Username;
                _password = configuration.Password;
                _useTls = configuration.UseTls;
            }

            _topics = !topics.IsNullOrEmpty() ? topics : new List<string> { _defaultTopic };

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += MessageReceived;
        }


        public async Task StartAsync()
        {
            try
            {
                MqttClientOptions clientOptions;

                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    if (_useTls)
                    {
                        clientOptions = new MqttClientOptionsBuilder()
                        .WithTcpServer(_server, _port)
                        .WithCredentials(_username, _password)
                        .WithTls()
                        .Build();
                    }
                    else
                    {
                        clientOptions = new MqttClientOptionsBuilder()
                        .WithTcpServer(_server, _port)
                        .WithCredentials(_username, _password)
                        .Build();
                    }
                }
                else
                {
                    clientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_server, _port)
                    .Build();
                }

                // Connect to the MQTT Client
                await _mqttClient.ConnectAsync(clientOptions);

                // Configure Topics to subscribe to
                var subscribeEptionsBuilder = _mqttFactory.CreateSubscribeOptionsBuilder();
                foreach (var topic in _topics)
                {
                    subscribeEptionsBuilder.WithTopicFilter(topic);
                }
                var subscribeOptions = subscribeEptionsBuilder.Build();

                // Subscribe to Topics
                await _mqttClient.SubscribeAsync(subscribeOptions);
            }
            catch (Exception ex)
            {
                if (ConnectionError != null) ConnectionError.Invoke(this, ex);
            }
        }

        public async Task StopAsync()
        {
            try
            {
                // Disconnect from the MQTT Client
                if (_mqttClient != null) await _mqttClient.DisconnectAsync();
            }
            catch { }      
        }

        public void Dispose()
        {
            if (_mqttClient != null) _mqttClient.Dispose();
        }


        private Task MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.Payload != null && args.ApplicationMessage.Payload.Length > 0)
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
            }

            return Task.CompletedTask;
        }

        private void ProcessObservation(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[0].Value;

                // Deserialize JSON to Observation
                var jsonObservation = JsonSerializer.Deserialize<JsonObservation>(message.Payload);
                if (jsonObservation != null)
                {
                    var observation = new Observation();
                    observation.DeviceUuid = deviceUuid;
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
            catch { }
        }

        private void ProcessObservations(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[0].Value;

                // Deserialize JSON to Observation
                var jsonObservations = JsonSerializer.Deserialize<IEnumerable<JsonObservation>>(message.Payload);
                if (!jsonObservations.IsNullOrEmpty())
                {
                    foreach (var jsonObservation in jsonObservations)
                    {
                        var observation = new Observation();
                        observation.DeviceUuid = deviceUuid;
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

        private void ProcessDevice(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[0].Value;

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
                    }
                }
            }
            catch { }
        }

        private void ProcessAsset(MqttApplicationMessage message)
        {
            try
            {
                // Read Device UUID
                var deviceUuid = _deviceUuidRegex.Match(message.Topic).Groups[0].Value;

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
                    }
                }
            }
            catch { }
        }
    }
}

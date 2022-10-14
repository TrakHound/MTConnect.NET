// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttRelay : IDisposable
    {
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly MTConnectMqttClientConfiguration _configuration;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly IEnumerable<string> _documentFormats = new List<string>() { "JSON" };
        private CancellationTokenSource _stop;


        public string Server => _configuration.Server;

        public int Port => _configuration.Port;

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int RetryInterval => _configuration.RetryInterval;

        public EventHandler Connected { get; set; }

        public EventHandler Disconnected { get; set; }

        public EventHandler<string> MessageSent { get; set; }

        public EventHandler<Exception> ConnectionError { get; set; }

        public EventHandler<Exception> PublishError { get; set; }


        public MTConnectMqttRelay(IMTConnectAgent mtconnectAgent, MTConnectMqttClientConfiguration configuration)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.ObservationAdded += ObservationAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;

            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttClientConfiguration();

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
        }

        public void Start()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

            try
            {
                if (_mqttClient != null) _mqttClient.DisconnectAsync();
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
                        MqttClientOptions mqttClientOptions;

                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                mqttClientOptions = new MqttClientOptionsBuilder()
                                .WithTcpServer(_configuration.Server, _configuration.Port)
                                .WithCredentials(_configuration.Username, _configuration.Password)
                                .WithTls()
                                .Build();
                            }
                            else
                            {
                                mqttClientOptions = new MqttClientOptionsBuilder()
                                .WithTcpServer(_configuration.Server, _configuration.Port)
                                .WithCredentials(_configuration.Username, _configuration.Password)
                                .Build();
                            }
                        }
                        else
                        {
                            mqttClientOptions = new MqttClientOptionsBuilder()
                            .WithTcpServer(_configuration.Server, _configuration.Port)
                            .Build();
                        }

                        await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                        if (Connected != null) Connected.Invoke(this, new EventArgs());

                        // Add Agent Devices
                        var devices = _mtconnectAgent.GetDevices();
                        if (!devices.Devices.IsNullOrEmpty())
                        {
                            foreach (var device in devices.Devices)
                            {
                                await PublishDevice(device);
                            }
                        }

                        //var current = _mtconnectAgent.GetDeviceStreams();
                        //if (!current.Streams.IsNullOrEmpty())
                        //{
                        //    foreach (var stream in current.Streams)
                        //    {
                        //        var observations = stream.Observations;
                        //        if (!observations.IsNullOrEmpty())
                        //        {
                        //            foreach (var observation in observations)
                        //            {
                        //                await PublishObservation(observation);
                        //            }
                        //        }
                        //    }
                        //}

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

        //public async Task DisconnectAsync()
        //{
        //    try
        //    {
        //        if (_mqttClient != null) await _mqttClient.DisconnectAsync();

        //        if (Disconnected != null) Disconnected.Invoke(this, new EventArgs());
        //    }
        //    catch { }
        //}

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


        private async Task PublishDevice(IDevice device)
        {
            foreach (var documentFormat in _documentFormats)
            {
                var messages = CreateMessages(device, documentFormat);
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
        }

        private async Task PublishObservation(IObservation observation)
        {
            foreach (var documentFormat in _documentFormats)
            {
                var message = CreateMessage(observation, documentFormat);
                if (message != null && message.Payload != null) await Publish(message);
            }
        }

        private async Task PublishAsset(IAsset asset)
        {
            foreach (var documentFormat in _documentFormats)
            {
                var messages = CreateMessage(asset, documentFormat);
                await Publish(messages);
            }
        }


        private async Task Publish(MqttApplicationMessage message)
        {
            try
            {
                if (_mqttClient != null && _mqttClient.IsConnected)
                {
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
                        await _mqttClient.PublishAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (PublishError != null) PublishError.Invoke(this, ex);
            }
        }

        #region "Messages"

        private MqttApplicationMessage CreateMessage(string topic, string payload)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(payload);

                return new MqttApplicationMessage
                {
                    Topic = topic,
                    Payload = bytes,
                    Retain = true
                };
            }
            catch { }

            return null;
        }


        private IEnumerable<MqttApplicationMessage> CreateMessages(IDevice device, string documentFormatterId = DocumentFormat.XML)
        {
            if (device != null && !string.IsNullOrEmpty(documentFormatterId))
            {
                var messages = new List<MqttApplicationMessage>();

                var topic = $"MTConnect/Devices/{device.Uuid}/Device";
                messages.Add(CreateMessage(topic, Formatters.EntityFormatter.Format(documentFormatterId, device)));

                return messages;
            }

            return null;
        }

        private MqttApplicationMessage CreateMessage(IObservation observation, string documentFormatterId = DocumentFormat.XML)
        {
            if (observation != null && observation.DataItem != null && observation.DataItem.Container != null && !observation.Values.IsNullOrEmpty())
            {
                var category = observation.Category.ToString().ToTitleCase() + "s";
                var topicPrefix = $"MTConnect/Devices/{observation.DeviceUuid}/Observations/{observation.DataItem.Container.Type}/{observation.DataItem.Container.Id}/{category}/{observation.Type}";

                var topic = $"{topicPrefix}/{observation.DataItemId}";
                if (!string.IsNullOrEmpty(observation.SubType)) topic = $"{topicPrefix}/SubTypes/{observation.SubType}/{observation.DataItemId}";

                return CreateMessage(topic, Formatters.EntityFormatter.Format(documentFormatterId, observation));
            }

            return null;
        }

        private IEnumerable<MqttApplicationMessage> CreateMessage(IAsset asset, string documentFormatterId = DocumentFormat.XML)
        {
            if (asset != null)
            {
                var messages = new List<MqttApplicationMessage>();

                var payload = Formatters.EntityFormatter.Format(documentFormatterId, asset);
                messages.Add(CreateMessage($"MTConnect/Assets/{asset.Type}/{asset.AssetId}", payload));
                messages.Add(CreateMessage($"MTConnect/Devices/{asset.DeviceUuid}/Assets/{asset.Type}/{asset.AssetId}", payload));

                return messages;
            }

            return null;
        }


        #endregion
    }
}

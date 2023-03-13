// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttBroker : IHostedService
    {
        private const int _retryInterval = 5000;

        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly MqttServer _mqttServer;
        private CancellationTokenSource _stop;
        private IEnumerable<string> _documentFormats = new List<string>() { "JSON" };


        public MTConnectMqttFormat Format { get; set; }

        public string TopicPrefix { get; set; }

        public bool RetainMessages { get; set; }

        public EventHandler ClientConnected { get; set; }

        public EventHandler ClientDisconnected { get; set; }

        public EventHandler<string> MessageSent { get; set; }

        public EventHandler<Exception> ConnectionError { get; set; }

        public EventHandler<Exception> PublishError { get; set; }


        public MTConnectMqttBroker(IMTConnectAgent mtconnectAgent, MqttServer mqttServer)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.ObservationAdded += ObservationAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;

            Format = MTConnectMqttFormat.Hierarchy;
            RetainMessages = true;

            _mqttServer = mqttServer;
            _mqttServer.ClientConnectedAsync += async (args) =>
            {
                if (ClientConnected != null) ClientConnected.Invoke(this, new EventArgs());
            };
            _mqttServer.ClientDisconnectedAsync += async (args) =>
            {
                if (ClientDisconnected != null) ClientDisconnected.Invoke(this, new EventArgs());
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();

            if (!_mqttServer.IsStarted)
            {
                _ = Task.Run(Worker, _stop.Token);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_stop != null) _stop.Cancel();
            if (_mqttServer != null) await _mqttServer.StopAsync();
        }


        private async Task Worker()
        {
            do
            {
                try
                {
                    try
                    {
                        await _mqttServer.StartAsync();

                        // Publish MTConnect Agent Information
                        await PublishAgent(_mtconnectAgent);

                        var devices = _mtconnectAgent.GetDevices();
                        foreach (var device in devices)
                        {
                            // Publish the Device
                            await PublishDevice(device);
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
                                observation.Timestamp = observationOutput.Timestamp;
                                observation.AddValues(observationOutput.Values);

                                await PublishObservation(observation);
                            }
                        }

                        while (!_stop.Token.IsCancellationRequested)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    await Task.Delay(_retryInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { }

            } while (!_stop.Token.IsCancellationRequested);
        }


        private async void DeviceAdded(object sender, IDevice device)
        {
            await PublishDevice(device);
            await PublishAgent(_mtconnectAgent);
        }

        private async void ObservationAdded(object sender, IObservation observation)
        {
            await PublishObservation(observation);
        }

        private async void AssetAdded(object sender, IAsset asset)
        {
            await PublishAsset(asset);
            await PublishAgent(_mtconnectAgent);
        }


        private async Task PublishAgent(IMTConnectAgent agent)
        {
            foreach (var documentFormat in _documentFormats)
            {
                var messages = MTConnectMqttMessage.Create(agent, RetainMessages);
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

        private async Task PublishDevice(IDevice device)
        {
            foreach (var documentFormat in _documentFormats)
            {
                var messages = MTConnectMqttMessage.Create(device, documentFormat, RetainMessages);
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
                if (observation.Category != Devices.DataItems.DataItemCategory.CONDITION)
                {
                    var message = MTConnectMqttMessage.Create(observation, Format, documentFormat, RetainMessages);
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
                                y.Timestamp = dataItemObservation.Timestamp;
                                y.AddValues(dataItemObservation.Values);
                                x.Add(y);
                            }

                            var message = MTConnectMqttMessage.Create(x, Format, documentFormat, RetainMessages);
                            if (message != null && message.Payload != null) await Publish(message);
                        }
                    }
                }
            }
        }

        private async Task PublishAsset(IAsset asset)
        {
            foreach (var documentFormat in _documentFormats)
            {
                var messages = MTConnectMqttMessage.Create(asset, documentFormat, RetainMessages);
                await Publish(messages);
            }
        }


        private async Task Publish(MqttApplicationMessage message)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                // Set the Topic Prefix
                if (!string.IsNullOrEmpty(TopicPrefix)) message.Topic = $"{TopicPrefix}/{message.Topic}";

                var injectMessage = new InjectedMqttApplicationMessage(message);

                await _mqttServer.InjectApplicationMessage(injectMessage);
            }
        }

        private async Task Publish(IEnumerable<MqttApplicationMessage> messages)
        {
            if (_mqttServer != null && _mqttServer.IsStarted && !messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    // Set the Topic Prefix
                    if (!string.IsNullOrEmpty(TopicPrefix)) message.Topic = $"{TopicPrefix}/{message.Topic}";

                    await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
                }
            }
        }
    }
}
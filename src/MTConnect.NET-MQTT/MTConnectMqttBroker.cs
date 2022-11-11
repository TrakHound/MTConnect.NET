// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttBroker : IHostedService
    {
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly MqttServer _mqttServer;
        private CancellationTokenSource _stop;
        private IEnumerable<string> _documentFormats = new List<string>() { "JSON" };


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
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_stop != null) _stop.Cancel();
            if (_mqttServer != null) await _mqttServer.StopAsync();
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
                var messages = CreateMessages(agent);
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
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
            }
        }

        private async Task Publish(IEnumerable<MqttApplicationMessage> messages)
        {
            if (_mqttServer != null && _mqttServer.IsStarted && !messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
                }
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


        private IEnumerable<MqttApplicationMessage> CreateMessages(IMTConnectAgent agent)
        {
            if (agent != null)
            {
                var messages = new List<MqttApplicationMessage>();

                // UUID
                var topic = $"MTConnect/Agents/{agent.Uuid}/UUID";
                messages.Add(CreateMessage(topic, agent.Uuid));

                // InstanceId
                topic = $"MTConnect/Agents/{agent.Uuid}/InstanceId";
                messages.Add(CreateMessage(topic, agent.InstanceId.ToString()));

                // Agent Application Version
                topic = $"MTConnect/Agents/{agent.Uuid}/Version";
                messages.Add(CreateMessage(topic, agent.Version.ToString()));

                //// Observation Buffer Size
                //topic = $"MTConnect/Agents/{agent.Uuid}/BufferSize";
                //messages.Add(CreateMessage(topic, agent.BufferSize.ToString()));

                //// Asset Buffer Size
                //topic = $"MTConnect/Agents/{agent.Uuid}/AssetBufferSize";
                //messages.Add(CreateMessage(topic, agent.AssetBufferSize.ToString()));

                //// Asset Count
                //topic = $"MTConnect/Agents/{agent.Uuid}/AssetCount";
                //messages.Add(CreateMessage(topic, agent.AssetCount.ToString()));

                // Sender
                topic = $"MTConnect/Agents/{agent.Uuid}/Sender";
                messages.Add(CreateMessage(topic, agent.Sender));

                // DeviceModelChangeTime
                topic = $"MTConnect/Agents/{agent.Uuid}/DeviceModelChangeTime";
                messages.Add(CreateMessage(topic, agent.DeviceModelChangeTime.ToString("o")));

                return messages;
            }

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
            if (observation != null && observation.DataItem != null && !observation.Values.IsNullOrEmpty())
            {
                var category = observation.Category.ToString().ToTitleCase() + "s";
                var topicPrefix = $"MTConnect/Devices/{observation.DeviceUuid}/Observations/{observation.DataItem.Container.Type}/{observation.DataItem.Container.Id}/{category}/{observation.Type}";

                var topic = $"{topicPrefix}/{observation.DataItemId}";
                if (!string.IsNullOrEmpty(observation.SubType)) topic = $"{topicPrefix}/SubTypes/{observation.SubType}/{observation.DataItemId}";

                if (observation.Category != Devices.DataItems.DataItemCategory.CONDITION)
                {
                    return CreateMessage(topic, Formatters.EntityFormatter.Format(documentFormatterId, observation));
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
                            return CreateMessage(topic, Formatters.EntityFormatter.Format(documentFormatterId, x));
                        }
                    }
                }
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

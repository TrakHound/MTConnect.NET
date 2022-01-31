// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Agents
{
    public class MTConnectMqttBroker : IHostedService
    {
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly IMqttServer _mqttServer;
        private CancellationTokenSource _stop;


        public MTConnectMqttBroker(IMTConnectAgent mtconnectAgent, IMqttServer mqttServer)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.ObservationAdded += DataItemAdded;
            _mqttServer = mqttServer;
            _mqttServer.UseClientConnectedHandler((args) =>
            {
                Console.WriteLine("MQTT Client Connected");
            });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();

            if (!_mqttServer.IsStarted)
            {
                var options = new MqttServerOptions();
                await _mqttServer.StartAsync(options);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_stop != null) _stop.Cancel();
            if (_mqttServer != null) await _mqttServer.StopAsync();
        }



        private async void DeviceAdded(object sender, Devices.Device device)
        {
            await Publish(CreateMessages(device));
        }

        private async void DataItemAdded(object sender, IObservation dataItem)
        {
            await Publish(CreateMessage(dataItem));
        }


        private async Task Publish(MqttApplicationMessage message)
        {
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                await _mqttServer.PublishAsync(message, _stop.Token);
            }
        }

        private async Task Publish(IEnumerable<MqttApplicationMessage> messages)
        {
            if (_mqttServer != null && _mqttServer.IsStarted && !messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    await _mqttServer.PublishAsync(message, _stop.Token);
                }
            }
        }

        #region "Messages"

        private MqttApplicationMessage CreateMessage(string topic, object payload)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var json = JsonSerializer.Serialize(payload, options);
                var bytes = Encoding.ASCII.GetBytes(json);

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

        private IEnumerable<MqttApplicationMessage> CreateMessages(Devices.Device device)
        {
            if (device != null)
            {
                var messages = new List<MqttApplicationMessage>();

                var topic = $"Devices/{device.Name}";
                messages.Add(CreateMessage(topic, device));

                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        var dataItemTopic = $"{topic}/DataItems/{dataItem.Id}";
                        messages.Add(CreateMessage(dataItemTopic, dataItem));
                    }
                }

                // Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in device.Compositions)
                    {
                        messages.AddRange(CreateMessages(topic, composition));
                    }
                }

                // Components
                if (!device.Components.IsNullOrEmpty())
                {
                    foreach (var component in device.Components)
                    {
                        messages.AddRange(CreateMessages(topic, component));
                    }
                }

                return messages;
            }

            return null;
        }

        private IEnumerable<MqttApplicationMessage> CreateMessages(string parentTopic, Devices.Component component)
        {
            var messages = new List<MqttApplicationMessage>();

            if (component != null)
            {
                var topic = $"{parentTopic}/Components/{component.Id}";
                messages.Add(CreateMessage(topic, component));

                // DataItems
                if (!component.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in component.DataItems)
                    {
                        var dataItemTopic = $"{topic}/DataItems/{dataItem.Id}";
                        messages.Add(CreateMessage(dataItemTopic, dataItem));
                    }
                }

                // Compositions
                if (!component.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in component.Compositions)
                    {
                        messages.AddRange(CreateMessages(topic, composition));
                    }
                }

                // Components
                if (!component.Components.IsNullOrEmpty())
                {
                    foreach (var subcomponent in component.Components)
                    {
                        messages.AddRange(CreateMessages(topic, subcomponent));
                    }
                }

                return messages;
            }

            return messages;
        }

        private IEnumerable<MqttApplicationMessage> CreateMessages(string parentTopic, Devices.Composition composition)
        {
            var messages = new List<MqttApplicationMessage>();

            if (composition != null)
            {
                var topic = $"{parentTopic}/Compositions/{composition.Id}";
                messages.Add(CreateMessage(topic, composition));

                // DataItems
                if (!composition.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in composition.DataItems)
                    {
                        var dataItemTopic = $"{topic}/DataItems/{dataItem.Id}";
                        messages.Add(CreateMessage(dataItemTopic, dataItem));
                    }
                }

                return messages;
            }

            return messages;
        }

        private MqttApplicationMessage CreateMessage(IObservation dataItem)
        {
            if (dataItem != null)
            {
                var topic = $"Streams/{dataItem.DeviceName}/{dataItem.Key}/{dataItem.ValueType}";
                return CreateMessage(topic, dataItem);
            }

            return null;
        }

        #endregion
    }
}

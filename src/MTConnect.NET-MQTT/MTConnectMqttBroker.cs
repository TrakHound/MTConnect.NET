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


        public MTConnectMqttBroker(IMTConnectAgent mtconnectAgent, MqttServer mqttServer)
        {
            _mtconnectAgent = mtconnectAgent;
            _mtconnectAgent.DeviceAdded += DeviceAdded;
            _mtconnectAgent.ObservationAdded += ObservationAdded;
            _mtconnectAgent.AssetAdded += AssetAdded;
            _mqttServer = mqttServer;
            _mqttServer.ClientConnectedAsync += async (args) =>
            {
                Console.WriteLine("MQTT Client Connected");
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();

            if (!_mqttServer.IsStarted)
            {
                await _mqttServer.StartAsync();

                var devices = _mtconnectAgent.GetDevicesResponseDocument();
                foreach (var device in devices.Devices)
                {
                    // Publish the Device
                    await PublishDevice(device);
                }

                //// Publish the Agent Device
                //await PublishDevice(_mtconnectAgent.Agent);

                //foreach (var device in )
                //{

                //}
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
            if (_mqttServer != null && _mqttServer.IsStarted)
            {
                //await _mqttServer.UpdateRetainedMessageAsync(message);
                await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
                //await _mqttServer.PublishAsync(message, _stop.Token);
            }
        }

        private async Task Publish(IEnumerable<MqttApplicationMessage> messages)
        {
            if (_mqttServer != null && _mqttServer.IsStarted && !messages.IsNullOrEmpty())
            {
                foreach (var message in messages)
                {
                    //await _mqttServer.UpdateRetainedMessageAsync(message);
                    await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
                    //await _mqttServer.PublishAsync(message, _stop.Token);
                }
            }
        }

        #region "Messages"

        private MqttApplicationMessage CreateMessage(string topic, string payload)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(payload);

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

                //// DataItems
                //if (!device.DataItems.IsNullOrEmpty())
                //{
                //    foreach (var dataItem in device.DataItems)
                //    {
                //        var dataItemTopic = $"{topic}/DataItems/{dataItem.Type}/{dataItem.Id}";
                //        if (!string.IsNullOrEmpty(dataItem.SubType)) dataItemTopic = $"{topic}/DataItems/{dataItem.Type}/{dataItem.Id}";

                //        messages.Add(CreateMessage(dataItemTopic, Formatters.EntityFormatter.Format(documentFormatterId, dataItem)));
                //    }
                //}

                //// Compositions
                //if (!device.Compositions.IsNullOrEmpty())
                //{
                //    foreach (var composition in device.Compositions)
                //    {
                //        messages.AddRange(CreateMessages(topic, composition, documentFormatterId));
                //    }
                //}

                //// Components
                //if (!device.Components.IsNullOrEmpty())
                //{
                //    foreach (var component in device.Components)
                //    {
                //        messages.AddRange(CreateMessages(topic, component, documentFormatterId));
                //    }
                //}

                return messages;
            }

            return null;
        }

        //private IEnumerable<MqttApplicationMessage> CreateMessages(string parentTopic, IComponent component, string documentFormatterId = DocumentFormat.XML)
        //{
        //    var messages = new List<MqttApplicationMessage>();

        //    if (component != null)
        //    {
        //        var topic = $"{parentTopic}/Components/{component.Type}/{component.Id}";
        //        messages.Add(CreateMessage(topic, Formatters.EntityFormatter.Format(documentFormatterId, component)));

        //        // DataItems
        //        if (!component.DataItems.IsNullOrEmpty())
        //        {
        //            foreach (var dataItem in component.DataItems)
        //            {
        //                var dataItemTopic = $"{topic}/DataItems/{dataItem.Type}/{dataItem.Id}";
        //                if (!string.IsNullOrEmpty(dataItem.SubType)) dataItemTopic = $"{topic}/DataItems/{dataItem.Type}/SubTypes/{dataItem.SubType}/{dataItem.Id}";

        //                messages.Add(CreateMessage(dataItemTopic, Formatters.EntityFormatter.Format(documentFormatterId, dataItem)));
        //            }
        //        }

        //        // Compositions
        //        if (!component.Compositions.IsNullOrEmpty())
        //        {
        //            foreach (var composition in component.Compositions)
        //            {
        //                messages.AddRange(CreateMessages(topic, composition, documentFormatterId));
        //            }
        //        }

        //        // Components
        //        if (!component.Components.IsNullOrEmpty())
        //        {
        //            foreach (var subcomponent in component.Components)
        //            {
        //                messages.AddRange(CreateMessages(topic, subcomponent, documentFormatterId));
        //            }
        //        }

        //        return messages;
        //    }

        //    return messages;
        //}

        //private IEnumerable<MqttApplicationMessage> CreateMessages(string parentTopic, IComposition composition, string documentFormatterId = DocumentFormat.XML)
        //{
        //    var messages = new List<MqttApplicationMessage>();

        //    if (composition != null)
        //    {
        //        var topic = $"{parentTopic}/Compositions/{composition.Type}/{composition.Id}";
        //        messages.Add(CreateMessage(topic, Formatters.EntityFormatter.Format(documentFormatterId, composition)));

        //        // DataItems
        //        if (!composition.DataItems.IsNullOrEmpty())
        //        {
        //            foreach (var dataItem in composition.DataItems)
        //            {
        //                var dataItemTopic = $"{topic}/DataItems/{dataItem.Type}/{dataItem.Id}";
        //                if (!string.IsNullOrEmpty(dataItem.SubType)) dataItemTopic = $"{topic}/DataItems/{dataItem.Type}/SubTypes/{dataItem.SubType}/{dataItem.Id}";

        //                messages.Add(CreateMessage(dataItemTopic, Formatters.EntityFormatter.Format(documentFormatterId, dataItem)));
        //            }
        //        }

        //        return messages;
        //    }

        //    return messages;
        //}

        private MqttApplicationMessage CreateMessage(IObservation observation, string documentFormatterId = DocumentFormat.XML)
        {
            if (observation != null && observation.DataItem != null && !observation.Values.IsNullOrEmpty())
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

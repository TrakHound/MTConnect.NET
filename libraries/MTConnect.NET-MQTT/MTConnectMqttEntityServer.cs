// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Observations;
using System.IO;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public class MTConnectMqttEntityServer
    {
        private readonly IMTConnectMqttEntityServerConfiguration _configuration;

        public string TopicPrefix => _configuration.TopicPrefix;


        public MTConnectMqttEntityServer(string topicPrefix = null, string documentFormat = DocumentFormat.JSON)
        {
            var configuration = new MTConnectMqttEntityServerConfiguration();
            configuration.TopicPrefix = topicPrefix;
            configuration.DocumentFormat = documentFormat;
            _configuration = configuration;
        }

        public MTConnectMqttEntityServer(IMTConnectMqttEntityServerConfiguration configuration)
        {
            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttEntityServerConfiguration();
        }


        public async Task PublishDevice(IMqttClient mqttClient, IDevice device)
        {
            if (mqttClient != null && mqttClient.IsConnected && device != null)
            {
                var message = CreateMessage(device);
                var result = await mqttClient.PublishAsync(message);
                if (result.IsSuccess)
                {

                }
                else
                {

                }
            }
        }

        public async Task PublishDevice(MqttServer mqttServer, IDevice device)
        {
            if (mqttServer != null && device != null)
            {
                var message = CreateMessage(device);
                var injectedMessage = new InjectedMqttApplicationMessage(message);

                await mqttServer.InjectApplicationMessage(injectedMessage);
            }
        }

        private MqttApplicationMessage CreateMessage(IDevice device)
        {
            if (device != null)
            {
                var topic = $"{_configuration.TopicPrefix}/Devices/{device.Uuid}/Device";

                var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, device);
                if (formatResult.Success && formatResult.Content != null)
                {
                    if (formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithPayload(formatResult.Content);
                    messageBuilder.WithRetainFlag(true);
                    return messageBuilder.Build();
                }
            }

            return null;
        }


        public async Task PublishObservation(IMqttClient mqttClient, IObservation observation)
        {
            if (mqttClient != null && mqttClient.IsConnected && observation != null)
            {
                var message = CreateMessage(observation);
                var result = await mqttClient.PublishAsync(message);
                if (result.IsSuccess)
                {

                }
                else
                {

                }
            }
        }

        public async Task PublishObservation(MqttServer mqttServer, IObservation observation)
        {
            if (mqttServer != null && observation != null)
            {
                var message = CreateMessage(observation);
                var injectedMessage = new InjectedMqttApplicationMessage(message);

                await mqttServer.InjectApplicationMessage(injectedMessage);
            }
        }

        private MqttApplicationMessage CreateMessage(IObservation observation)
        {
            if (observation != null)
            {
                var device = observation.DataItem?.Device;
                if (device != null)
                {
                    var topic = $"{_configuration.TopicPrefix}/Devices/{device.Uuid}/Observations/{observation.DataItemId}";

                    var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, observation);
                    if (formatResult.Success)
                    {
                        if (formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithPayload(formatResult.Content);
                        messageBuilder.WithRetainFlag(true);
                        return messageBuilder.Build();
                    }
                }
            }

            return null;
        }


        public async Task PublishAsset(IMqttClient mqttClient, IAsset asset)
        {
            if (mqttClient != null && mqttClient.IsConnected && asset != null)
            {
                var message = CreateMessage(asset);
                var result = await mqttClient.PublishAsync(message);
                if (result.IsSuccess)
                {

                }
                else
                {

                }
            }
        }

        public async Task PublishAsset(MqttServer mqttServer, IAsset asset)
        {
            if (mqttServer != null && asset != null)
            {
                var message = CreateMessage(asset);
                var injectedMessage = new InjectedMqttApplicationMessage(message);

                await mqttServer.InjectApplicationMessage(injectedMessage);
            }
        }

        private MqttApplicationMessage CreateMessage(IAsset asset)
        {
            if (asset != null)
            {
                var topic = $"{_configuration.TopicPrefix}/Devices/{asset.DeviceUuid}/Assets/{asset.AssetId}";

                var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, asset);
                if (formatResult.Success)
                {
                    if (formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                    var messageBuilder = new MqttApplicationMessageBuilder();
                    messageBuilder.WithTopic(topic);
                    messageBuilder.WithPayload(formatResult.Content);
                    messageBuilder.WithRetainFlag(true);
                    return messageBuilder.Build();
                }
            }

            return null;
        }
    }
}
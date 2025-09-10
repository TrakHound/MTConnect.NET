// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    public class MTConnectMqttEntityServer
    {
        private readonly IMTConnectMqttEntityServerConfiguration _configuration;

        public string TopicPrefix => _configuration.TopicPrefix;

        public event EventHandler<string> MessageSent;
        public event EventHandler<string> SendError;
        public event EventHandler<Exception> ClientError;
        public event EventHandler<Exception> ServerError;


        public MTConnectMqttEntityServer(string topicPrefix = null, string documentFormat = DocumentFormat.JSON, int qos = 0)
        {
            var configuration = new MTConnectMqttEntityServerConfiguration();
            configuration.TopicPrefix = topicPrefix;
            configuration.DocumentFormat = documentFormat;
            configuration.Qos = qos;
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
                try
                {
                    var message = CreateMessage(device);
                    var result = await mqttClient.PublishAsync(message);
                    if (result.IsSuccess)
                    {
                        if (MessageSent != null) MessageSent.Invoke(this, message.Topic);
                    }
                    else
                    {
                        if (SendError != null) SendError.Invoke(this, message.Topic);
                    }
                }
                catch (Exception ex)
                {
                    if (ClientError != null) ClientError.Invoke(this, ex);
                }
            }
        }

        public async Task PublishDevice(MqttServer mqttServer, IDevice device)
        {
            if (mqttServer != null && device != null)
            {
                try
                {
                    var message = CreateMessage(device);
                    var injectedMessage = new InjectedMqttApplicationMessage(message);

                    await mqttServer.InjectApplicationMessage(injectedMessage);
                }
                catch (Exception ex)
                {
                    if (ServerError != null) ServerError.Invoke(this, ex);
                }
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
                    messageBuilder.WithQualityOfServiceLevel(GetQualityOfService(_configuration.Qos));
                    return messageBuilder.Build();
                }
            }

            return null;
        }


        public async Task<MqttClientPublishResult> PublishObservation(IMqttClient mqttClient, IObservation observation)
        {
            if (mqttClient != null && mqttClient.IsConnected && observation != null)
            {
                try
                {
                    var message = CreateMessage(observation);
                    var result = await mqttClient.PublishAsync(message);
                    if (result.IsSuccess)
                    {
                        if (MessageSent != null) MessageSent.Invoke(this, message.Topic);
                    }
                    else
                    {
                        if (SendError != null) SendError.Invoke(this, message.Topic);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    if (ClientError != null) ClientError.Invoke(this, ex);
                }
            }
            return null;
        }

        public async Task<MqttClientPublishResult> PublishObservations(IMqttClient mqttClient, IEnumerable<IObservation> observations)
        {
            if (mqttClient != null && mqttClient.IsConnected && !observations.IsNullOrEmpty())
            {
                try
                {
                    var message = CreateMessage(observations);
                    var result = await mqttClient.PublishAsync(message);
                    if (result.IsSuccess)
                    {
                        if (MessageSent != null) MessageSent.Invoke(this, message.Topic);
                    }
                    else
                    {
                        if (SendError != null) SendError.Invoke(this, message.Topic);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    if (ClientError != null) ClientError.Invoke(this, ex);
                }
            }
            return null;
        }

        public async Task PublishObservation(MqttServer mqttServer, IObservation observation)
        {
            if (mqttServer != null && observation != null)
            {
                try
                {
                    var message = CreateMessage(observation);
                    var injectedMessage = new InjectedMqttApplicationMessage(message);

                    await mqttServer.InjectApplicationMessage(injectedMessage);
                }
                catch (Exception ex)
                {
                    if (ServerError != null) ServerError.Invoke(this, ex);
                }
            }
        }

        public async Task PublishObservations(MqttServer mqttServer, IEnumerable<IObservation> observations)
        {
            if (mqttServer != null && !observations.IsNullOrEmpty())
            {
                try
                {
                    var message = CreateMessage(observations);
                    var injectedMessage = new InjectedMqttApplicationMessage(message);

                    await mqttServer.InjectApplicationMessage(injectedMessage);
                }
                catch (Exception ex)
                {
                    if (ServerError != null) ServerError.Invoke(this, ex);
                }
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

                    var formatOptions = new Dictionary<string, string>();
                    formatOptions.Add("categoryOutput", "true");
                    formatOptions.Add("instanceIdOutput", "true");

                    var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, observation, formatOptions);
                    if (formatResult.Success)
                    {
                        if (formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithPayload(formatResult.Content);
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithQualityOfServiceLevel(GetQualityOfService(_configuration.Qos));
                        return messageBuilder.Build();
                    }
                }
            }

            return null;
        }

        private MqttApplicationMessage CreateMessage(IEnumerable<IObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var observation = observations.FirstOrDefault();
                var device = observation.DataItem?.Device;
                if (device != null)
                {
                    var topic = $"{_configuration.TopicPrefix}/Devices/{device.Uuid}/Observations/{observation.DataItemId}";

                    var formatOptions = new Dictionary<string, string>();
                    formatOptions.Add("categoryOutput", "true");
                    formatOptions.Add("instanceIdOutput", "true");

                    var formatResult = EntityFormatter.Format(_configuration.DocumentFormat, observations, formatOptions);
                    if (formatResult.Success)
                    {
                        if (formatResult.Content.Position > 0) formatResult.Content.Seek(0, SeekOrigin.Begin);

                        var messageBuilder = new MqttApplicationMessageBuilder();
                        messageBuilder.WithTopic(topic);
                        messageBuilder.WithPayload(formatResult.Content);
                        messageBuilder.WithRetainFlag(true);
                        messageBuilder.WithQualityOfServiceLevel(GetQualityOfService(_configuration.Qos));
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
                try
                {
                    var message = CreateMessage(asset);
                    var result = await mqttClient.PublishAsync(message);
                    if (result.IsSuccess)
                    {
                        if (MessageSent != null) MessageSent.Invoke(this, message.Topic);
                    }
                    else
                    {
                        if (SendError != null) SendError.Invoke(this, message.Topic);
                    }
                }
                catch (Exception ex)
                {
                    if (ClientError != null) ClientError.Invoke(this, ex);
                }
            }
        }

        public async Task PublishAsset(MqttServer mqttServer, IAsset asset)
        {
            if (mqttServer != null && asset != null)
            {
                try
                {
                    var message = CreateMessage(asset);
                    var injectedMessage = new InjectedMqttApplicationMessage(message);

                    await mqttServer.InjectApplicationMessage(injectedMessage);
                }
                catch (Exception ex)
                {
                    if (ServerError != null) ServerError.Invoke(this, ex);
                }
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
                    messageBuilder.WithQualityOfServiceLevel(GetQualityOfService(_configuration.Qos));
                    return messageBuilder.Build();
                }
            }

            return null;
        }

        private static MQTTnet.Protocol.MqttQualityOfServiceLevel GetQualityOfService(int qos)
        {
            if (qos == 1) return MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
            else if (qos == 2) return MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce;
            else return MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce;
        }
    }
}
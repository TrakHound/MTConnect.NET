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
    /// <summary>
    /// Publishes individual MTConnect entities (devices, observations, assets) over MQTT under
    /// their dedicated topics rather than as packaged response documents. Each <c>Publish*</c>
    /// method comes in two flavours: an <see cref="IMqttClient"/> overload for outbound publishes
    /// to an external broker, and an <see cref="MqttServer"/> overload that injects the message
    /// directly into an embedded broker via <see cref="InjectedMqttApplicationMessage"/>.
    /// </summary>
    public class MTConnectMqttEntityServer
    {
        private readonly IMTConnectMqttEntityServerConfiguration _configuration;

        /// <summary>The MQTT topic prefix used for every publish (sourced from the configuration).</summary>
        public string TopicPrefix => _configuration.TopicPrefix;

        /// <summary>Raised after each successful publish with the topic of the message that was sent.</summary>
        public event EventHandler<string> MessageSent;

        /// <summary>Raised when a publish completes but the broker reports a non-success result; the argument is the topic that failed.</summary>
        public event EventHandler<string> SendError;

        /// <summary>Raised when an <see cref="IMqttClient"/> publish throws (transport failure, malformed payload).</summary>
        public event EventHandler<Exception> ClientError;

        /// <summary>Raised when an <see cref="MqttServer"/> inject throws (the embedded broker rejected the message).</summary>
        public event EventHandler<Exception> ServerError;


        /// <summary>Constructs an entity server with a fresh configuration populated from the supplied loose arguments.</summary>
        /// <param name="topicPrefix">Topic prefix to use; null or empty keeps the default.</param>
        /// <param name="documentFormat">Entity serialisation format; defaults to JSON.</param>
        /// <param name="qos">MQTT Quality of Service level applied to every publish.</param>
        public MTConnectMqttEntityServer(string topicPrefix = null, string documentFormat = DocumentFormat.JSON, int qos = 0)
        {
            var configuration = new MTConnectMqttEntityServerConfiguration();
            configuration.TopicPrefix = topicPrefix;
            configuration.DocumentFormat = documentFormat;
            configuration.Qos = qos;
            _configuration = configuration;
        }

        /// <summary>Constructs an entity server from an existing configuration object; a null argument falls back to a freshly-initialised default configuration.</summary>
        /// <param name="configuration">The configuration providing topic prefix, document format, and QoS.</param>
        public MTConnectMqttEntityServer(IMTConnectMqttEntityServerConfiguration configuration)
        {
            _configuration = configuration;
            if (_configuration == null) _configuration = new MTConnectMqttEntityServerConfiguration();
        }


        /// <summary>
        /// Publishes a single device document to <c>{TopicPrefix}/Devices/{uuid}/Device</c> via
        /// the supplied <paramref name="mqttClient"/>. The message is retained so newly
        /// connecting clients see the latest device model. Raises <see cref="MessageSent"/>,
        /// <see cref="SendError"/>, or <see cref="ClientError"/> depending on the outcome.
        /// </summary>
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

        /// <summary>
        /// Injects a single device document into an embedded <see cref="MqttServer"/> using
        /// <see cref="InjectedMqttApplicationMessage"/>, avoiding the network round-trip when the
        /// publisher hosts the broker itself. Raises <see cref="ServerError"/> on failure.
        /// </summary>
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


        /// <summary>
        /// Publishes a single observation to
        /// <c>{TopicPrefix}/Devices/{deviceUuid}/Observations/{dataItemId}</c> via the supplied
        /// <paramref name="mqttClient"/>. Returns the broker's publish result so callers can
        /// inspect acknowledgement state when QoS &gt; 0 is in effect.
        /// </summary>
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

        /// <summary>
        /// Publishes a batch of observations that share the same DataItem (and therefore the same
        /// topic) via the supplied <paramref name="mqttClient"/>. The topic is derived from the
        /// first observation; remaining observations contribute to the serialised payload only.
        /// </summary>
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

        /// <summary>Injects a single observation into an embedded <see cref="MqttServer"/> without a network round-trip. Raises <see cref="ServerError"/> on failure.</summary>
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

        /// <summary>Injects a batch of observations (sharing the same DataItem topic) into an embedded <see cref="MqttServer"/>. Raises <see cref="ServerError"/> on failure.</summary>
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


        /// <summary>
        /// Publishes a single asset to <c>{TopicPrefix}/Devices/{deviceUuid}/Assets/{assetId}</c>
        /// via the supplied <paramref name="mqttClient"/>. The message is retained so consumers
        /// joining after the publish still see the asset.
        /// </summary>
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

        /// <summary>Injects a single asset into an embedded <see cref="MqttServer"/> without a network round-trip. Raises <see cref="ServerError"/> on failure.</summary>
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
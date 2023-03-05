// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public MTConnectMqttFormat Format { get; set; }

        public bool RetainMessages { get; set; }

        public bool AllowUntrustedCertificates => _configuration.AllowUntrustedCertificates;

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

            Format = MTConnectMqttFormat.Hierarchy;
            RetainMessages = true;

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
                        // Declare new MQTT Client Options with Tcp Server
                        var clientOptionsBuilder = new MqttClientOptionsBuilder().WithTcpServer(_configuration.Server, _configuration.Port);

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_configuration.ClientId))
                        {
                            clientOptionsBuilder.WithClientId(_configuration.ClientId);
                        }

                        var certificates = new List<X509Certificate2>();

                        // Add CA (Certificate Authority)
                        if (!string.IsNullOrEmpty(_configuration.CertificateAuthority))
                        {
                            certificates.Add(new X509Certificate2(GetFilePath(_configuration.CertificateAuthority)));
                        }

                        // Add Client Certificate & Private Key
                        if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
                        {

#if NET5_0_OR_GREATER
                            certificates.Add(new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx)));
#else
                            throw new Exception("PEM Certificates Not Supported in .NET Framework 4.8 or older");
#endif

                            clientOptionsBuilder.WithCleanSession();
                            clientOptionsBuilder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                            {
                                UseTls = true,
                                SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                                IgnoreCertificateRevocationErrors = AllowUntrustedCertificates,
                                IgnoreCertificateChainErrors = AllowUntrustedCertificates,
                                AllowUntrustedCertificates = AllowUntrustedCertificates,
                                Certificates = certificates
                            });
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password).WithTls();
                            }
                            else
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                        }

                        // Build MQTT Client Options
                        var clientOptions = clientOptionsBuilder.Build();

                        await _mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

                        if (Connected != null) Connected.Invoke(this, new EventArgs());

                        // Publish MTConnect Agent Information
                        await PublishAgent(_mtconnectAgent);

                        // Add Agent Devices
                        var devices = _mtconnectAgent.GetDevices();
                        if (!devices.IsNullOrEmpty())
                        {
                            foreach (var device in devices)
                            {
                                await PublishDevice(device);
                            }
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

        public void Dispose()
        {
            if (_mqttClient != null) _mqttClient.Dispose();
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
                var messageBuilder = new MqttApplicationMessageBuilder();
                messageBuilder.WithTopic(topic);
                messageBuilder.WithPayload(payload);
                messageBuilder.WithRetainFlag(RetainMessages);
                return messageBuilder.Build();
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
            if (observation != null && !string.IsNullOrEmpty(observation.DeviceUuid) && observation.DataItem != null && observation.DataItem.Container != null && !observation.Values.IsNullOrEmpty())
            {
                var topic = CreateTopic(observation);

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

        private string CreateTopic(IObservation observation)
        {
            if (observation != null)
            {
                var type = DataItem.GetPascalCaseType(observation.DataItem.Type);
                var subtype = DataItem.GetPascalCaseType(observation.DataItem.SubType);

                var category = observation.Category.ToString().ToTitleCase() + "s";

                // Add Prefixes
                var prefixes = new List<string>();
                prefixes.Add("MTConnect");
                prefixes.Add("Devices");
                prefixes.Add(observation.DeviceUuid);
                prefixes.Add("Observations");

                var paths = new List<string>();

                // Add Container
                paths.Add(RemoveNamespacePrefix(observation.DataItem.Container.Type));
                paths.Add(observation.DataItem.Container.Id);
                paths.Add(category);

                // Add Type
                paths.Add(RemoveNamespacePrefix(type));

                // Add SubType
                if (!string.IsNullOrEmpty(subtype))
                {
                    paths.Add("SubTypes");
                    paths.Add(RemoveNamespacePrefix(subtype));
                }

                // Add DataItemId
                paths.Add(observation.DataItemId);


                switch (Format)
                {
                    case MTConnectMqttFormat.Hierarchy:

                        return string.Join("/", new string[]
                        {
                            string.Join("/", prefixes),
                            string.Join("/", paths),
                            observation.DataItemId
                        });

                    default:
                        return string.Join("/", new string[]
{
                            string.Join("/", prefixes),
                            observation.DataItemId
});
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


        private static string GetFilePath(string path)
        {
            var x = path;
            if (!Path.IsPathRooted(x))
            {              
                x = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, x);
            }

            return x;
        }

        private static string RemoveNamespacePrefix(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Contains(':'))
                {
                    return type.Substring(type.IndexOf(':') + 1);
                }

                return type;
            }

            return null;
        }
    }
}
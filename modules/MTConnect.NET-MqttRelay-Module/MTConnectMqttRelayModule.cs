// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Mqtt;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTConnect.Controllers.Mqtt
{
    public class MTConnectMqttRelayModule : IMTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-relay";

        private readonly Logger _mqttLogger = LogManager.GetLogger("mqtt-relay-logger");
        //private readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private readonly MqttRelayConfiguration _configuration;
        private IMTConnectAgentBroker _mtconnectAgent;
        private MTConnectMqttRelay _relay;


        public string Id { get; }

        public string Description { get; }


        public MTConnectMqttRelayModule(IMTConnectAgentBroker mtconnectAgent, object configuration)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttRelayConfiguration>(configuration);
        }


        public void StartBeforeLoad() { }

        public void StartAfterLoad()
        {
            _ = Task.Run(StartAsync);
        }

        public void Stop()
        {
            if (_relay != null)
            {
                _relay.Stop();
                _relay.Dispose();
            }
        }


        private async Task StartAsync()
        {
            if (_configuration != null)
            {
                var clientConfiguration = new MTConnectMqttClientConfiguration();
                clientConfiguration.Server = _configuration.Server;
                clientConfiguration.Port = _configuration.Port;
                //clientConfiguration.Port = _port > 0 ? _port : _configuration.Port;
                clientConfiguration.QoS = _configuration.QoS;
                clientConfiguration.Username = _configuration.Username;
                clientConfiguration.Password = _configuration.Password;
                clientConfiguration.ClientId = _configuration.ClientId;
                clientConfiguration.CertificateAuthority = _configuration.CertificateAuthority;
                clientConfiguration.PemCertificate = _configuration.PemCertificate;
                clientConfiguration.PemPrivateKey = _configuration.PemPrivateKey;
                clientConfiguration.UseTls = _configuration.UseTls;
                clientConfiguration.AllowUntrustedCertificates = _configuration.AllowUntrustedCertificates;
                clientConfiguration.TopicPrefix = _configuration.TopicPrefix;


                // Set Observation Intervals
                IEnumerable<int> observationIntervals = new List<int> { 0, 1000 };
                if (!_configuration.ObservationIntervals.IsNullOrEmpty()) observationIntervals = _configuration.ObservationIntervals;


                _relay = new MTConnectMqttRelay(_mtconnectAgent, clientConfiguration, observationIntervals);
                _relay.Format = _configuration.MqttFormat;
                _relay.RetainMessages = _configuration.RetainMessages;
                _relay.Connected += MqttClientConnected;
                _relay.Disconnected += MqttClientDisconnected;
                _relay.MessageSent += MqttClientMessageSent;
                _relay.PublishError += MqttClientPublishError;
                _relay.ConnectionError += MqttClientConnectionError;
                _relay.Start();
            }
        }

        private static string GetFilePath(string path)
        {
            var x = path;
            if (!Path.IsPathRooted(x))
            {
                x = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, x);
            }

            return x;
        }


        #region "Logging"

        private void MqttClientConnected(object sender, EventArgs args)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Info($"[MQTT-Relay] : MQTT Client Connected : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientDisconnected(object sender, EventArgs args)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Debug($"[MQTT-Relay] : MQTT Client Disconnected : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientMessageSent(object sender, string message)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Debug($"[MQTT-Relay] : MQTT Client Message Sent : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientPublishError(object sender, Exception exception)
        {
            _mqttLogger.Warn($"[MQTT-Relay] : MQTT Client Publish Error : " + exception.Message);
        }

        private void MqttClientConnectionError(object sender, Exception exception)
        {
            _mqttLogger.Warn($"[MQTT-Relay] : MQTT Client Connection Error : " + exception.Message);
        }

        #endregion

    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Mqtt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Modules
{
    public class MTConnectMqttBrokerModule : IMTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt-broker";

        //private readonly Logger _httpLogger = LogManager.GetLogger("http-logger");
        //private readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private readonly MqttBrokerConfiguration _configuration;
        private IMTConnectAgentBroker _mtconnectAgent;
        private MqttServer _mqttServer;
        private MTConnectMqttBroker _mqttBroker;


        public string Id { get; }

        public string Description { get; }


        public MTConnectMqttBrokerModule(IMTConnectAgentBroker mtconnectAgent, object configuration)
        {
            _mtconnectAgent = mtconnectAgent;
            _configuration = AgentApplicationConfiguration.GetConfiguration<MqttBrokerConfiguration>(configuration);
        }


        public void StartBeforeLoad() { }

        public void StartAfterLoad()
        {
            _ = Task.Run(StartAsync);
        }

        public void Stop()
        {
            if (_mqttBroker != null) _mqttBroker.StopAsync(CancellationToken.None);
            if (_mqttServer != null) _mqttServer.StopAsync();
        }


        private async Task StartAsync()
        {
            var mqttServerOptionsBuilder = new MqttServerOptionsBuilder().WithDefaultEndpoint();

            // Add Certificate & Private Key
            if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
            {
                X509Certificate2 certificate = null;

#if NET5_0_OR_GREATER
                certificate = new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx));
#endif

                if (certificate != null)
                {
                    mqttServerOptionsBuilder.WithoutDefaultEndpoint();
                    mqttServerOptionsBuilder.WithEncryptedEndpoint();
                    mqttServerOptionsBuilder.WithEncryptedEndpointPort(_configuration.Port);
                    mqttServerOptionsBuilder.WithEncryptionCertificate(certificate);
                    mqttServerOptionsBuilder.WithEncryptionSslProtocol(System.Security.Authentication.SslProtocols.Tls12);
                }
            }

            var mqttServerOptions = mqttServerOptionsBuilder.Build();

            var mqttFactory = new MqttFactory();
            _mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);


            // Set Observation Intervals
            IEnumerable<int> observationIntervals = new List<int> { 0, 1000 };
            if (!_configuration.ObservationIntervals.IsNullOrEmpty()) observationIntervals = _configuration.ObservationIntervals;

            _mqttBroker = new MTConnectMqttBroker(_mtconnectAgent, _mqttServer, observationIntervals);
            _mqttBroker.Format = _configuration.MqttFormat;
            _mqttBroker.RetainMessages = _configuration.RetainMessages;
            _mqttBroker.TopicPrefix = _configuration.TopicPrefix;
            await _mqttBroker.StartAsync(CancellationToken.None);
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
    }
}
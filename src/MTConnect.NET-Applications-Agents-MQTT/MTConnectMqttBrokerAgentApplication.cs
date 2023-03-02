// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MTConnect.Configurations;
using MTConnect.Mqtt;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent application with a built in MQTT broker.
    /// Supports Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectMqttBrokerAgentApplication : MTConnectMqttAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-MQTT-Broker";
        private const string DefaultServiceDisplayName = "MTConnect MQTT Broker Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using MQTT to provide access to device information using the MTConnect Standard";


        private readonly Logger _mqttLogger = LogManager.GetLogger("mqtt-logger");

        private MqttServer _mqttServer;
        private MTConnectMqttBroker _mqttBroker;
        private IMqttAgentApplicationConfiguration _configuration;
        private int _port = 0;


        public MTConnectMqttBrokerAgentApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;

            if (ConfigurationType == null) ConfigurationType = typeof(MqttAgentApplicationConfiguration);
        }


        /// <summary>
        /// Add the Http Port as the 3rd command line argument
        /// </summary>
        /// <param name="args"></param>
        protected override void OnCommandLineArgumentsRead(string[] args)
        {
            int port = 0;

            if (!args.IsNullOrEmpty())
            {
                // Port
                if (args.Length > 2)
                {
                    if (int.TryParse(args[2], out var p))
                    {
                        port = p;
                        _applicationLogger.Info($"Agent MQTT Port = {port}");
                    }
                }
            }

            _port = port;
        }

        protected override IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            var configuration = AgentConfiguration.Read<MqttAgentApplicationConfiguration>(configurationPath);
            base.OnAgentConfigurationUpdated(configuration);
            _configuration = configuration;
            return _configuration;
        }

        protected override void OnAgentConfigurationWatcherInitialize(IAgentApplicationConfiguration configuration)
        {
            _agentConfigurationWatcher = new AgentConfigurationFileWatcher<MqttAgentApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }

        protected override void OnAgentConfigurationUpdated(AgentConfiguration configuration)
        {
            _configuration = configuration as IMqttAgentApplicationConfiguration;
        }


        protected override async void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            var mqttServerOptionsBuilder = new MqttServerOptionsBuilder().WithDefaultEndpoint();

            // Add Certificate & Private Key
            if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
            {
                var certificate = Certificates.FromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey));
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

            _mqttBroker = new MTConnectMqttBroker(Agent, _mqttServer);
            await _mqttBroker.StartAsync(CancellationToken.None);
        }

        protected override void OnStopAgent()
        {
            if (_mqttBroker != null) _mqttBroker.StopAsync(CancellationToken.None);
            if (_mqttServer != null) _mqttServer.StopAsync();
        }


        #region "Help"

        protected override string OnPrintHelpUsage()
        {
            return "[mqtt_port]";
        }

        protected override void OnPrintHelpArguments()
        {
            Console.WriteLine(string.Format("{0,20}  |  {1,5}", "mqtt_port", "Specifies the TCP Port to use for the MQTT Broker"));
            Console.WriteLine(string.Format("{0,20}     {1,5}", "", "Note : This overrides what is read from the Configuration file"));
        }

        #endregion

        #region "Logging"

        private void MqttClientConnected(object sender, EventArgs args)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Info($"[MQTT Broker] : MQTT Client Connected : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientDisconnected(object sender, EventArgs args)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Debug($"[MQTT Broker] : MQTT Client Disconnected : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientMessageSent(object sender, string message)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Debug($"[MQTT Broker] : MQTT Client Message Sent : " + relay.Server + " : " + relay.Port);
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
    }
}
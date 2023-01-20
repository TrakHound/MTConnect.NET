// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Mqtt;
using NLog;
using System;
using System.Collections.Generic;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent application with a built in MQTT client to relay messages to a MQTT Broker.
    /// Supports Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectMqttRelayAgentApplication : MTConnectMqttAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-MQTT-Relay";
        private const string DefaultServiceDisplayName = "MTConnect MQTT Relay Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using MQTT to publish to a MQTT Broker";


        private readonly Logger _mqttLogger = LogManager.GetLogger("mqtt-logger");

        private MTConnectMqttRelay _relay;
        private IMqttAgentApplicationConfiguration _configuration;
        private int _port = 0;


        public MTConnectMqttRelayAgentApplication()
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


        protected override async void OnStartAgentAfterLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            if (_configuration != null)
            {
                var clientConfiguration = new MTConnectMqttClientConfiguration();
                clientConfiguration.Server = _configuration.Server;
                clientConfiguration.Port = _port > 0 ? _port : _configuration.Port;
                clientConfiguration.Username = _configuration.Username;
                clientConfiguration.Password = _configuration.Password;
                clientConfiguration.UseTls = _configuration.UseTls;

                _relay = new MTConnectMqttRelay(Agent, clientConfiguration);
                _relay.Connected += MqttClientConnected;
                _relay.Disconnected += MqttClientDisconnected;
                _relay.MessageSent += MqttClientMessageSent;
                _relay.PublishError += MqttClientPublishError;
                _relay.ConnectionError += MqttClientConnectionError;
                _relay.Start();
            }

            base.OnStartAgentAfterLoad(devices, initializeDataItems);
        }

        protected override void OnStopAgent()
        {
            if (_relay != null) _relay.Dispose();
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
            _mqttLogger.Info($"[MQTT Client] : MQTT Client Connected : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientDisconnected(object sender, EventArgs args)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Debug($"[MQTT Client] : MQTT Client Disconnected : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientMessageSent(object sender, string message)
        {
            var relay = (MTConnectMqttRelay)sender;
            _mqttLogger.Debug($"[MQTT Client] : MQTT Client Message Sent : " + relay.Server + " : " + relay.Port);
        }

        private void MqttClientPublishError(object sender, Exception exception)
        {
            _mqttLogger.Warn($"[MQTT Client] : MQTT Client Publish Error : " + exception.Message);
        }

        private void MqttClientConnectionError(object sender, Exception exception)
        {
            _mqttLogger.Warn($"[MQTT Client] : MQTT Client Connection Error : " + exception.Message);
        }

        #endregion

    }
}
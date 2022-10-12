// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MQTTnet;
using MQTTnet.Server;
using MTConnect.Configurations;
using MTConnect.Mqtt;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent application with a built in MQTT client to relay messages to a MQTT Broker.
    /// Supports Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectMqttRelayAgentApplication : MTConnectAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-MQTT-Relay";
        private const string DefaultServiceDisplayName = "MTConnect MQTT Relay Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using MQTT to provide access to device information using the MTConnect Standard";


        private readonly Logger _mqttLogger = LogManager.GetLogger("mqtt-logger");

        private MTConnectMqttRelay _relay;
        private IHttpAgentApplicationConfiguration _configuration;
        private int _port = 0;


        public MTConnectMqttRelayAgentApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;
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

        //protected override IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        //{
        //    // Read the Configuration File
        //    var configuration = AgentConfiguration.Read<HttpAgentApplicationConfiguration>(configurationPath);
        //    base.OnAgentConfigurationUpdated(configuration);
        //    _configuration = configuration;
        //    return _configuration;
        //}

        //protected override void OnAgentConfigurationWatcherInitialize(IAgentApplicationConfiguration configuration)
        //{
        //    _agentConfigurationWatcher = new AgentConfigurationFileWatcher<HttpAgentApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        //}

        //protected override void OnAgentConfigurationUpdated(AgentConfiguration configuration)
        //{
        //    _configuration = configuration as IHttpAgentApplicationConfiguration;
        //}

        //protected virtual MTConnectHttpAgentServer OnHttpServerInitialize(int port)
        //{
        //    return new MTConnectHttpAgentServer(_configuration, Agent, null, port);
        //}

        protected override async void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            //_relay = new MTConnectMqttRelay(Agent, "5679887d308d402888f323be02124836.s1.eu.hivemq.cloud", 8883);
            _relay = new MTConnectMqttRelay(Agent, "localhost", 1883);
            await _relay.Connect();

            base.OnStartAgentBeforeLoad(devices, initializeDataItems);
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

    }
}
// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MQTTnet;
using MQTTnet.Server;
using MTConnect.Agents;
using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent Application that reads from other MTConnect Agents and acts as a Gateway or Pass-through broker
    /// </summary>
    public class MTConnectShdrMqttAgentApplication : MTConnectShdrHttpAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP-Gateway";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Gateway Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using HTTP to provide access to device information using the MTConnect Standard";
        private const int ClientInformationUpdateInterval = 5000;


        public MTConnectShdrMqttAgentApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;
        }

        protected override async void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            _ = Task.Run(async () =>
            {
                var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

                var mqttFactory = new MqttFactory();
                using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
                {
                    var broker = new MTConnectMqttBroker(Agent, mqttServer);
                    await broker.StartAsync(CancellationToken.None);

                    //var serverOptions = new MqttServerOptions();
                    //await mqttServer.StartAsync(serverOptions);

                    Console.WriteLine("Press Enter to exit.");
                    Console.ReadLine();

                    await broker.StopAsync(CancellationToken.None);

                    //// Stop and dispose the MQTT server if it is no longer needed!
                    //await mqttServer.StopAsync();
                }
            });

            base.OnStartAgentBeforeLoad(devices, initializeDataItems);
        }
    }
}
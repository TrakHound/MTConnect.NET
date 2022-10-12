// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Configurations;
using MTConnect.Mqtt;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect Agent Application that reads from other MTConnect Agents and acts as a Gateway or Pass-through broker
    /// </summary>
    public class MTConnectShdrMqttAgentRelayApplication : MTConnectShdrHttpAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP-Gateway";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Gateway Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using HTTP to provide access to device information using the MTConnect Standard";

        private MTConnectMqttRelay _relay;


        public MTConnectShdrMqttAgentRelayApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;
        }

        protected override async void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false)
        {
            //_relay = new MTConnectMqttRelay(Agent, "5679887d308d402888f323be02124836.s1.eu.hivemq.cloud", 8883);
            _relay = new MTConnectMqttRelay(Agent, "localhost", 1883);
            await _relay.Connect();

            base.OnStartAgentBeforeLoad(devices, initializeDataItems);
        }
    }
}
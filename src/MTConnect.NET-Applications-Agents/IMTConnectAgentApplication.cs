// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using System;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An interface for an MTConnect Agent Application
    /// </summary>
    public interface IMTConnectAgentApplication
    {
        string ServiceName { get; }

        string ServiceDisplayName { get; }

        string ServiceDescription { get; }


        IMTConnectAgentBroker Agent { get; }

        EventHandler<AgentConfiguration> OnRestart { get; set; }



        /// <summary>
        /// Start the Agent Application
        /// </summary>
        void StartAgent(string configurationPath, bool verboseLogging = false);

        /// <summary>
        /// Stop the Agent Application
        /// </summary>
        void StopAgent();
    }
}
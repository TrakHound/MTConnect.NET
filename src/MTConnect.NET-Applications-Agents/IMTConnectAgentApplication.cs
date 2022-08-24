// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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


        IMTConnectAgent Agent { get; }

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
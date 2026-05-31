// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using System;

namespace MTConnect.Applications
{
    /// <summary>
    /// An interface for an MTConnect Agent Application
    /// </summary>
    public interface IMTConnectAgentApplication
    {
        /// <summary>
        /// Windows-service identifier the host registers under (e.g.
        /// <c>MTConnect-Agent</c>). Ignored on non-Windows platforms.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Human-readable Windows-service display name, shown in the
        /// Services management console.
        /// </summary>
        string ServiceDisplayName { get; }

        /// <summary>
        /// Long-form description recorded against the Windows service
        /// in the registry.
        /// </summary>
        string ServiceDescription { get; }

        /// <summary>
        /// The underlying agent broker the application hosts. Available
        /// from the moment <c>StartAgent</c> returns.
        /// </summary>
        IMTConnectAgentBroker Agent { get; }


        /// <summary>
        /// Raised when the configuration-file watcher detects a change
        /// and the agent is restarting with the freshly-loaded
        /// <see cref="AgentConfiguration"/>.
        /// </summary>
        event EventHandler<AgentConfiguration> OnRestart;


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
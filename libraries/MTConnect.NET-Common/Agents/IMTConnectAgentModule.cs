// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;

namespace MTConnect.Agents
{
    /// <summary>
    /// A pluggable extension that is hosted by an MTConnect Agent and participates in the Agent startup and shutdown lifecycle.
    /// </summary>
    public interface IMTConnectAgentModule
    {
        /// <summary>
        /// A unique identifier that distinguishes this module from other modules loaded by the Agent.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A human-readable description of the purpose this module serves within the Agent.
        /// </summary>
        string Description { get; }


        /// <summary>
        /// Raised when the module emits a log entry, allowing the host Agent to surface module diagnostics.
        /// </summary>
        event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Invoked during Agent startup before Devices are loaded into the Agent.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        void StartBeforeLoad(bool initializeDataItems);

        /// <summary>
        /// Invoked during Agent startup after Devices have been loaded into the Agent.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        void StartAfterLoad(bool initializeDataItems);

        /// <summary>
        /// Invoked during Agent shutdown so the module can release resources and stop any background activity.
        /// </summary>
        void Stop();
    }
}

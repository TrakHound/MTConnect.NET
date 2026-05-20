// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Input;
using MTConnect.Logging;

namespace MTConnect.Agents
{
    /// <summary>
    /// A pluggable extension that intercepts observations and Assets as they flow into an MTConnect Agent, allowing each item to be transformed, enriched, or filtered before it is stored.
    /// </summary>
    public interface IMTConnectAgentProcessor
    {
        /// <summary>
        /// A unique identifier that distinguishes this processor from other processors loaded by the Agent.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A human-readable description of the transformation this processor applies.
        /// </summary>
        string Description { get; }


        /// <summary>
        /// Raised when the processor emits a log entry, allowing the host Agent to surface processor diagnostics.
        /// </summary>
        event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Invoked once during Agent startup so the processor can perform any initialization required before it begins processing data.
        /// </summary>
        void Load();


        /// <summary>
        /// Process an incoming observation before it is added to the Agent.
        /// </summary>
        /// <param name="observation">The observation, together with its resolved Device and DataItem context, to process.</param>
        /// <returns>The observation input to store, or <c>null</c> to suppress the observation.</returns>
        IObservationInput Process(ProcessObservation observation);

        /// <summary>
        /// Process an incoming Asset before it is added to the Agent.
        /// </summary>
        /// <param name="asset">The Asset to process.</param>
        /// <returns>The Asset to store, or <c>null</c> to suppress the Asset.</returns>
        IAsset Process(IAsset asset);
    }
}

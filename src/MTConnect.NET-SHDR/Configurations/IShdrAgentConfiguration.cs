// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR Agent
    /// </summary>
    public interface IShdrAgentConfiguration : IHttpAgentConfiguration
    {
        /// <summary>
        /// Do not overwrite the UUID with the UUID from the adapter, preserve the UUID for the Device. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        bool PreserveUuid { get; }

        /// <summary>
        /// Suppress the Adapter IP Address and port when creating the Agent Device ids and names for 1.7. This applies to all adapters.
        /// </summary>
        bool SuppressIpAddress { get; }

        /// <summary>
        /// The amount of time (in milliseconds) an adapter can be silent before it is disconnected. 
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// The amount of time (in milliseconds) between adapter reconnection attempts. 
        /// </summary>
        int ReconnectInterval { get; }

        /// <summary>
        /// Gets or Sets whether a Device Model can be sent from an SHDR Adapter
        /// </summary>
        bool AllowShdrDevice { get; }

        /// <summary>
        /// List of SHDR Adapter connection configurations
        /// </summary>
        IEnumerable<ShdrAdapterConfiguration> Adapters { get; }
    }
}

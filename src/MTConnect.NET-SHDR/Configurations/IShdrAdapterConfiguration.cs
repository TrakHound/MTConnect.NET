// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR Adapter
    /// </summary>
    public interface IShdrAdapterConfiguration : IShdrClientConfiguration
    {
        /// <summary>
        /// For devices that do not have the ability to provide available events, if yes, this sets the Availability to AVAILABLE upon connection.
        /// </summary>
        bool AvailableOnConnection { get; }

        /// <summary>
        /// Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        bool IgnoreTimestamps { get; }

        /// <summary>
        /// Adapter setting for data item units conversion in the agent. Assumes the adapter has already done unit conversion. Defaults to global.
        /// </summary>
        bool ConvertUnits { get; }

        /// <summary>
        /// Gets or Sets the default for Ignoring the case of Observation values
        /// </summary>
        bool IgnoreObservationCase { get; }

        /// <summary>
        /// Gets or Sets whether the Connection Information (Host / Port) is output to the Agent to be collected by a client
        /// </summary>
        bool OutputConnectionInformation { get; }
    }
}

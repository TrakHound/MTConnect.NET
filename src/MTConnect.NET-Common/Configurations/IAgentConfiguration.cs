// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using System;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Agent
    /// </summary>
    public interface IAgentConfiguration
    {
        string ChangeToken { get; }

        string Path { get; }


        /// <summary>
        /// The maximum number of Observations the agent can hold in its buffer
        /// </summary>
        int ObservationBufferSize { get; }

        /// <summary>
        /// The maximum number of assets the agent can hold in its buffer
        /// </summary>
        int AssetBufferSize { get; }


        /// <summary>
        /// Overwrite timestamps with the agent time. 
        /// This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        bool IgnoreTimestamps { get; }

        /// <summary>
        /// Gets or Sets the default MTConnect version to output response documents for.
        /// </summary>
        Version DefaultVersion { get; }

        /// <summary>
        /// Gets or Sets the default for Converting Units when adding Observations
        /// </summary>
        bool ConvertUnits { get; }

        /// <summary>
        /// Gets or Sets the default for Ignoring the case of Observation values
        /// </summary>
        bool IgnoreObservationCase { get; }

        /// <summary>
        /// Gets or Sets the default Input (Observation or Asset) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        InputValidationLevel InputValidationLevel { get; }


        void Save(string path = null, bool createBackup = true);
    }
}

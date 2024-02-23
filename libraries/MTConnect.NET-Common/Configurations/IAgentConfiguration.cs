// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System;
using System.Text.Json.Serialization;

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
        /// Gets the default MTConnect version to output response documents for.
        /// </summary>
        Version DefaultVersion { get; }

        /// <summary>
        /// Gets the default for Converting Units when adding Observations
        /// </summary>
        bool ConvertUnits { get; }

        /// <summary>
        /// Gets the default for Ignoring the case of Observation values
        /// </summary>
        bool IgnoreObservationCase { get; }

        /// <summary>
        /// Gets the default Input (Observation or Asset) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        InputValidationLevel InputValidationLevel { get; }


        /// <summary>
        /// Gets or Sets whether the Agent Device is output
        /// </summary>
        bool EnableAgentDevice { get; }

        /// <summary>
        /// Gets whether Metrics are captured (ex. ObserationUpdateRate, AssetUpdateRate)
        /// </summary>
        bool EnableMetrics { get; }


        void SaveJson(string path = null, bool createBackup = true);

        void SaveYaml(string path = null, bool createBackup = true);
    }
}
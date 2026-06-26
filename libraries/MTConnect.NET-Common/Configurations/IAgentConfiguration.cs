// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Agent
    /// </summary>
    public interface IAgentConfiguration
    {
        /// <summary>
        /// An opaque token that changes whenever the underlying configuration source is reloaded, allowing consumers to detect that the configuration has been replaced.
        /// </summary>
        string ChangeToken { get; }

        /// <summary>
        /// The file system path the configuration was loaded from, used as the default target when the configuration is saved back to disk.
        /// </summary>
        string Path { get; }


        /// <summary>
        /// The maximum number of Observations the agent can hold in its buffer
        /// </summary>
        uint ObservationBufferSize { get; }

        /// <summary>
        /// The maximum number of assets the agent can hold in its buffer
        /// </summary>
        uint AssetBufferSize { get; }


        /// <summary>
        /// Sets the TimeZone to use when timestamps are output from the Agent
        /// </summary>
        string TimeZoneOutput { get; }

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
        /// Gets or Sets whether validation information is output
        /// </summary>
        bool EnableValidation { get; }

        /// <summary>
        /// Gets or Sets the default Device (MTConnectDevices) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        DeviceValidationLevel DeviceValidationLevel { get; }

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


        /// <summary>
        /// Serializes this configuration to JSON and writes it to disk.
        /// </summary>
        /// <param name="path">The destination path; when null the path the configuration was loaded from is used.</param>
        /// <param name="createBackup">When true, an existing file at the destination is preserved as a backup before being overwritten.</param>
        void SaveJson(string path = null, bool createBackup = true);

        /// <summary>
        /// Serializes this configuration to YAML and writes it to disk.
        /// </summary>
        /// <param name="path">The destination path; when null the path the configuration was loaded from is used.</param>
        /// <param name="createBackup">When true, an existing file at the destination is preserved as a backup before being overwritten.</param>
        void SaveYaml(string path = null, bool createBackup = true);
    }
}
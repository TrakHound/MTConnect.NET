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
        /// The value emitted as the <c>Header/@sender</c> attribute on MTConnect response documents (see MTConnect Part 1 §7). When null or empty, <see cref="MTConnect.Agents.MTConnectAgent"/> falls back to <see cref="System.Net.Dns.GetHostName"/>.
        /// </summary>
        string Sender { get; }


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
        /// Gets the default Device (MTConnectDevices) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        DeviceValidationLevel DeviceValidationLevel { get; }

        /// <summary>
        /// Gets the default Input (Observation or Asset) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        InputValidationLevel InputValidationLevel { get; }

        /// <summary>
        /// Gets whether an empty, null, or whitespace-only Result is preserved for Event DataItems whose
        /// Type has a controlled vocabulary (for example EXECUTION, CONTROLLER_MODE). When <c>false</c>
        /// (the default), such Results are coerced to <c>UNAVAILABLE</c> to satisfy the MTConnect
        /// Standard requirement that a controlled-vocabulary Event's Result be a member of the
        /// vocabulary. When <c>true</c>, the empty Result is published verbatim, which some
        /// implementations rely on for parity with adapters that emit empty values.
        /// Numeric DataItems (all Samples, and the numeric-typed Events enumerated by the SysML model)
        /// are always coerced regardless of this flag, per the Part 2 "Sample MUST always be reported
        /// in float" requirement. Free-form String Event DataItems (PROGRAM, MESSAGE, TOOL_ID,
        /// ASSET_CHANGED, and every other non-vocabulary Type) always preserve the empty Result.
        /// </summary>
        bool AllowEmptyResultForEnumEvents { get; }


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
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Shdr > Http Agent
    /// </summary>
    public interface IAgentApplicationConfiguration : IAgentConfiguration
    {
        /// <summary>
        /// Optional static UUID to assign to the Agent meta-device. When set,
        /// this value overrides the per-boot <c>Guid.NewGuid()</c> default and
        /// survives restarts without relying on <c>agent.information.json</c>
        /// being present on disk. Corresponds to <c>AgentDeviceUUID</c> in the
        /// cppagent reference implementation.
        /// </summary>
        string AgentUuid { get; set; }


        /// <summary>
        /// The Path to look for the file(s) that represent the Device Information Models to load into the Agent.
        /// The path can either be a single file or a directory. The path can be absolute or relative to the executable's directory
        /// </summary>
        string Devices { get; set; }


        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Changes the display name of the service. This helps with identification when multiple agents on run as services on the same machine.
        /// </summary>
        string ServiceDisplayName { get; set; }

        /// <summary>
        /// Changes the description of the service. This helps with identification when multiple agents on run as services on the same machine.
        /// </summary>
        string ServiceDescription { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether the Agent buffers are durable and retain state after restart
        /// </summary>
        bool Durable { get; set; }

        /// <summary>
        /// The base path to the directory to write the File Buffers when 'durable = true'
        /// </summary>
        string DurableBufferPath { get; set; }

        /// <summary>
        /// Gets or Sets whether the durable Agent buffers use Compression
        /// </summary>
        bool UseBufferCompression { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart
        /// </summary>
        bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        int ConfigurationFileRestartInterval { get; set; }


        /// <summary>
        /// The raw, untyped module configuration sections declared for the agent, each later resolved to a strongly typed configuration on demand.
        /// </summary>
        IEnumerable<object> Modules { get; set; }

        /// <summary>
        /// The raw, untyped processor configuration sections declared for the agent's observation/asset processing pipeline.
        /// </summary>
        IEnumerable<object> Processors { get; set; }


        /// <summary>
        /// Returns every configured module section keyed by its declared module identifier.
        /// </summary>
        Dictionary<object, object> GetModules();

        /// <summary>
        /// Returns the configured module sections registered under the given module key as untyped objects.
        /// </summary>
        /// <param name="key">The module identifier whose sections are requested.</param>
        IEnumerable<object> GetModules(string key);

        /// <summary>
        /// Returns the number of module sections configured under the given module key.
        /// </summary>
        /// <param name="key">The module identifier to count sections for.</param>
        int GetModuleCount(string key);

        /// <summary>
        /// Returns the configured module sections registered under the given module key, each deserialized to <typeparamref name="TConfiguration"/>.
        /// </summary>
        /// <typeparam name="TConfiguration">The strongly typed configuration the module sections are bound to.</typeparam>
        /// <param name="key">The module identifier whose sections are requested.</param>
        IEnumerable<TConfiguration> GetModules<TConfiguration>(string key);

        /// <summary>
        /// Indicates whether at least one module section is configured under the given module key.
        /// </summary>
        /// <param name="key">The module identifier to test.</param>
        bool IsModuleConfigured(string key);


        /// <summary>
        /// Returns every configured processor section keyed by its declared processor identifier.
        /// </summary>
        Dictionary<object, object> GetProcessors();

        /// <summary>
        /// Returns the configured processor sections registered under the given processor key as untyped objects.
        /// </summary>
        /// <param name="key">The processor identifier whose sections are requested.</param>
        IEnumerable<object> GetProcessors(string key);

        /// <summary>
        /// Returns the configured processor sections registered under the given processor key, each deserialized to <typeparamref name="TConfiguration"/>.
        /// </summary>
        /// <typeparam name="TConfiguration">The strongly typed configuration the processor sections are bound to.</typeparam>
        /// <param name="key">The processor identifier whose sections are requested.</param>
        IEnumerable<TConfiguration> GetProcessors<TConfiguration>(string key);
    }
}
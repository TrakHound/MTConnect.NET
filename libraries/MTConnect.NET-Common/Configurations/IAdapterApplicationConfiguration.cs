// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Adapter
    /// </summary>
    public interface IAdapterApplicationConfiguration : IDataSourceConfiguration
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
        /// Get a unique identifier for the Adapter
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The Name or UUID of the Device to create a connection for
        /// </summary>
        string DeviceKey { get; set; }


        /// <summary>
        /// The interval (in milliseconds) at which new data is sent to the Agent
        /// </summary>
        int WriteInterval { get; set; }


        /// <summary>
        /// Determines whether to filter out duplicate data
        /// </summary>
        bool FilterDuplicates { get; set; }

        /// <summary>
        /// Determines whether to output Timestamps for each SHDR line
        /// </summary>
        bool OutputTimestamps { get; set; }

        /// <summary>
        /// Determines whether to send all changes to data or only most recent at the specified Interval
        /// </summary>
        bool EnableBuffer { get; set; }


        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple Adapters to run as services on the same machine.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Adapter will restart
        /// </summary>
        bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Adapter restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        int ConfigurationFileRestartInterval { get; set; }


        /// <summary>
        /// Free-form key/value settings consumed by the adapter engine; keys correspond to engine property names with loosely typed values.
        /// </summary>
        Dictionary<string, object> Engine { get; set; }

        /// <summary>
        /// The raw, untyped module configuration sections declared for the adapter, each later resolved to a strongly typed configuration on demand.
        /// </summary>
        IEnumerable<object> Modules { get; set; }


        /// <summary>
        /// Returns the value of the named engine property, or null when the property is not present.
        /// </summary>
        /// <param name="propertyName">The engine property key to look up.</param>
        object GetEngineProperty(string propertyName);


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
        /// Returns the configured module sections registered under the given module key, each deserialized to <typeparamref name="TConfiguration"/>.
        /// </summary>
        /// <typeparam name="TConfiguration">The strongly typed configuration the module sections are bound to.</typeparam>
        /// <param name="key">The module identifier whose sections are requested.</param>
        IEnumerable<TConfiguration> GetModules<TConfiguration>(string key);
    }
}
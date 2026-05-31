// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Agent Configuration File Watcher that notifies when the specified <typeparamref name="TConfiguration"/> is updated
    /// </summary>
    /// <typeparam name="TConfiguration">The type of MTConnectAgentConfiguration file to read</typeparam>
    public class AdapterConfigurationFileWatcher<TConfiguration> : ConfigurationFileWatcher<AdapterApplicationConfiguration>, IAdapterConfigurationFileWatcher where TConfiguration : AdapterApplicationConfiguration
    {
        /// <summary>
        /// Initialises a new watcher against <paramref name="path"/>,
        /// polling for changes at <paramref name="interval"/>
        /// milliseconds.
        /// </summary>
        /// <param name="path">Filesystem path to watch.</param>
        /// <param name="interval">Poll interval in milliseconds.</param>
        public AdapterConfigurationFileWatcher(string path, int interval = DefaultInterval) :base(path, interval) { }


        /// <summary>
        /// Reads the file at <paramref name="path"/> as YAML or JSON
        /// (selected by file extension) and binds it to
        /// <typeparamref name="TConfiguration"/>. Returns <c>default</c>
        /// when the read fails or the extension is unrecognised.
        /// </summary>
        /// <param name="path">Filesystem path of the configuration
        /// file.</param>
        /// <returns>The parsed configuration, or <c>default</c>.</returns>
        protected override AdapterApplicationConfiguration OnRead(string path)
        {
            AdapterApplicationConfiguration configuration = default;

            switch (Path.GetExtension(path))
            {
                case ".yaml": configuration = AdapterApplicationConfiguration.ReadYaml<TConfiguration>(path); break;
                case ".yml": configuration = AdapterApplicationConfiguration.ReadYaml<TConfiguration>(path); break;
                case ".json": configuration = AdapterApplicationConfiguration.ReadJson<TConfiguration>(path); break;
            }

            if (configuration != null)
            {
                configuration.Path = path;
                return configuration;
            }

            return default;
        }
    }
}
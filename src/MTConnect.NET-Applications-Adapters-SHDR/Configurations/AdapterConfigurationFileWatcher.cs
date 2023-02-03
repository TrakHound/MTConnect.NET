// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Agent Configuration File Watcher that notifies when the specified <typeparamref name="TConfiguration"/> is updated
    /// </summary>
    /// <typeparam name="TConfiguration">The type of MTConnectAgentConfiguration file to read</typeparam>
    public class AdapterConfigurationFileWatcher<TConfiguration> : ConfigurationFileWatcher<ShdrAdapterApplicationConfiguration>, IAdapterConfigurationFileWatcher where TConfiguration : ShdrAdapterApplicationConfiguration
    {
        public AdapterConfigurationFileWatcher(string path, int interval = DefaultInterval) :base(path, interval) { }


        protected override ShdrAdapterApplicationConfiguration OnRead(string path)
        {
            ShdrAdapterApplicationConfiguration configuration = default;

            switch (Path.GetExtension(path))
            {
                case ".yaml": configuration = ShdrAdapterApplicationConfiguration.ReadYaml<TConfiguration>(path); break;
                case ".yml": configuration = ShdrAdapterApplicationConfiguration.ReadYaml<TConfiguration>(path); break;
                case ".json": configuration = ShdrAdapterApplicationConfiguration.ReadJson<TConfiguration>(path); break;
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
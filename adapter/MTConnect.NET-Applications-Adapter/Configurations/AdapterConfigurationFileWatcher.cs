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
        public AdapterConfigurationFileWatcher(string path, int interval = DefaultInterval) :base(path, interval) { }


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
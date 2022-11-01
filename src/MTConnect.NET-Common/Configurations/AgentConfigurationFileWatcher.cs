// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Agent Configuration File Watcher that notifies when the specified <typeparamref name="TConfiguration"/> is updated
    /// </summary>
    /// <typeparam name="TConfiguration">The type of MTConnectAgentConfiguration file to read</typeparam>
    public class AgentConfigurationFileWatcher<TConfiguration> : IAgentConfigurationFileWatcher where TConfiguration : AgentConfiguration
    {
        private const int DefaultInterval = 2000;

        private readonly string _path;
        private readonly int _interval;
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _timer;
        private bool _update = false;


        public EventHandler<AgentConfiguration> ConfigurationUpdated { get; set; }

        public EventHandler<string> ErrorReceived { get; set; }


        public AgentConfigurationFileWatcher(string path, int interval = DefaultInterval)
        {
            _path = path;
            _interval = interval;

            Start();
        }

        private void Start()
        {
            try
            {
                _watcher = new FileSystemWatcher();
                _watcher.Path = Path.GetDirectoryName(_path);
                _watcher.Filter = Path.GetFileName(_path);
                _watcher.Changed += ChangeReceived;
                _watcher.EnableRaisingEvents = true;

                _timer = new System.Timers.Timer();
                _timer.Interval = _interval;
                _timer.Elapsed += UpdateTimerElapsed;
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null) ErrorReceived.Invoke(this, ex.Message);
            }
        }

        private void Update()
        {
            if (_update)
            {
                var configuration = ReadFile();
                if (configuration != null)
                {
                    if (ConfigurationUpdated != null) ConfigurationUpdated.Invoke(this, configuration);
                }

                _update = false;
            }
        }

        private void UpdateTimerElapsed(object sender, EventArgs args)
        {
            Update();
        }


        private void ChangeReceived(object sender, FileSystemEventArgs args)
        {
            if (args.ChangeType == WatcherChangeTypes.Changed)
            {
                _update = true;
            }
        }

        private TConfiguration ReadFile()
        {
            try
            {
                TConfiguration configuration = null;

                switch (Path.GetExtension(_path))
                {
                    case ".yaml": configuration = AgentConfiguration.ReadYaml<TConfiguration>(_path); break;
                    case ".json": configuration = AgentConfiguration.ReadJson<TConfiguration>(_path); break;
                }

                if (configuration != null)
                {
                    configuration.Path = _path;
                    return configuration;
                }
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null) ErrorReceived.Invoke(this, ex.Message);
            }

            return null;
        }


        public void Dispose()
        {
            if (_watcher != null) _watcher.Dispose();
            if (_timer != null) _timer.Dispose();
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration File Watcher that notifies when the specified <typeparamref name="T"/> is updated
    /// </summary>
    /// <typeparam name="TConfiguration">The type of Configuration file to read</typeparam>
    public class ConfigurationFileWatcher<T> : IConfigurationFileWatcher<T>
    {
        protected const int DefaultInterval = 2000;

        private readonly string _path;
        private readonly int _interval;
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _timer;
        private bool _update = false;


        public event EventHandler<T> ConfigurationUpdated;

        public event EventHandler<string> ErrorReceived;


        public ConfigurationFileWatcher(string path, int interval = DefaultInterval)
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

        protected virtual T OnRead(string path)
        {
            return default;
        }

        private T ReadFile()
        {
            try
            {
                return OnRead(_path);
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null) ErrorReceived.Invoke(this, ex.Message);
            }

            return default;
        }


        public void Dispose()
        {
            if (_watcher != null) _watcher.Dispose();
            if (_timer != null) _timer.Dispose();
        }
    }
}
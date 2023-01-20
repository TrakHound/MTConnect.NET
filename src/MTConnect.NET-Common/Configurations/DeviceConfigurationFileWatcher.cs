// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;
using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Device Configuration File Watcher that notifies when the specified Device Configuration path is updated
    /// </summary>
    public class DeviceConfigurationFileWatcher : IDisposable
    {
        private const int DefaultInterval = 2000;

        private readonly string _path;
        private readonly int _interval;
        private readonly string _documentFormatter;
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _timer;
        private bool _update = false;


        public EventHandler<DeviceConfiguration> ConfigurationUpdated { get; set; }

        public EventHandler<string> ErrorReceived { get; set; }


        public DeviceConfigurationFileWatcher(string path, int interval = DefaultInterval, string documentFormatter = DocumentFormat.XML)
        {
            _path = path;
            _interval = interval;
            _documentFormatter = documentFormatter;

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
                var devices = DeviceConfiguration.FromFile(_path, _documentFormatter);
                if (!devices.IsNullOrEmpty())
                {
                    foreach (var device in devices)
                    {
                        if (ConfigurationUpdated != null) ConfigurationUpdated.Invoke(this, device);
                    }
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


        public void Dispose()
        {
            if (_watcher != null) _watcher.Dispose();
            if (_timer != null) _timer.Dispose();
        }
    }
}
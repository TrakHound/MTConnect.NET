// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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


        /// <summary>
        /// Raised once per device whenever the watched file changes and is successfully re-parsed into one or more device configurations.
        /// </summary>
        public event EventHandler<DeviceConfiguration> ConfigurationUpdated;

        /// <summary>
        /// Raised when a change is detected but the file cannot be read or parsed; the event argument carries the error message.
        /// </summary>
        public event EventHandler<string> ErrorReceived;


        /// <summary>
        /// Initializes the watcher for the given device configuration file and begins monitoring it for changes.
        /// </summary>
        /// <param name="path">The full path of the device configuration file to watch.</param>
        /// <param name="interval">The debounce interval, in milliseconds, between detecting a change and re-reading the file.</param>
        /// <param name="documentFormatter">The document format used to parse the file, defaulting to XML.</param>
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


        /// <summary>
        /// Stops monitoring and releases the underlying file system watcher and polling timer.
        /// </summary>
        public void Dispose()
        {
            if (_watcher != null) _watcher.Dispose();
            if (_timer != null) _timer.Dispose();
        }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Processors
{
    /// <summary>
    /// Agent processor that pipes each inbound observation through every
    /// Python script in a configured directory. Each <c>*.py</c> file must
    /// declare a top-level <c>process(observation)</c> function that
    /// returns a (possibly modified) <c>ProcessObservation</c>. Scripts
    /// are evaluated by the IronPython engine; the directory is watched
    /// with a <see cref="FileSystemWatcher"/> so add / change / remove
    /// events trigger a debounced reload.
    /// </summary>
    public class Processor : MTConnectAgentProcessor
    {
        /// <summary>
        /// Token used in <c>agent.config.yaml</c> to bind this processor
        /// (<c>type: python</c>). The agent host matches this value
        /// against the configuration discriminator at startup.
        /// </summary>
        public const string ConfigurationTypeId = "python";
        private const string ProcessorId = "Python Scripts";
        private const int DefaultUpdateInterval = 2000;

        private const string _functionName = "process";
        private const string _defaultDirectory = "processors";
        private const string _defaultExtension = ".py";

        private readonly Microsoft.Scripting.Hosting.ScriptEngine _pythonEngine;
        private readonly Dictionary<string, Func<ProcessObservation, ProcessObservation>> _functions = new Dictionary<string, Func<ProcessObservation, ProcessObservation>>();

        // Immutable snapshot of _functions read by OnProcess without locking (replaced whenever _functions changes)
        private volatile KeyValuePair<string, Func<ProcessObservation, ProcessObservation>>[] _functionSnapshot = new KeyValuePair<string, Func<ProcessObservation, ProcessObservation>>[0];
        private readonly ProcessorConfiguration _configuration;
        private readonly object _lock = new object();

        private FileSystemWatcher _watcher;
        private System.Timers.Timer _watcherTimer;
        private bool _update = false;


        /// <summary>
        /// Initialises the processor, binds the supplied configuration
        /// payload to <see cref="ProcessorConfiguration"/>, creates the
        /// IronPython engine, and starts the script-directory watcher.
        /// </summary>
        /// <param name="configuration">Raw configuration object the
        /// agent host passes through; bound to
        /// <see cref="ProcessorConfiguration"/> via the standard agent
        /// configuration helper.</param>
        public Processor(object configuration)
        {
            Id = ProcessorId;

            _pythonEngine = IronPython.Hosting.Python.CreateEngine();
            _configuration = AgentApplicationConfiguration.GetConfiguration<ProcessorConfiguration>(configuration);
            if (_configuration == null) _configuration = new ProcessorConfiguration();

            StartWatcher();
        }


        /// <summary>
        /// Rescans the configured script directory and (re)loads every
        /// <c>*.py</c> file. Existing functions are cleared first so a
        /// removed script no longer participates in
        /// <see cref="OnProcess"/>. Safe to call from the file-system
        /// watcher's debounced timer.
        /// </summary>
        public override void Load()
        {
            // Compile into a separate set so the previous functions keep processing until the swap
            var functions = new Dictionary<string, Func<ProcessObservation, ProcessObservation>>();

            var dir = GetDirectory();
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, $"*{_defaultExtension}");
                if (!files.IsNullOrEmpty())
                {
                    foreach (var file in files)
                    {
                        var process = LoadEngine(file);
                        if (process != null) functions[file] = process;
                    }
                }
            }

            lock (_lock)
            {
                _functions.Clear();
                foreach (var function in functions) _functions.Add(function.Key, function.Value);
                UpdateFunctionSnapshot();
            }
        }

        private Func<ProcessObservation, ProcessObservation> LoadEngine(string file)
        {
            try
            {
                var src = File.ReadAllText(file);

                var scope = _pythonEngine.CreateScope();
                _pythonEngine.Execute(src, scope);

                var process = scope.GetVariable<Func<ProcessObservation, ProcessObservation>>(_functionName);
                if (process != null)
                {
                    Log(Logging.MTConnectLogLevel.Debug, $"Python Script Loaded : {file}");

                    return process;
                }
            }
            catch (Exception ex)
            {
                Log(Logging.MTConnectLogLevel.Error, $"Error Loading Python Script : {file} : {ex.Message}");
            }

            return null;
        }


        /// <summary>
        /// Pipes the supplied observation through every loaded Python
        /// <c>process</c> function in registration order. Each function
        /// receives the *current* observation (with prior functions'
        /// changes applied) and returns either a transformed observation
        /// or <c>null</c> to drop it. Exceptions inside a function are
        /// logged and the original observation continues down the chain.
        /// </summary>
        /// <param name="observation">Observation produced by the agent
        /// pipeline immediately upstream of this processor.</param>
        /// <returns>The observation flattened into an
        /// <see cref="IObservationInput"/> ready to enqueue, or
        /// <c>null</c> when every function dropped the observation.</returns>
        protected override IObservationInput OnProcess(ProcessObservation observation)
        {
            ProcessObservation outputObservation = observation;

            try
            {
                var functions = _functionSnapshot;
                foreach (var function in functions)
                {
                    if (outputObservation == null) break;

                    try
                    {
                        outputObservation = function.Value(outputObservation);
                    }
                    catch (Exception ex)
                    {
                        Log(Logging.MTConnectLogLevel.Error, $"Process Error : {function.Key} : {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(Logging.MTConnectLogLevel.Error, $"Error During Process : {ex.Message}");
            }

            if (outputObservation != null)
            {
                var resultObservation = new ObservationInput();
                var dataItem = outputObservation.DataItem ?? observation.DataItem;
                resultObservation.DeviceKey = dataItem?.Device?.Uuid;
                resultObservation.DataItemKey = dataItem?.Id;
                resultObservation.Values = outputObservation.Values;
                resultObservation.Timestamp = outputObservation.Timestamp.ToUnixTime();
                return resultObservation;
            }
            else
            {
                return null;
            }
        }


        private void AddFunction(string filePath, Func<ProcessObservation, ProcessObservation> function)
        {
            if (filePath != null && function != null)
            {
                lock (_lock)
                {
                    _functions.Remove(filePath);
                    _functions.Add(filePath, function);
                    UpdateFunctionSnapshot();
                }
            }
        }

        private void RemoveFunction(string key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    _functions.Remove(key);
                    UpdateFunctionSnapshot();
                }
            }
        }

        // Must be called while holding _lock
        private void UpdateFunctionSnapshot()
        {
            var snapshot = new KeyValuePair<string, Func<ProcessObservation, ProcessObservation>>[_functions.Count];
            var i = 0;
            foreach (var function in _functions) snapshot[i++] = function;
            _functionSnapshot = snapshot;
        }


        private void StartWatcher()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = GetDirectory();
            _watcher.Filter = $"*{_defaultExtension}";
            _watcher.Created += FileWatcherUpdated;
            _watcher.Changed += FileWatcherUpdated;
            _watcher.Deleted += FileWatcherUpdated;
            _watcher.EnableRaisingEvents = true;

            _watcherTimer = new System.Timers.Timer();
            _watcherTimer.Interval = DefaultUpdateInterval;
            _watcherTimer.Elapsed += WatcherTimerElapsed;
            _watcherTimer.Enabled = true;
        }

        private void StopWatcher()
        {
            if (_watcher != null) _watcher.Dispose();
            if (_watcherTimer != null) _watcherTimer.Dispose();
        }

        private void FileWatcherUpdated(object sender, FileSystemEventArgs e)
        {
            _update = true;
        }

        private void WatcherTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            if (_update)
            {
                Load();

                _update = false;
            }
        }


        private string GetDirectory()
        {
            var dir = _configuration.Directory;
            if (string.IsNullOrEmpty(dir)) dir = _defaultDirectory;
            if (!Path.IsPathRooted(dir)) dir = Path.Combine(AppContext.BaseDirectory, dir);
            return dir;
        }
    }
}
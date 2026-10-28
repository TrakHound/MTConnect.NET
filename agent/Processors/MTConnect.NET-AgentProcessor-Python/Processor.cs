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
        private readonly Dictionary<string, ScriptFunction> _functions = new Dictionary<string, ScriptFunction>();

        // Immutable snapshot of _functions read by OnProcess without locking (replaced whenever _functions changes)
        private volatile ScriptFunction[] _functionSnapshot = new ScriptFunction[0];

        // A function that fails this many times in a row is disabled until its script is reloaded
        private const int MaxConsecutiveFailures = 10;

        private sealed class ScriptFunction
        {
            public string Key;
            public Func<ProcessObservation, ProcessObservation> Process;

            // Optional filters read from the script (data_item_types / data_item_ids); null means every DataItem
            public HashSet<string> DataItemTypes;
            public HashSet<string> DataItemIds;

            public int ConsecutiveFailures;
            public volatile bool Disabled;

            public bool Applies(ProcessObservation observation)
            {
                if (Disabled) return false;

                var dataItem = observation.DataItem;
                if (DataItemTypes != null && (dataItem?.Type == null || !DataItemTypes.Contains(dataItem.Type))) return false;
                if (DataItemIds != null && (dataItem?.Id == null || !DataItemIds.Contains(dataItem.Id))) return false;
                return true;
            }
        }
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
            var functions = new Dictionary<string, ScriptFunction>();

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

        private ScriptFunction LoadEngine(string file)
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

                    return new ScriptFunction
                    {
                        Key = file,
                        Process = process,
                        DataItemTypes = ReadStringSet(scope, "data_item_types"),
                        DataItemIds = ReadStringSet(scope, "data_item_ids")
                    };
                }
            }
            catch (Exception ex)
            {
                Log(Logging.MTConnectLogLevel.Error, $"Error Loading Python Script : {file} : {ex.Message}");
            }

            return null;
        }


        // Reads an optional list of strings defined by the script (e.g. data_item_types = ["EXECUTION"])
        private static HashSet<string> ReadStringSet(Microsoft.Scripting.Hosting.ScriptScope scope, string name)
        {
            if (scope.TryGetVariable(name, out object value) && value is System.Collections.IEnumerable items && !(value is string))
            {
                var set = new HashSet<string>(StringComparer.Ordinal);
                foreach (var item in items)
                {
                    if (item != null) set.Add(item.ToString());
                }
                return set;
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

                    // Skip scripts that don't apply to this DataItem (avoids a call into the Python runtime)
                    if (!function.Applies(outputObservation)) continue;

                    try
                    {
                        outputObservation = function.Process(outputObservation);
                        function.ConsecutiveFailures = 0;
                    }
                    catch (Exception ex)
                    {
                        if (++function.ConsecutiveFailures >= MaxConsecutiveFailures)
                        {
                            function.Disabled = true;
                            Log(Logging.MTConnectLogLevel.Error, $"Process Error : {function.Key} : {ex.Message} : Disabled after {MaxConsecutiveFailures} consecutive errors (it is re-enabled when the script changes)");
                        }
                        else
                        {
                            Log(Logging.MTConnectLogLevel.Error, $"Process Error : {function.Key} : {ex.Message}");
                        }
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


        // Must be called while holding _lock
        private void UpdateFunctionSnapshot()
        {
            var snapshot = new ScriptFunction[_functions.Count];
            var i = 0;
            foreach (var function in _functions.Values) snapshot[i++] = function;
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
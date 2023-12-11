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
    public class Processor : MTConnectAgentProcessor
    {
        public const string ConfigurationTypeId = "python";
        private const string ProcessorId = "Python Scripts";
        private const int DefaultUpdateInterval = 2000;

        private const string _functionName = "process";
        private const string _defaultDirectory = "processors";
        private const string _defaultExtension = ".py";

        private readonly Microsoft.Scripting.Hosting.ScriptEngine _pythonEngine;
        private readonly Dictionary<string, Func<ProcessObservation, ProcessObservation>> _functions = new Dictionary<string, Func<ProcessObservation, ProcessObservation>>();
        private readonly ProcessorConfiguration _configuration;
        private readonly object _lock = new object();

        private FileSystemWatcher _watcher;
        private System.Timers.Timer _watcherTimer;
        private bool _update = false;


        public Processor(object configuration)   
        {
            Id = ProcessorId;

            _pythonEngine = IronPython.Hosting.Python.CreateEngine();
            _configuration = AgentApplicationConfiguration.GetConfiguration<ProcessorConfiguration>(configuration);
            if (_configuration == null) _configuration = new ProcessorConfiguration();

            StartWatcher();
        }


        public override void Load()
        {
            lock (_lock) _functions.Clear();

            var dir = GetDirectory();
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, $"*{_defaultExtension}");
                if (!files.IsNullOrEmpty())
                {
                    foreach (var file in files)
                    {
                        LoadEngine(file);
                    }
                }
            }
        }

        private void LoadEngine(string file)
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

                    AddFunction(file, process);
                }
            }
            catch (Exception ex)
            {
                Log(Logging.MTConnectLogLevel.Error, $"Error Loading Python Script : {file} : {ex.Message}");
            }
        }


        protected override IObservationInput OnProcess(ProcessObservation observation)
        {
            ProcessObservation outputObservation = observation;

            try
            {
                IEnumerable<string> functionKeys;
                lock (_lock) functionKeys = _functions.Keys;

                if (!functionKeys.IsNullOrEmpty())
                {
                    foreach (var functionKey in functionKeys)
                    {
                        Func<ProcessObservation, ProcessObservation> function;
                        lock (_lock) function = GetFunction(functionKey);

                        if (function != null)
                        {
                            try
                            {
                                outputObservation = function(outputObservation);
                            }
                            catch (Exception ex)
                            {
                                Log(Logging.MTConnectLogLevel.Error, $"Process Error : {functionKey} : {ex.Message}");
                            }
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
                resultObservation.DeviceKey = observation.DataItem?.Device?.Uuid;
                resultObservation.DataItemKey = observation.DataItem?.Id;
                resultObservation.Values = observation.Values;
                resultObservation.Timestamp = observation.Timestamp.ToUnixTime();
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
                }
            }
        }

        private Func<ProcessObservation, ProcessObservation> GetFunction(string filePath)
        {
            if (filePath != null)
            {
                lock (_lock)
                {
                    if (_functions.ContainsKey(filePath))
                    {
                        return _functions[filePath];
                    }
                }
            }

            return null;
        }

        private void RemoveFunction(string key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    _functions.Remove(key);
                }
            }
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
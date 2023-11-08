// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Observations.Input;

namespace MTConnect.Processors
{
    public class MTConnectPythonProcessor : IMTConnectAgentProcessor
    {
        public const string ConfigurationTypeId = "python";

        private const string _functionName = "process";
        private const string _defaultDirectory = "processors";
        private const string _defaultExtension = ".py";

        private readonly Microsoft.Scripting.Hosting.ScriptEngine _pythonEngine;
        private readonly Dictionary<string, Func<ProcessObservation, ProcessObservation>> _functions = new Dictionary<string, Func<ProcessObservation, ProcessObservation>>();
        private readonly PythonProcessorConfiguration _configuration;
        private readonly object _lock = new object();


        public MTConnectPythonProcessor(object configuration)       
        {
            _pythonEngine = IronPython.Hosting.Python.CreateEngine();
            _configuration = AgentApplicationConfiguration.GetConfiguration<PythonProcessorConfiguration>(configuration);
            if (_configuration == null) _configuration = new PythonProcessorConfiguration();

            Load();
        }

        private void Load()
        {
            lock (_lock) _functions.Clear();

            var dir = _configuration.Directory;
            if (string.IsNullOrEmpty(dir)) dir = _defaultDirectory;
            if (!Path.IsPathRooted(dir)) dir = Path.Combine(AppContext.BaseDirectory, dir);

            var files = Directory.GetFiles(dir, $"*{_defaultExtension}");
            if (!files.IsNullOrEmpty())
            {
                foreach (var file in files)
                {
                    LoadEngine(file);
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
                    var key = file.ToMD5Hash();

                    AddFunction(key, process);
                }
            }
            catch (Exception ex)
            {
            
            }
        }


        public IObservationInput Process(ProcessObservation observation)
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
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public IAsset Process(IAsset asset)
        {
            return asset;
        }


        private void AddFunction(string key, Func<ProcessObservation, ProcessObservation> function)
        {
            if (key != null && function != null)
            {
                lock (_lock)
                {
                    _functions.Remove(key);
                    _functions.Add(key, function);
                }
            }
        }

        private Func<ProcessObservation, ProcessObservation> GetFunction(string key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    return _functions.GetValueOrDefault(key);
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
    }
}
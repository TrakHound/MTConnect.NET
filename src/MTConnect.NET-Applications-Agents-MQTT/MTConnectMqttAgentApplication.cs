// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An MTConnect MQTT Agent Application base class supporting Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public abstract class MTConnectMqttAgentApplication : IMTConnectMqttAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent";
        private const string DefaultServiceDisplayName = "MTConnect Agent";
        private const string DefaultServiceDescription = "MTConnect Agent to provide access to device information using the MTConnect Standard";

        protected readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        protected readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        protected readonly Logger _agentMetricLogger = LogManager.GetLogger("agent--metric-logger");
        protected readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");

        private readonly List<DeviceConfigurationFileWatcher> _deviceConfigurationWatchers = new List<DeviceConfigurationFileWatcher>();

        protected LogLevel _logLevel = LogLevel.Debug;
        private MTConnectAgent _mtconnectAgent;
        protected IAgentConfigurationFileWatcher _agentConfigurationWatcher;
        private System.Timers.Timer _metricsTimer;
        private bool _started = false;
        protected bool _verboseLogging = true;


        public string ServiceName { get; set; }

        public string ServiceDisplayName { get; set; }

        public string ServiceDescription { get; set; }

        protected Type ConfigurationType { get; set; }


        public IMTConnectAgent Agent => _mtconnectAgent;

        public EventHandler<AgentConfiguration> OnRestart { get; set; }


        public MTConnectMqttAgentApplication()
        {
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;

            if (ConfigurationType == null) ConfigurationType = typeof(AgentApplicationConfiguration);
        }


        /// <summary>
        /// Run Arguments [help|install|debug|run|reset] [configuration_file]
        /// </summary>
        public void Run(string[] args, bool isBlocking = false)
        {
            string command = "run";
            string configFile = null;


            string serviceName = ServiceName;
            string serviceDisplayName = ServiceDisplayName;
            string serviceDescription = ServiceDescription;
            bool serviceStart = true;

            // Read Command Line Arguments
            if (!args.IsNullOrEmpty())
            {
                command = args[0];

                // Configuration File Path
                if (args.Length > 1)
                {
                    configFile = args[1];
                    _applicationLogger.Info($"Agent Configuration Path = {configFile}");
                }
            }

            OnCommandLineArgumentsRead(args);

            // Convert Json Configuration File to YAML
            string jsonConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.JsonFilename);
            if (File.Exists(jsonConfigPath))
            {
                var dummyConfiguration = OnConfigurationFileRead(jsonConfigPath);
                if (dummyConfiguration != null) dummyConfiguration.SaveYaml();
            }

            // Copy Default Configuration File
            string yamlConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.YamlFilename);
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.DefaultYamlFilename);
            if (!File.Exists(yamlConfigPath) && !File.Exists(jsonConfigPath) && File.Exists(defaultPath))
            {
                File.Copy(defaultPath, yamlConfigPath);
            }

            // Read the Agent Configuation File
            var configuration = OnConfigurationFileRead(configFile);
            if (configuration != null)
            {
                _agentLogger.Info($"Configuration File Read Successfully from: {configuration.Path}");

                // Set Service Name
                if (!string.IsNullOrEmpty(configuration.ServiceName)) serviceDisplayName = configuration.ServiceName;

                // Set Service Auto Start
                serviceStart = configuration.ServiceAutoStart;
            }
            else
            {
                _agentLogger.Warn("Error Reading Configuration File. Default Configuration is loaded.");

                configuration = new AgentApplicationConfiguration();
            }
            

            // Declare a new Service (to use Service commands)
            Service service = null;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                service = new Service(this, serviceName, serviceDisplayName, serviceDescription, serviceStart);
            }

            switch (command)
            {
                case "run":
                    _verboseLogging = false;
                    StartAgent(configuration, _verboseLogging);

                    if (isBlocking) while (true) Thread.Sleep(100); // Block (exit console by 'Ctrl + C')
                    else break;

                case "run-service":

                    _verboseLogging = true;
                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        ServiceBase.Run(service);
                    }
                    else _applicationLogger.Info($"'Run-Service' Command is not supported on this Operating System");

                    break;

                case "debug":
                    _verboseLogging = true;
                    StartAgent(configuration, _verboseLogging);

                    if (isBlocking) while (true) Thread.Sleep(100); // Block (exit console by 'Ctrl + C')
                    else break;

                case "install":

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        service.StopService();
                        service.RemoveService();
                        service.InstallService(configFile);
                    }
                    else _applicationLogger.Info($"'Install' Command is not supported on this Operating System");

                    break;

                case "install-start":

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        service.StopService();
                        service.RemoveService();
                        service.InstallService(configFile);
                        service.StartService();
                    }
                    else _applicationLogger.Info($"'Install-Start' Command is not supported on this Operating System");

                    break;

                case "remove":

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        service.StopService();
                        service.RemoveService();
                    }
                    else _applicationLogger.Info($"'Remove' Command is not supported on this Operating System");

                    break;

                case "start":

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        service.StartService();
                    }
                    else _applicationLogger.Info($"'Start' Command is not supported on this Operating System");

                    break;

                case "stop":

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        service.StopService();
                    }
                    else _applicationLogger.Info($"'Stop' Command is not supported on this Operating System");

                    break;

                case "help":
                    PrintHelp();
                    break;

                default:
                    _applicationLogger.Info($"'{command}' : Command not recognized : See help for more information");
                    PrintHelp();
                    break;
            }
        }

        protected virtual void OnCommandLineArgumentsRead(string[] args) { }

        protected virtual IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            return AgentConfiguration.Read<AgentApplicationConfiguration>(configurationPath);
        }


        public void StartAgent(string configurationPath, bool verboseLogging = false)
        {
            var configuration = OnConfigurationFileRead(configurationPath);

            // Start the Agent with new Configuration
            StartAgent(configuration, verboseLogging);
        }

        public void StartAgent(IAgentApplicationConfiguration configuration, bool verboseLogging = false)
        {
            if (!_started && configuration != null)
            {
                GarbageCollector.Initialize();

                _deviceConfigurationWatchers.Clear();
                var initializeDataItems = true;

                // Read Agent Information File
                var agentInformation = MTConnectAgentInformation.Read();
                if (agentInformation == null)
                {
                    agentInformation = new MTConnectAgentInformation();
                }

                // Save the AgentInformation file
                agentInformation.Save();


                // Create MTConnectAgentBroker
                _mtconnectAgent = new MTConnectAgent(configuration, agentInformation.Uuid, agentInformation.InstanceId, agentInformation.DeviceModelChangeTime, initializeDataItems);

                if (verboseLogging)
                {
                    //_mtconnectAgent.ObservationAdded += ObservationAdded;
                    _mtconnectAgent.InvalidComponentAdded += InvalidComponent;
                    _mtconnectAgent.InvalidCompositionAdded += InvalidComposition;
                    _mtconnectAgent.InvalidDataItemAdded += InvalidDataItem;
                    _mtconnectAgent.InvalidObservationAdded += InvalidObservation;
                    _mtconnectAgent.InvalidAssetAdded += InvalidAsset;
                }

                // Read Device Configuration Files
                var devicesPath = configuration.Devices;
                if (string.IsNullOrEmpty(devicesPath)) devicesPath = "devices";
                var devices = DeviceConfiguration.FromFiles(devicesPath, DocumentFormat.XML);
                if (!devices.IsNullOrEmpty())
                {
                    // Add Device(s) to Agent
                    foreach (var device in devices)
                    {
                        _agentLogger.Info($"Device ({device.Name}) Read From File : {device.Path}");

                        _mtconnectAgent.AddDevice(device, initializeDataItems);
                    }

                    if (configuration.MonitorConfigurationFiles)
                    {
                        // Set Device Configuration File Watcher
                        var paths = devices.Select(o => o.Path).Distinct();
                        foreach (var path in paths)
                        {
                            // Create a Device Configuration File Watcher
                            var deviceConfigurationWatcher = new DeviceConfigurationFileWatcher(path, configuration.ConfigurationFileRestartInterval * 1000);
                            deviceConfigurationWatcher.ConfigurationUpdated += DeviceConfigurationFileUpdated;
                            deviceConfigurationWatcher.ErrorReceived += DeviceConfigurationFileError;
                            _deviceConfigurationWatchers.Add(deviceConfigurationWatcher);
                        }
                    }
                }
                else
                {
                    _agentLogger.Warn($"No Devices Found : Reading from : {configuration.Devices}");
                }

                OnStartAgentBeforeLoad(devices, initializeDataItems);

                OnStartAgentAfterLoad(devices, initializeDataItems);


                // Start Agent
                _mtconnectAgent.Start();

                // Start Agent Metrics
                StartMetrics();

                if (configuration.MonitorConfigurationFiles)
                {
                    // Set the Agent Configuration File Watcher
                    if (_agentConfigurationWatcher != null) _agentConfigurationWatcher.Dispose();
                    OnAgentConfigurationWatcherInitialize(configuration);
                    _agentConfigurationWatcher.ConfigurationUpdated += AgentConfigurationFileUpdated;
                    _agentConfigurationWatcher.ErrorReceived += AgentConfigurationFileError;
                }

                _started = true;
            }
        }

        public void StopAgent()
        {
            if (_started)
            {
                // Stop Device Configuration FileWatchers
                if (!_deviceConfigurationWatchers.IsNullOrEmpty())
                {
                    foreach (var deviceConfigurationFileWatcher in _deviceConfigurationWatchers) deviceConfigurationFileWatcher.Dispose();
                }

                if (_mtconnectAgent != null) _mtconnectAgent.Dispose();
                if (_agentConfigurationWatcher != null) _agentConfigurationWatcher.Dispose();
                if (_metricsTimer != null) _metricsTimer.Dispose();

                _deviceConfigurationWatchers.Clear();

                OnStopAgent();

                _started = false;
            }
        }


        protected virtual void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false) { }
        protected virtual void OnStartAgentAfterLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false) { }

        protected virtual void OnStopAgent() { }


        #region "Agent Configuration"

        protected virtual void OnAgentConfigurationWatcherInitialize(IAgentApplicationConfiguration configuration)
        {
            _agentConfigurationWatcher = new AgentConfigurationFileWatcher<AgentApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }

        private void AgentConfigurationFileUpdated(object sender, AgentConfiguration configuration)
        {
            var applicationConfiguration = configuration as IAgentApplicationConfiguration;
            if (applicationConfiguration != null)
            {
                _applicationLogger.Info($"[Application] : Agent Configuration File Updated ({configuration.Path})");

                StopAgent();

                OnAgentConfigurationUpdated(configuration);

                StartAgent(applicationConfiguration, _verboseLogging);

                if (OnRestart != null) OnRestart.Invoke(this, configuration);
            }
        }

        protected virtual void OnAgentConfigurationUpdated(AgentConfiguration configuration) { }

        private void AgentConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Agent Configuration File Error : {message}");
        }

        #endregion

        #region "Device Configuration"

        private void DeviceConfigurationFileUpdated(object sender, DeviceConfiguration configuration)
        {
            if (configuration != null)
            {
                _applicationLogger.Info($"[Application] : Device Configuration File Updated ({configuration.Path})");

                // Add Device to MTConnect Agent
                _mtconnectAgent.AddDevice(configuration);
            }
        }

        private void DeviceConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Device Configuration File Error : {message}");
        }

        #endregion

        #region "Metrics"

        private void StartMetrics()
        {
            if (_mtconnectAgent.Metrics != null)
            {
                int observationLastCount = 0;
                int observationDelta = 0;
                int assetLastCount = 0;
                int assetDelta = 0;
                var updateInterval = _mtconnectAgent.Metrics.UpdateInterval.TotalSeconds;
                var windowInterval = _mtconnectAgent.Metrics.WindowInterval.TotalMinutes;

                _metricsTimer = new System.Timers.Timer();
                _metricsTimer.Interval = updateInterval * 1000;
                _metricsTimer.Elapsed += (s, e) =>
                {
                    // Observations
                    var observationCount = _mtconnectAgent.Metrics.GetObservationCount();
                    var observationAverage = _mtconnectAgent.Metrics.ObservationAverage;
                    observationDelta = observationCount - observationLastCount;

                    _agentMetricLogger.Info("[Agent] : Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                    _agentMetricLogger.Info("[Agent] : Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                    // Assets
                    var assetCount = _mtconnectAgent.Metrics.GetAssetCount();
                    var assetAverage = _mtconnectAgent.Metrics.AssetAverage;
                    assetDelta = assetCount - assetLastCount;

                    _agentMetricLogger.Info("[Agent] : Assets - Delta for last " + updateInterval + " seconds: " + assetDelta);
                    _agentMetricLogger.Info("[Agent] : Assets - Average for last " + windowInterval + " minutes: " + Math.Round(assetAverage, 5));

                    observationLastCount = observationCount;
                    assetLastCount = assetCount;
                };
                _metricsTimer.Start();
            }           
        }

        #endregion

        #region "Help"

        private void PrintHelp()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name.ToLower();

            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.Write($"     {name} [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file] ");

            var overrideHelp = OnPrintHelpUsage();
            if (!string.IsNullOrEmpty(overrideHelp)) Console.Write(overrideHelp);
            Console.Write(Environment.NewLine);

            Console.WriteLine();
            Console.WriteLine("--------------------");
            Console.WriteLine();
            Console.WriteLine("Options :");
            Console.WriteLine();
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "help", "Prints usage information"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "install", "Install as a Service"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "install-start", "Install as a Service and Start the Service"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "start", "Start the Service"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "stop", "Stop the Service"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "remove", "Remove the Service"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "debug", "Runs the Agent in the terminal (with verbose logging)"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "run", "Runs the Agent in the terminal"));
            Console.WriteLine(string.Format("{0,15}  |  {1,5}", "run-service", "Runs the Agent as a Service"));
            Console.WriteLine();
            OnPrintHelpOptions();
            Console.WriteLine("Arguments :");
            Console.WriteLine("--------------------");
            Console.WriteLine();
            Console.WriteLine(string.Format("{0,20}  |  {1,5}", "configuration_file", "Specifies the Agent Configuration file to load"));
            Console.WriteLine(string.Format("{0,20}     {1,5}", "", "Default : agent.config.json"));
            Console.WriteLine();
            OnPrintHelpArguments();
        }

        protected virtual string OnPrintHelpUsage() { return ""; }

        protected virtual void OnPrintHelpOptions() { }

        protected virtual void OnPrintHelpArguments() { }

        #endregion

        #region "Logging"
   
        private void ObservationAdded(object sender, IObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                foreach (var value in observation.Values)
                {
                    _agentLogger.Debug($"[Agent] : Observation Added Successfully : {observation.DeviceUuid} : {observation.DataItemId} : {value.Key} = {value.Value}");
                }
            }
        }


        private void InvalidComponent(string deviceUuid, IComponent component, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : ComponentId = {component.Id} : {result.Message}");
        }

        private void InvalidComposition(string deviceUuid, IComposition composition, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : CompositionId = {composition.Id} : {result.Message}");
        }

        private void InvalidDataItem(string deviceUuid, IDataItem dataItem, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : DataItemId = {dataItem.Id} : {result.Message}");
        }

        private void InvalidObservation(string deviceUuid, string dataItemKey, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : DataItemKey = {dataItemKey} : {result.Message}");
        }

        private void InvalidAsset(IAsset asset, AssetValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {result.Message}");
        }

        #endregion
    }
}
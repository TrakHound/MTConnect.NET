// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Adapters;
using MTConnect.Configurations;
using MTConnect.Input;
using MTConnect.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace MTConnect.Applications
{
    /// <summary>
    /// An MTConnect Adapter Application base class supporting Command line arguments, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectAdapterApplication : IMTConnectAdapterApplication
    {
        private const string DefaultServiceLabel = "";
        private const string DefaultServiceName = "MTConnect-Adapter";
        private const string DefaultServiceDisplayName = "MTConnect Adapter";
        private const string DefaultServiceDescription = "MTConnect Adapter to transfer data to an MTConnect Agent";

        protected readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        protected readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");
        protected readonly Logger _moduleLogger = LogManager.GetLogger("module-logger");
        private readonly Dictionary<string, IMTConnectAdapter> _adapters = new Dictionary<string, IMTConnectAdapter>();
        private readonly object _lock = new object();

        private MTConnectAdapterModules _modules;
        private IMTConnectDataSource _dataSource;
        private bool _started = false;

        protected bool _verboseLogging = true;
        protected LogLevel _logLevel = LogLevel.Debug;
        protected IConfigurationFileWatcher<AdapterApplicationConfiguration> _adapterConfigurationWatcher;


        public string ServiceLabel { get; set; }

        public string ServiceName { get; set; }

        public string ServiceDisplayName { get; set; }

        public string ServiceDescription { get; set; }


        public IMTConnectDataSource DataSource => _dataSource;

        public event EventHandler<AdapterApplicationConfiguration> OnRestart;


        public MTConnectAdapterApplication(IMTConnectDataSource dataSource)
        {
            _dataSource = dataSource;
            if (_dataSource != null)
            {
                _dataSource.ObservationAdded += DataSourceObservationAdded;
                _dataSource.AssetAdded += DataSourceAssetAdded;
            }

            ServiceLabel = DefaultServiceLabel;
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;
        }


        /// <summary>
        /// Run Arguments [help|install|debug|run|reset] [configuration_file]
        /// </summary>
        public void Run(string[] args, bool isBlocking = false)
        {
            string command = "run";
            string configFile = null;


            string serviceLabel = ServiceLabel;
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
                    _applicationLogger.Info($"[Application] : Adapter Configuration Path = {configFile}");
                }
            }

            OnCommandLineArgumentsRead(args);


            // Copy Default Configuration File
            string yamlConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AdapterApplicationConfiguration.YamlFilename);
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AdapterApplicationConfiguration.DefaultYamlFilename);
            if (!File.Exists(yamlConfigPath) && File.Exists(defaultPath))
            {
                File.Copy(defaultPath, yamlConfigPath);
            }

            // Read the Agent Configuation File
            var configuration = AdapterApplicationConfiguration.Read<AdapterApplicationConfiguration>(configFile);
            if (configuration != null)
            {
                _adapterLogger.Info($"[Application] : Configuration File Read Successfully from: {configuration.Path}");

                // Set Service Name
                if (!string.IsNullOrEmpty(configuration.ServiceName)) serviceDisplayName = configuration.ServiceName;

                // Set Service Auto Start
                serviceStart = configuration.ServiceAutoStart;
            }
            else
            {
                _adapterLogger.Warn("[Application] : Error Reading Configuration File. Default Configuration is loaded.");

                configuration = new AdapterApplicationConfiguration();
            }

            // Declare a new Service (to use Service commands)
            Service service = null;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                service = new Service(this, serviceLabel, serviceName, serviceDisplayName, serviceDescription, serviceStart);
            }

            switch (command)
            {
                case "run":
                    _verboseLogging = false;
                    StartAdapter(configuration, _verboseLogging);

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
                    StartAdapter(configuration, _verboseLogging);

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


        public void StartAdapter(string configurationPath, bool verboseLogging = false)
        {
            var configuration = AdapterApplicationConfiguration.Read<AdapterApplicationConfiguration>(configurationPath);

            // Start the Agent with new Configuration
            StartAdapter(configuration, verboseLogging);
        }

        public void StartAdapter(IAdapterApplicationConfiguration configuration, bool verboseLogging = false)
        {
            if (!_started && configuration != null)
            {
                // Initialize Agent Modules
                _modules = new MTConnectAdapterModules(configuration);
                _modules.ModuleLoaded += ModuleLoaded;
                _modules.Load();

                foreach (var module in _modules.Modules)
                {
                    var adapter = new MTConnectAdapter(configuration.WriteInterval, configuration.EnableBuffer);
                    adapter.WriteObservationsFunction = module.AddObservations;
                    adapter.WriteAssetsFunction = module.AddAssets;
                    adapter.WriteDevicesFunction = module.AddDevices;
                    adapter.Start();

                    _adapters.Remove(module.Id);
                    _adapters.Add(module.Id, adapter);

                    module.Adapter = adapter;
                    module.LogReceived += ModuleLogReceived;
                    module.Start();
                }

                OnStartAdapter();

                // Start DataSource
                _dataSource.Configuration = configuration;
                _dataSource.Start();

                if (configuration.MonitorConfigurationFiles)
                {
                    // Set the Adapter Configuration File Watcher
                    if (_adapterConfigurationWatcher != null) _adapterConfigurationWatcher.Dispose();
                    _adapterConfigurationWatcher = new AdapterConfigurationFileWatcher<AdapterApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
                    _adapterConfigurationWatcher.ConfigurationUpdated += AdapterConfigurationFileUpdated;
                    _adapterConfigurationWatcher.ErrorReceived += AdapterConfigurationFileError;
                }

                _started = true;
            }
        }

        public void StopAdapter()
        {
            if (_started)
            {
                //if (_adapter != null) _adapter.Stop();
                if (_adapterConfigurationWatcher != null) _adapterConfigurationWatcher.Dispose();

                OnStopAdapter();

                if (_adapters != null)
                {
                    foreach (var adapter in _adapters) adapter.Value.Stop();
                    _adapters.Clear();
                }

                if (_modules != null) _modules.Stop();

                _started = false;
            }
        }

        //public void StartAdapter(IAdapterApplicationConfiguration configuration, bool verboseLogging = false)
        //{
        //    if (!_started && configuration != null)
        //    {               
        //        // Create MTConnectAgentBroker
        //        if (configuration.EnableBuffer)
        //        {
        //            _adapter = new ShdrIntervalQueueAdapter(configuration.DeviceKey, configuration.Port, configuration.Heartbeat, configuration.WriteInterval);
        //        }
        //        else
        //        {
        //            _adapter = new ShdrIntervalAdapter(configuration.DeviceKey, configuration.Port, configuration.Heartbeat, configuration.WriteInterval);
        //        }

        //        _adapter.Timeout = configuration.Timeout;
        //        _adapter.MultilineAssets = configuration.MultilineAssets;
        //        _adapter.MultilineDevices = configuration.MultilineDevices;
        //        _adapter.FilterDuplicates = configuration.FilterDuplicates;
        //        _adapter.OutputTimestamps = configuration.OutputTimestamps;

        //        if (!string.IsNullOrEmpty(_adapter.DeviceKey))
        //        {
        //            _adapterLogger.Info($"[Adapter] : Listening on TCP Port {_adapter.Port} for Device ({_adapter.DeviceKey})...");
        //        }
        //        else
        //        {
        //            _adapterLogger.Info($"[Adapter] : Listening on TCP Port {_adapter.Port}...");
        //        }

        //        if (verboseLogging)
        //        {
        //            _adapter.AgentConnected += AgentConnected;
        //            _adapter.AgentDisconnected += AgentDisconnected;
        //            _adapter.AgentConnectionError += AgentConnectionError;
        //            _adapter.LineSent += LineSent;
        //            _adapter.PingReceived += PingReceived;
        //            _adapter.PongSent += PongSent;
        //            _adapter.SendError += SendError;
        //        }

        //        // Set Engine Adapter
        //        if (_engine != null) _engine.Adapter = _adapter;

        //        OnStartAdapter();

        //        // Start Adapter
        //        _adapter.Start();

        //        if (configuration.MonitorConfigurationFiles)
        //        {
        //            // Set the Adapter Configuration File Watcher
        //            if (_adapterConfigurationWatcher != null) _adapterConfigurationWatcher.Dispose();
        //            OnAdapterConfigurationWatcherInitialize(configuration);
        //            _adapterConfigurationWatcher.ConfigurationUpdated += AdapterConfigurationFileUpdated;
        //            _adapterConfigurationWatcher.ErrorReceived += AdapterConfigurationFileError;
        //        }

        //        _started = true;
        //    }
        //}

        //public void StopAdapter()
        //{
        //    if (_started)
        //    {
        //        if (_adapter != null) _adapter.Stop();
        //        if (_adapterConfigurationWatcher != null) _adapterConfigurationWatcher.Dispose();

        //        OnStopAdapter();

        //        _started = false;
        //    }
        //}


        protected virtual void OnStartAdapter() 
        {
            // Start DataSource
            //if (_dataSource != null) _dataSource.Start();
        }

        protected virtual void OnStopAdapter() 
        {
            // Stop DataSource
            //if (_dataSource != null) _dataSource.Stop();
        }


        #region "Adapter Configuration"

        //protected virtual void OnAdapterConfigurationWatcherInitialize(IAdapterApplicationConfiguration configuration)
        //{
        //    _adapterConfigurationWatcher = new AdapterConfigurationFileWatcher<AdapterApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        //}

        private void AdapterConfigurationFileUpdated(object sender, AdapterApplicationConfiguration configuration)
        {
            var applicationConfiguration = configuration as IAdapterApplicationConfiguration;
            if (applicationConfiguration != null)
            {
                _applicationLogger.Info($"[Application] : Adapter Configuration File Updated ({configuration.Path})");

                StopAdapter();

                OnAdapterConfigurationUpdated(configuration);

                StartAdapter(applicationConfiguration, _verboseLogging);

                if (OnRestart != null) OnRestart.Invoke(this, configuration);
            }
        }

        protected virtual void OnAdapterConfigurationUpdated(AdapterApplicationConfiguration configuration) { }

        private void AdapterConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Adapter Configuration File Error : {message}");
        }

        #endregion

        #region "DataSource"

        private void DataSourceObservationAdded(object sender, IObservationInput observation)
        {
            Dictionary<string, IMTConnectAdapter> adapters;
            lock (_lock) adapters = _adapters;

            if (!adapters.IsNullOrEmpty())
            {
                foreach (var adapter in adapters)
                {
                    adapter.Value.AddObservation(observation);
                }
            }
        }

        private void DataSourceAssetAdded(object sender, IAssetInput asset)
        {
            Dictionary<string, IMTConnectAdapter> adapters;
            lock (_lock) adapters = _adapters;

            if (!adapters.IsNullOrEmpty())
            {
                foreach (var adapter in adapters)
                {
                    adapter.Value.AddAsset(asset);
                }
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

        private void ModuleLoaded(object sender, IMTConnectAdapterModule module)
        {
            _applicationLogger.Debug($"[Application] : Module Loaded : " + module.GetType().Name);
        }

        private void ModuleLogReceived(object sender, MTConnectLogLevel logLevel, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                switch (logLevel)
                {
                    case MTConnectLogLevel.Fatal: _moduleLogger.Fatal(message); break;
                    case MTConnectLogLevel.Error: _moduleLogger.Error(message); break;
                    case MTConnectLogLevel.Warning: _moduleLogger.Warn(message); break;
                    case MTConnectLogLevel.Information: _moduleLogger.Info(message); break;
                    case MTConnectLogLevel.Debug: _moduleLogger.Debug(message); break;
                    case MTConnectLogLevel.Trace: _moduleLogger.Trace(message); break;
                }
            }
        }

        #endregion

    }
}
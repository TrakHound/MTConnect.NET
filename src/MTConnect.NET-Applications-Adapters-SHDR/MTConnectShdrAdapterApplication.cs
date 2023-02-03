// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Adapters.Shdr;
using MTConnect.Configurations;
using NLog;
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace MTConnect.Applications.Adapters
{
    /// <summary>
    /// An MTConnect SHDR Adapter Application base class supporting Command line arguments, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectShdrAdapterApplication<TConfiguration> : IMTConnectShdrAdapterApplication where TConfiguration : ShdrAdapterApplicationConfiguration
    {
        private const string DefaultServiceLabel = "";
        private const string DefaultServiceName = "MTConnect-Adapter";
        private const string DefaultServiceDisplayName = "MTConnect SHDR Adapter";
        private const string DefaultServiceDescription = "MTConnect SHDR Adapter to transfer data to an MTConnect Agent";

        protected readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        protected readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");

        private MTConnectShdrAdapterEngine<TConfiguration> _engine;
        private ShdrAdapter _adapter;
        private bool _started = false;

        protected bool _verboseLogging = true;
        protected LogLevel _logLevel = LogLevel.Debug;
        protected IConfigurationFileWatcher<ShdrAdapterApplicationConfiguration> _adapterConfigurationWatcher;


        public string ServiceLabel { get; set; }

        public string ServiceName { get; set; }

        public string ServiceDisplayName { get; set; }

        public string ServiceDescription { get; set; }

        protected Type ConfigurationType { get; set; }


        public MTConnectShdrAdapterEngine<TConfiguration> Engine => _engine;

        public ShdrAdapter Adapter => _adapter;

        public EventHandler<ShdrAdapterApplicationConfiguration> OnRestart { get; set; }


        public MTConnectShdrAdapterApplication(MTConnectShdrAdapterEngine<TConfiguration> engine)
        {
            _engine = engine;

            ServiceLabel = DefaultServiceLabel;
            ServiceName = DefaultServiceName;
            ServiceDisplayName = DefaultServiceDisplayName;
            ServiceDescription = DefaultServiceDescription;

            if (ConfigurationType == null) ConfigurationType = typeof(ShdrAdapterApplicationConfiguration);
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
            string yamlConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ShdrAdapterApplicationConfiguration.YamlFilename);
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ShdrAdapterApplicationConfiguration.DefaultYamlFilename);
            if (!File.Exists(yamlConfigPath) && File.Exists(defaultPath))
            {
                File.Copy(defaultPath, yamlConfigPath);
            }

            // Read the Agent Configuation File
            var configuration = OnConfigurationFileRead(configFile);
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

                configuration = new ShdrAdapterApplicationConfiguration();
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

        protected virtual IShdrAdapterApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            return ShdrAdapterApplicationConfiguration.Read<ShdrAdapterApplicationConfiguration>(configurationPath);
        }


        public void StartAdapter(string configurationPath, bool verboseLogging = false)
        {
            var configuration = OnConfigurationFileRead(configurationPath);

            // Start the Agent with new Configuration
            StartAdapter(configuration, verboseLogging);
        }

        public void StartAdapter(IShdrAdapterApplicationConfiguration configuration, bool verboseLogging = false)
        {
            if (!_started && configuration != null)
            {               
                // Create MTConnectAgentBroker
                if (configuration.EnableBuffer)
                {
                    _adapter = new ShdrIntervalQueueAdapter(configuration.DeviceKey, configuration.Port, configuration.Heartbeat, configuration.WriteInterval);
                }
                else
                {
                    _adapter = new ShdrIntervalAdapter(configuration.DeviceKey, configuration.Port, configuration.Heartbeat, configuration.WriteInterval);
                }

                _adapter.Timeout = configuration.Timeout;
                _adapter.MultilineAssets = configuration.MultilineAssets;
                _adapter.MultilineDevices = configuration.MultilineDevices;
                _adapter.FilterDuplicates = configuration.FilterDuplicates;
                _adapter.OutputTimestamps = configuration.OutputTimestamps;

                if (!string.IsNullOrEmpty(_adapter.DeviceKey))
                {
                    _adapterLogger.Info($"[Adapter] : Listening on TCP Port {_adapter.Port} for Device ({_adapter.DeviceKey})...");
                }
                else
                {
                    _adapterLogger.Info($"[Adapter] : Listening on TCP Port {_adapter.Port}...");
                }

                if (verboseLogging)
                {
                    _adapter.AgentConnected += AgentConnected;
                    _adapter.AgentDisconnected += AgentDisconnected;
                    _adapter.AgentConnectionError += AgentConnectionError;
                    _adapter.LineSent += LineSent;
                    _adapter.PingReceived += PingReceived;
                    _adapter.PongSent += PongSent;
                    _adapter.SendError += SendError;
                }
           
                // Set Engine Adapter
                if (_engine != null) _engine.Adapter = _adapter;

                OnStartAdapter();

                // Start Adapter
                _adapter.Start();

                if (configuration.MonitorConfigurationFiles)
                {
                    // Set the Adapter Configuration File Watcher
                    if (_adapterConfigurationWatcher != null) _adapterConfigurationWatcher.Dispose();
                    OnAdapterConfigurationWatcherInitialize(configuration);
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
                if (_adapter != null) _adapter.Stop();
                if (_adapterConfigurationWatcher != null) _adapterConfigurationWatcher.Dispose();

                OnStopAdapter();

                _started = false;
            }
        }


        protected virtual void OnStartAdapter() 
        {
            // Start Engine
            if (_engine != null) _engine.Start();
        }

        protected virtual void OnStopAdapter() 
        {
            // Stop Engine
            if (_engine != null) _engine.Stop();
        }


        #region "Adapter Configuration"

        protected virtual void OnAdapterConfigurationWatcherInitialize(IShdrAdapterApplicationConfiguration configuration)
        {
            _adapterConfigurationWatcher = new AdapterConfigurationFileWatcher<ShdrAdapterApplicationConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }

        private void AdapterConfigurationFileUpdated(object sender, ShdrAdapterApplicationConfiguration configuration)
        {
            var applicationConfiguration = configuration as IShdrAdapterApplicationConfiguration;
            if (applicationConfiguration != null)
            {
                _applicationLogger.Info($"[Application] : Adapter Configuration File Updated ({configuration.Path})");

                StopAdapter();

                OnAdapterConfigurationUpdated(configuration);

                StartAdapter(applicationConfiguration, _verboseLogging);

                if (OnRestart != null) OnRestart.Invoke(this, configuration);
            }
        }

        protected virtual void OnAdapterConfigurationUpdated(ShdrAdapterApplicationConfiguration configuration) { }

        private void AdapterConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Adapter Configuration File Error : {message}");
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

        private void AgentConnected(object sender, string clientId)
        {
            _adapterLogger.Info($"[Adapter] : Connected to Agent : {clientId}");
        }

        private void AgentDisconnected(object sender, string clientId)
        {
            _adapterLogger.Info($"[Adapter] : Disconnected from Agent : {clientId}");
        }

        private void AgentConnectionError(object sender, string clientId)
        {
            _adapterLogger.Info($"[Adapter] : Could not connect to Agent : {clientId}");
        }

        private void LineSent(object sender, AdapterEventArgs args)
        {
            _adapterLogger.Debug($"[Adapter] : SHDR Protocol Line Sent : Client ID = {args.ClientId} : {args.Message}");
        }

        private void SendError(object sender, AdapterEventArgs args)
        {
            _adapterLogger.Info($"[Adapter] : Error Sending SHDR Protocol Line : Client ID = {args.ClientId} : {args.Message}");
        }

        private void PingReceived(object sender, string clientId)
        {
            _adapterLogger.Debug($"[Adapter] : Agent Ping Received : Client ID = {clientId}");
        }

        private void PongSent(object sender, string clientId)
        {
            _adapterLogger.Debug($"[Adapter] : Agent Pong Sent : Client ID = {clientId}");
        }

        #endregion
    }
}
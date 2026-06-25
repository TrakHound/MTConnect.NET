// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Logging;
using MTConnect.Observations;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;

namespace MTConnect.Applications
{
    /// <summary>
    /// An MTConnect Agent Application base class supporting Command line arguments, Device management, Buffer management, Logging, Windows Service, and Configuration File management
    /// </summary>
    public class MTConnectAgentApplication : IMTConnectAgentApplication
    {
        private const string DefaultServiceName = "MTConnect.NET-Agent";
        private const string DefaultServiceDisplayName = "MTConnect.NET Agent";
        private const string DefaultServiceDescription = "MTConnect Agent to provide access to device information using the MTConnect Standard";

        /// <summary>NLog logger for application-level diagnostics
        /// (startup, shutdown, configuration load).</summary>
        protected readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        /// <summary>NLog logger for the agent broker's internal events
        /// (device add, observation enqueue).</summary>
        protected readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        /// <summary>NLog logger for the periodic agent-metrics emitter
        /// (observation rate, buffer depth).</summary>
        protected readonly Logger _agentMetricLogger = LogManager.GetLogger("agent-metrics");
        /// <summary>NLog logger for input-validation issues
        /// (rejected observations, type mismatches).</summary>
        protected readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation");
        /// <summary>NLog logger shared by every configured agent
        /// module.</summary>
        protected readonly Logger _moduleLogger = LogManager.GetLogger("module");
        /// <summary>NLog logger shared by every configured agent
        /// processor.</summary>
        protected readonly Logger _processorLogger = LogManager.GetLogger("processor");

        private readonly List<DeviceConfigurationFileWatcher> _deviceConfigurationWatchers = new List<DeviceConfigurationFileWatcher>();
        private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
        private readonly object _lock = new object();

#if NET5_0_OR_GREATER
        /// <summary>NLog log-level applied to every internal logger.
        /// Defaults to <see cref="LogLevel.Debug"/>; the <c>debug</c>
        /// and <c>trace</c> CLI commands override it.</summary>
        protected LogLevel _logLevel = LogLevel.Debug;
#else
        /// <summary>NLog log-level applied to every internal logger.</summary>
        protected LogLevel _logLevel = LogLevel.Debug;
#endif

        private MTConnectAgentBroker _mtconnectAgent;
        private IMTConnectObservationBuffer _observationBuffer;
        private MTConnectAssetFileBuffer _assetBuffer;
        private MTConnectAgentModules _modules;
        private MTConnectAgentProcessors _processors;
        private IAgentApplicationConfiguration _initialAgentConfiguration;
        /// <summary>File-system watcher that reloads the agent
        /// configuration when the underlying YAML / JSON file
        /// changes.</summary>
        protected IAgentConfigurationFileWatcher _agentConfigurationWatcher;
        private System.Timers.Timer _metricsTimer;
        private bool _started = false;
        /// <summary>When <c>true</c>, the agent emits a console header
        /// at startup and includes per-component summaries in the
        /// startup log.</summary>
        protected bool _verboseLogging = true;


        /// <inheritdoc />
        public string ServiceName { get; set; }

        /// <inheritdoc />
        public string ServiceDisplayName { get; set; }

        /// <inheritdoc />
        public string ServiceDescription { get; set; }

        /// <inheritdoc />
        public IMTConnectAgentBroker Agent => _mtconnectAgent;


        /// <inheritdoc />
        public event EventHandler<AgentConfiguration> OnRestart;


        /// <summary>
        /// Initialises a new instance, optionally seeded with an
        /// already-loaded <paramref name="agentConfiguration"/>. When a
        /// configuration is supplied the service-name / display-name /
        /// description triple is taken from it; otherwise the bundled
        /// <c>MTConnect.NET-Agent</c> defaults apply.
        /// </summary>
        /// <param name="agentConfiguration">Pre-loaded configuration,
        /// or <c>null</c> to load from disk during <see cref="Run"/>.</param>
        public MTConnectAgentApplication(IAgentApplicationConfiguration agentConfiguration = null)
        {
            if (agentConfiguration != null)
            {
                ServiceName = !string.IsNullOrEmpty(agentConfiguration.ServiceName) ? agentConfiguration.ServiceName : DefaultServiceName;
                ServiceDisplayName = !string.IsNullOrEmpty(agentConfiguration.ServiceDisplayName) ? agentConfiguration.ServiceDisplayName : DefaultServiceDisplayName;
                ServiceDescription = !string.IsNullOrEmpty(agentConfiguration.ServiceDescription) ? agentConfiguration.ServiceDescription : DefaultServiceDescription;
            }

            _initialAgentConfiguration = agentConfiguration;
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


            // Copy Default NLog Configuration File
            string logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NLog.config");
            string defaultLogConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NLog.default.config");
            if (!File.Exists(logConfigPath) && File.Exists(defaultLogConfigPath))
            {
                File.Copy(defaultLogConfigPath, logConfigPath);

                LogManager.Configuration = LogManager.Configuration.Reload();
                LogManager.ReconfigExistingLoggers();
            }


            if (string.IsNullOrEmpty(configFile))
            {
                // Convert Json Configuration File to YAML
                string jsonConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.JsonFilename);
                if (File.Exists(jsonConfigPath))
                {
                    var dummyConfiguration = OnConfigurationFileRead(jsonConfigPath);
                    if (dummyConfiguration != null) dummyConfiguration.SaveYaml();
                }

                // Copy Default Configuration File
                string yamlConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.YamlFilename);
                string defaultConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.DefaultYamlFilename);
                if (!File.Exists(yamlConfigPath) && !File.Exists(jsonConfigPath) && File.Exists(defaultConfigPath))
                {
                    File.Copy(defaultConfigPath, yamlConfigPath);
                }
            }

            // Read the Agent Configuation File
            var configuration = _initialAgentConfiguration;
            if (configuration == null)
            {
                configuration = OnConfigurationFileRead(configFile);
                if (configuration != null) _applicationLogger.Info($"Configuration File Read Successfully from: {configuration.Path}");
            }
            if (configuration != null)
            {
                // Set Service Name
                if (!string.IsNullOrEmpty(configuration.ServiceName)) serviceName = configuration.ServiceName;
                if (!string.IsNullOrEmpty(configuration.ServiceDisplayName)) serviceDisplayName = configuration.ServiceDisplayName;
                if (!string.IsNullOrEmpty(configuration.ServiceDescription)) serviceDescription = configuration.ServiceDescription;

                // Set Service Auto Start
                serviceStart = configuration.ServiceAutoStart;
            }


            if (configuration != null)
            {
                // Declare a new Service (to use Service commands)
                Service service = null;
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    service = new Service(this, serviceName, serviceDisplayName, serviceDescription, serviceStart);
                    service.LogInformationReceived += ServiceLogInformationReceived;
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
                            System.ServiceProcess.ServiceBase.Run(service);
                        }
                        else _applicationLogger.Info($"'Run-Service' Command is not supported on this Operating System");

                        break;

                    case "debug":

                        if (LogManager.Configuration != null)
                        {
                            var consoleTarget = LogManager.Configuration.FindTargetByName("logConsole");
                            if (consoleTarget != null)
                            {
                                LogManager.Configuration.AddRule(new LoggingRule("*", LogLevel.Debug, consoleTarget));
                            }
                        }

                        StartAgent(configuration, true);

                        if (isBlocking) while (true) Thread.Sleep(100); // Block (exit console by 'Ctrl + C')
                        else break;

                    case "trace":

                        if (LogManager.Configuration != null)
                        {
                            var consoleTarget = LogManager.Configuration.FindTargetByName("logConsole");
                            if (consoleTarget != null)
                            {
                                LogManager.Configuration.AddRule(new LoggingRule("*", LogLevel.Trace, consoleTarget));
                            }
                        }

                        StartAgent(configuration, true);

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


                    case "reset":

                        _applicationLogger.Info("Reset Buffer requested..");

                        // Clear the Observation Buffer
                        MTConnectObservationFileBuffer.Reset(configuration.DurableBufferPath);
                        _applicationLogger.Info("Observation Buffer Reset Successfully");

                        // Clear the Asset Buffer
                        MTConnectAssetFileBuffer.Reset(configuration.DurableBufferPath);
                        _applicationLogger.Info("Asset Buffer Reset Successfully");

                        // Clear the Index
                        FileIndex.Reset(configuration.DurableBufferPath);
                        _applicationLogger.Info("Indexes Reset Successfully");

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
            else
            {
                var configurationPath = configFile;
                if (!Path.IsPathRooted(configurationPath))
                {
                    configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }

                _applicationLogger.Fatal($"Error Reading Configuration File. Path = {configurationPath}");

                //_applicationLogger.Warn("Error Reading Configuration File. Default Configuration is loaded.");

                //configuration = new AgentApplicationConfiguration();
            }
        }

        /// <summary>
        /// Extension hook: invoked once the host has finished parsing
        /// the command-line arguments. Override in a derived class to
        /// inspect or react to the raw arguments.
        /// </summary>
        /// <param name="args">Raw command-line arguments.</param>
        protected virtual void OnCommandLineArgumentsRead(string[] args) { }

        /// <summary>
        /// Extension hook: loads the agent configuration from
        /// <paramref name="configurationPath"/>. The default
        /// implementation delegates to
        /// <see cref="AgentConfiguration.Read{T}(string)"/>; override
        /// to swap the loader (e.g. to read from an environment-
        /// variable backing store).
        /// </summary>
        /// <param name="configurationPath">Path to the agent's YAML /
        /// JSON configuration file.</param>
        /// <returns>The loaded configuration, or <c>null</c> when the
        /// file could not be read.</returns>
        protected virtual IAgentApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            // Read the Configuration File
            return AgentConfiguration.Read<AgentApplicationConfiguration>(configurationPath);
        }


        /// <summary>
        /// Loads the configuration from <paramref name="configurationPath"/>
        /// (via the <see cref="OnConfigurationFileRead"/> hook) and
        /// delegates to the in-memory overload.
        /// </summary>
        /// <param name="configurationPath">Path to the agent's YAML /
        /// JSON configuration file.</param>
        /// <param name="verboseLogging">When <c>true</c>, the agent
        /// emits a console header at startup and includes per-component
        /// summaries in the startup log.</param>
        public void StartAgent(string configurationPath, bool verboseLogging = false)
        {
            var configuration = OnConfigurationFileRead(configurationPath);

            // Start the Agent with new Configuration
            StartAgent(configuration, verboseLogging);
        }

        /// <summary>
        /// Builds the agent broker, loads every device + module +
        /// processor described in <paramref name="configuration"/>, and
        /// starts the agent. Idempotent — a second call while the agent
        /// is running is a no-op.
        /// </summary>
        /// <param name="configuration">Already-loaded agent
        /// configuration.</param>
        /// <param name="verboseLogging">When <c>true</c>, the agent
        /// emits a console header at startup and includes per-component
        /// summaries in the startup log.</param>
        public void StartAgent(IAgentApplicationConfiguration configuration, bool verboseLogging = false)
        {
            if (!_started && configuration != null)
            {
                GarbageCollector.Initialize();

                _deviceConfigurationWatchers.Clear();
                var initializeDataItems = true;

                // Read Agent Information File
                var existingAgentInformation = MTConnectAgentInformation.Read();
                var freshlyConstructed = (existingAgentInformation == null);
                var agentInformation = existingAgentInformation ?? new MTConnectAgentInformation();

                // Apply explicit AgentUuid config override (if set). This
                // pins the Agent meta-device UUID across restarts without
                // requiring agent.information.json to be present on disk,
                // and takes precedence over any UUID previously stored in
                // that file. See MTConnect v2.7 XSD UuidType ("for its
                // entire life") vs Header.instanceId (per-boot).
                if (!string.IsNullOrEmpty(configuration.AgentUuid))
                {
                    agentInformation.Uuid = configuration.AgentUuid;
                }

                // When no operator config override and no persisted state file,
                // derive a deterministic UUID v5 (RFC 4122 §4.3, DNS namespace,
                // SHA-1) from ServiceName:port so the Agent meta-device satisfies
                // UuidType's "for it's entire life" annotation across every restart
                // in the ephemeral-container deployment path.  Mirrors cppagent's
                // name_generator prior art.  Port is 0 (sentinel) because
                // IAgentApplicationConfiguration does not surface a listener-port
                // property; the seed is still unique per ServiceName.
                if (freshlyConstructed && string.IsNullOrEmpty(configuration.AgentUuid))
                {
                    agentInformation.Uuid = DeterministicAgentUuid.Derive(
                        configuration.ServiceName,
                        System.Environment.MachineName,
                        port: 0);
                }

                // Create Observation File Buffer
                if (configuration.Durable)
                {
                    // Create Observation File Buffer
                    var observationBuffer = new MTConnectObservationFileBuffer(configuration, configuration.DurableBufferPath);
                    observationBuffer.UseCompression = configuration.UseBufferCompression;
                    observationBuffer.BufferLoadStarted += ObservationBufferStarted;
                    observationBuffer.BufferLoadCompleted += ObservationBufferCompleted;
                    observationBuffer.BufferRetentionCompleted += ObservationBufferRetentionCompleted;

                    // Create Asset File Buffer
                    _assetBuffer = new MTConnectAssetFileBuffer(configuration, configuration.DurableBufferPath);
                    _assetBuffer.UseCompression = configuration.UseBufferCompression;
                    _assetBuffer.BufferLoadStarted += AssetBufferStarted;
                    _assetBuffer.BufferLoadCompleted += AssetBufferCompleted;

                    // Read Buffer Observations
                    initializeDataItems = !observationBuffer.Load();

                    // Read Buffer Assets
                    _assetBuffer.Load();

                    _observationBuffer = observationBuffer;
                }
                else
                {
                    _observationBuffer = new MTConnectObservationBuffer(configuration);
                }

                if (!configuration.Durable || initializeDataItems)
                {
                    // Reset InstanceId with strict-monotonic guarantee. The Max() floor
                    // makes the counter time-meaningful while the +1 defeats same-second
                    // collisions on two consecutive restarts (per the SysML XMI MUST
                    // clause at MTConnectSysMLModel_V2.7.xml line 15608: "MUST be changed
                    // to a different unique number each time the buffer is cleared").
                    // UnixDateTime.Now mirrors MTConnectAgentInformation's parameterless
                    // ctor at libraries/MTConnect.NET-Common/Agents/MTConnectAgentInformation.cs
                    // line 39; the +1/Max combination extends that to strict monotonicity.
                    agentInformation.InstanceId = Math.Max(
                        agentInformation.InstanceId + 1,
                        (ulong)UnixDateTime.Now);
                }

                // Save the AgentInformation file
                agentInformation.Save();


                // Create MTConnectAgentBroker
                _mtconnectAgent = new MTConnectAgentBroker(configuration, _observationBuffer, _assetBuffer, agentInformation.Uuid, agentInformation.InstanceId, agentInformation.DeviceModelChangeTime, initializeDataItems);

                // Initialize Agent Modules
                _modules = new MTConnectAgentModules(configuration, _mtconnectAgent);
                _modules.ModuleLoaded += ModuleLoaded;
                _modules.LogReceived += ModuleLogReceived;
                _modules.Load();

                // Read Indexes for Buffer
                if (configuration.Durable)
                {
                    // Read Device Indexes
                    _mtconnectAgent.InitializeDeviceIndex(FileIndex.ToDictionary(FileIndex.FromFile(configuration.DurableBufferPath, FileIndex.DevicesFileName)));

                    // Read DataItem Indexes
                    _mtconnectAgent.InitializeDataItemIndex(FileIndex.ToDictionary(FileIndex.FromFile(configuration.DurableBufferPath, FileIndex.DataItemsFileName)));
                }

                if (verboseLogging)
                {
                    _mtconnectAgent.DevicesRequestReceived += DevicesRequested;
                    _mtconnectAgent.DevicesResponseSent += DevicesSent;
                    _mtconnectAgent.StreamsRequestReceived += StreamsRequested;
                    _mtconnectAgent.StreamsResponseSent += StreamsSent;
                    _mtconnectAgent.AssetsRequestReceived += AssetsRequested;
                    _mtconnectAgent.DeviceAssetsRequestReceived += DeviceAssetsRequested;
                    _mtconnectAgent.AssetsResponseSent += AssetsSent;
                    //_mtconnectAgent.ObservationAdded += ObservationAdded;
                    _mtconnectAgent.InvalidComponentAdded += InvalidComponent;
                    _mtconnectAgent.InvalidCompositionAdded += InvalidComposition;
                    _mtconnectAgent.InvalidDataItemAdded += InvalidDataItem;
                    _mtconnectAgent.InvalidObservationAdded += InvalidObservation;
                    _mtconnectAgent.InvalidAssetAdded += InvalidAsset;
                }

                OnStartAgentBeforeLoad(null, initializeDataItems);
                _modules.StartBeforeLoad(initializeDataItems);

                // Read Device Configuration Files
                IEnumerable<DeviceConfiguration> deviceConfigurations = null;
                var devicesPath = configuration.Devices;
                if (!string.IsNullOrEmpty(devicesPath))
                {
                    deviceConfigurations = DeviceConfiguration.FromFiles(devicesPath, DocumentFormat.XML);
                    if (!deviceConfigurations.IsNullOrEmpty())
                    {
                        // Add Device(s) to Agent
                        foreach (var device in deviceConfigurations)
                        {
                            _agentLogger.Info($"Device ({device.Name}) Read From File : {device.Path}");

                            _mtconnectAgent.AddDevice(device, initializeDataItems);
                        }

                        if (configuration.MonitorConfigurationFiles)
                        {
                            // Set Device Configuration File Watcher
                            var paths = deviceConfigurations.Select(o => o.Path).Distinct();
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
                }
                

                // Initilialize Processors
                _processors = new MTConnectAgentProcessors(configuration);
                _processors.ProcessorLoaded += ProcessorLoaded;
                _processors.LogReceived += ProcessorLogReceived;
                _processors.Load();
                _mtconnectAgent.ProcessObservationFunction = _processors.Process;


                // Initialize Agent Current Observations/Conditions
                // This updates the MTConnectAgent's cache used to determine duplicate observations
                if (_observationBuffer != null)
                {
                    _mtconnectAgent.InitializeCurrentObservations(_observationBuffer.CurrentObservations.Values);
                    _mtconnectAgent.InitializeCurrentObservations(_observationBuffer.CurrentConditions.SelectMany(o => o.Value));
                }

                _modules.StartAfterLoad(initializeDataItems);
                OnStartAgentAfterLoad(deviceConfigurations, initializeDataItems);

                // Save Indexes for Buffer
                if (configuration.Durable)
                {
                    // Save Device Indexes
                    FileIndex.ToFile(configuration.DurableBufferPath, FileIndex.DevicesFileName, FileIndex.Create(_mtconnectAgent.DeviceIndexes));

                    // Save DataItem Indexes
                    FileIndex.ToFile(configuration.DurableBufferPath, FileIndex.DataItemsFileName, FileIndex.Create(_mtconnectAgent.DataItemIndexes));
                }

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

        /// <summary>
        /// Stops every running component (device watchers, modules,
        /// processors, configuration watcher, metrics timer) and
        /// disposes the agent broker. Idempotent if the agent never
        /// started.
        /// </summary>
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
                if (_observationBuffer != null) _observationBuffer.Dispose();
                if (_assetBuffer != null) _assetBuffer.Dispose();
                if (_agentConfigurationWatcher != null) _agentConfigurationWatcher.Dispose();
                if (_metricsTimer != null) _metricsTimer.Dispose();

                _deviceConfigurationWatchers.Clear();

                OnStopAgent();

                if (_modules != null) _modules.Stop();
                if (_processors != null) _processors.Dispose();

                _started = false;
            }
        }


        /// <summary>
        /// Extension hook: fires after the agent broker is created but
        /// before devices are added to it. Override to register custom
        /// data-items or initialise extension state that the device
        /// load relies on.
        /// </summary>
        /// <param name="devices">Devices that are about to be added.</param>
        /// <param name="initializeDataItems">Whether the agent should
        /// seed observations for every data item.</param>
        protected virtual void OnStartAgentBeforeLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false) { }

        /// <summary>
        /// Extension hook: fires after every device + module +
        /// processor has been added to the agent broker. Override to
        /// register custom subscribers that depend on the fully-built
        /// agent.
        /// </summary>
        /// <param name="devices">Devices that have been added.</param>
        /// <param name="initializeDataItems">Whether the agent seeded
        /// observations for every data item.</param>
        protected virtual void OnStartAgentAfterLoad(IEnumerable<DeviceConfiguration> devices, bool initializeDataItems = false) { }

        /// <summary>
        /// Extension hook: fires once <see cref="StopAgent"/> has
        /// disposed every internal component. Override to release any
        /// extension state held by a derived class.
        /// </summary>
        protected virtual void OnStopAgent() { }


        #region "Agent Configuration"

        /// <summary>
        /// Extension hook: creates the file-system watcher that
        /// reloads the agent configuration on disk changes. The
        /// default implementation wires a
        /// <see cref="AgentConfigurationFileWatcher{T}"/> against the
        /// file path declared in <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">Currently-active configuration.</param>
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

        /// <summary>
        /// Extension hook: fires after the configuration watcher
        /// reloads the configuration from disk. Override to react to
        /// runtime configuration changes (e.g. update derived state in
        /// a host application).
        /// </summary>
        /// <param name="configuration">Freshly-reloaded
        /// configuration.</param>
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

                    _agentMetricLogger.Debug("Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                    _agentMetricLogger.Debug("Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                    // Assets
                    var assetCount = _mtconnectAgent.Metrics.GetAssetCount();
                    var assetAverage = _mtconnectAgent.Metrics.AssetAverage;
                    assetDelta = assetCount - assetLastCount;

                    _agentMetricLogger.Debug("Assets - Delta for last " + updateInterval + " seconds: " + assetDelta);
                    _agentMetricLogger.Debug("Assets - Average for last " + windowInterval + " minutes: " + Math.Round(assetAverage, 5));

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
            var assembly = Assembly.GetEntryAssembly();
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

        /// <summary>
        /// Extension hook: returns extra text appended to the
        /// <c>Usage:</c> line printed by the <c>help</c> command.
        /// Override in a host that adds custom commands or arguments.
        /// </summary>
        /// <returns>Extra usage text, or an empty string for none.</returns>
        protected virtual string OnPrintHelpUsage() { return ""; }

        /// <summary>
        /// Extension hook: prints extra rows in the <c>Options:</c>
        /// section of the <c>help</c> command. The default
        /// implementation is empty.
        /// </summary>
        protected virtual void OnPrintHelpOptions() { }

        /// <summary>
        /// Extension hook: prints extra rows in the <c>Arguments:</c>
        /// section of the <c>help</c> command. The default
        /// implementation is empty.
        /// </summary>
        protected virtual void OnPrintHelpArguments() { }

        #endregion

        #region "Logging"

        private void ServiceLogInformationReceived(object sender, string message)
        {
            _applicationLogger.Info("Windows Service : " + message);
        }


        private void ModuleLoaded(object sender, IMTConnectAgentModule module)
        {
            _applicationLogger.Info("Module Loaded : " + module.Id);
        }

        private void ModuleLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var module = (IMTConnectAgentModule)sender;

                var loggerId = logId;
                if (string.IsNullOrEmpty(loggerId)) loggerId = module.Id;
                loggerId = $"modules.{loggerId.Replace(' ', '-')}".ToLower();

                Logger logger;
                lock (_lock)
                {
                    _loggers.TryGetValue(loggerId, out logger);
                    if (logger == null)
                    {
                        logger = LogManager.GetLogger(loggerId);
                        _loggers.Add(loggerId, logger);
                    }
                }

                var logEvent = new LogEventInfo();
                logEvent.LoggerName = loggerId;
                logEvent.Message = message;

                switch (logLevel)
                {
                    case MTConnectLogLevel.Fatal: logEvent.Level = LogLevel.Fatal; break;
                    case MTConnectLogLevel.Error: logEvent.Level = LogLevel.Error; break;
                    case MTConnectLogLevel.Warning: logEvent.Level = LogLevel.Warn; break;
                    case MTConnectLogLevel.Information: logEvent.Level = LogLevel.Info; break;
                    case MTConnectLogLevel.Debug: logEvent.Level = LogLevel.Debug; break;
                    case MTConnectLogLevel.Trace: logEvent.Level = LogLevel.Trace; break;
                }

                logger.Log(logEvent);
            }
        }

        private void ProcessorLoaded(object sender, IMTConnectAgentProcessor processor)
        {
            _applicationLogger.Info($"Processor Loaded : " + processor.Id);
        }

        private void ProcessorLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var processor = (IMTConnectAgentProcessor)sender;

                var loggerId = logId;
                if (string.IsNullOrEmpty(loggerId)) loggerId = processor.Id;
                loggerId = $"processors.{loggerId.Replace(' ', '-')}".ToLower();

                Logger logger;
                lock (_lock)
                {
                    _loggers.TryGetValue(loggerId, out logger);
                    if (logger == null)
                    {
                        logger = LogManager.GetLogger(loggerId);
                        _loggers.Add(loggerId, logger);
                    }
                }

                var logEvent = new LogEventInfo();
                logEvent.LoggerName = loggerId;
                logEvent.Message = message;

                switch (logLevel)
                {
                    case MTConnectLogLevel.Fatal: logEvent.Level = LogLevel.Fatal; break;
                    case MTConnectLogLevel.Error: logEvent.Level = LogLevel.Error; break;
                    case MTConnectLogLevel.Warning: logEvent.Level = LogLevel.Warn; break;
                    case MTConnectLogLevel.Information: logEvent.Level = LogLevel.Info; break;
                    case MTConnectLogLevel.Debug: logEvent.Level = LogLevel.Debug; break;
                    case MTConnectLogLevel.Trace: logEvent.Level = LogLevel.Trace; break;
                }

                logger.Log(logEvent);
            }
        }

        private void DevicesRequested(string deviceName)
        {
            _agentLogger.Debug($"MTConnectDevices Requested : " + deviceName);
        }

        private void DevicesSent(IDevicesResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"MTConnectDevices Sent : " + document.Header.CreationTime);
            }
        }

        private void StreamsRequested(string deviceName)
        {
            _agentLogger.Debug($"MTConnectStreams Requested : " + deviceName);
        }

        private void StreamsSent(object sender, EventArgs args)
        {
            _agentLogger.Log(_logLevel, "MTConnectStreams Sent");
        }

        private void AssetsRequested(IEnumerable<string> assetIds)
        {
            var ids = "";
            if (!assetIds.IsNullOrEmpty())
            {
                string.Join(";", assetIds.ToArray());
            }

            _agentLogger.Debug($"MTConnectAssets Requested : AssetIds = {ids}");
        }

        private void DeviceAssetsRequested(string deviceUuid)
        {
            _agentLogger.Debug($"MTConnectAssets Requested : DeviceUuid = {deviceUuid}");
        }


        private void AssetsSent(IAssetsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"MTConnectAssets Sent : {document.Header.CreationTime}");
            }
        }

        private void AssetBufferStarted(object sender, EventArgs args)
        {
            _agentLogger.Info($"Loading Assets from File Buffer...");
        }

        private void AssetBufferCompleted(object sender, AssetBufferLoadArgs args)
        {
            _agentLogger.Info($"{args.Count} Assets Loaded from File Buffer in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
        }


        private void ObservationAdded(object sender, IObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                foreach (var value in observation.Values)
                {
                    _agentLogger.Debug($"Observation Added Successfully : {observation.DeviceUuid} : {observation.DataItemId} : {value.Key} = {value.Value}");
                }
            }
        }

        private void ObservationBufferStarted(object sender, EventArgs args)
        {
            _agentLogger.Info($"Loading Observations from File Buffer...");
        }

        private void ObservationBufferCompleted(object sender, ObservationBufferLoadArgs args)
        {
            _agentLogger.Info($"{args.Count} Observations Loaded from File Buffer in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
        }

        private void ObservationBufferRetentionCompleted(object sender, ObservationBufferRetentionArgs args)
        {
            _agentLogger.Debug($"Observations File Buffer Retention : Removing ({args.From} - {args.To})");

            if (args.Count > 0)
            {
                _agentLogger.Debug($"Observations File Buffer Retention : {args.Count} Buffer Files Removed in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
            }
        }


        private void InvalidComponent(string deviceUuid, IComponent component, ValidationResult result)
        {
            _agentValidationLogger.Warn($"{deviceUuid} : ComponentId = {component.Id} : {result.Message}");
        }

        private void InvalidComposition(string deviceUuid, IComposition composition, ValidationResult result)
        {
            _agentValidationLogger.Warn($"{deviceUuid} : CompositionId = {composition.Id} : {result.Message}");
        }

        private void InvalidDataItem(string deviceUuid, IDataItem dataItem, ValidationResult result)
        {
            _agentValidationLogger.Warn($"{deviceUuid} : DataItemId = {dataItem.Id} : {result.Message}");
        }

        private void InvalidObservation(string deviceUuid, string dataItemKey, ValidationResult result)
        {
            _agentValidationLogger.Warn($"{deviceUuid} : DataItemKey = {dataItemKey} : {result.Message}");
        }

        private void InvalidAsset(IAsset asset, ValidationResult result)
        {
            _agentValidationLogger.Warn($"{result.Message}");
        }

        #endregion

    }
}
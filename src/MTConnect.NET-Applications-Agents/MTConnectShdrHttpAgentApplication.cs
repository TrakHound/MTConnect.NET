// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Http;
using MTConnect.Observations;
using MTConnect.Streams;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace MTConnect.Applications.Agents
{
    public class MTConnectShdrHttpAgentApplication : IMTConnectAgentApplication
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using HTTP to provide access to device information";

        private readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        private readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private readonly Logger _agentMetricLogger = LogManager.GetLogger("agent--metric-logger");
        private readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private readonly Logger _httpLogger = LogManager.GetLogger("http-logger");
        private readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");
        private readonly Logger _adapterShdrLogger = LogManager.GetLogger("adapter-shdr-logger");

        private readonly List<ShdrAdapterClient> _adapters = new List<ShdrAdapterClient>();
        private readonly List<DeviceConfigurationFileWatcher> _deviceConfigurationWatchers = new List<DeviceConfigurationFileWatcher>();

        private LogLevel _logLevel = LogLevel.Debug;
        private MTConnectAgent _mtconnectAgent;
        private MTConnectObservationFileBuffer _observationBuffer;
        private MTConnectAssetFileBuffer _assetBuffer;
        private MTConnectHttpServer _httpServer;
        private AgentConfigurationFileWatcher<ShdrHttpAgentConfiguration> _agentConfigurationWatcher;
        private System.Timers.Timer _metricsTimer;
        private bool _started = false;
        private int _port = 0;
        private bool _verboseLogging = true;


        public IMTConnectAgent Agent => _mtconnectAgent;


        /// <summary>
        /// Run Arguments [help|install|debug|run] [configuration_file] [http_port]
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            string command = "run";
            string configFile = null;
            int port = 0;

            string serviceName = DefaultServiceName;
            string serviceDisplayName = DefaultServiceDisplayName;
            string serviceDescription = DefaultServiceDescription;
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

                // Port
                if (args.Length > 2)
                {
                    if (int.TryParse(args[2], out var p))
                    {
                        port = p;
                        _applicationLogger.Info($"Agent HTTP Port = {port}");
                    }
                }
            }
            _port = port;

            // Read the Http Agent Configuation File
            var configuration = AgentConfiguration.Read<ShdrHttpAgentConfiguration>(configFile);
            if (configuration != null)
            {
                // Set Service Name
                if (!string.IsNullOrEmpty(configuration.ServiceName)) serviceDisplayName = configuration.ServiceName;

                // Set Service Auto Start
                serviceStart = configuration.ServiceAutoStart;
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
                    StartAgent(configuration, _verboseLogging, port);
                    while (true) Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

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
                    StartAgent(configuration, _verboseLogging, port);
                    while (true) Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

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


        public void StartAgent(string configurationPath, bool verboseLogging = false, int port = 0)
        {
            // Read the Configuration File
            var configuration = AgentConfiguration.Read<ShdrHttpAgentConfiguration>(configurationPath);

            // Start the Agent
            StartAgent(configuration, verboseLogging, port);
        }

        public void StartAgent(ShdrHttpAgentConfiguration configuration, bool verboseLogging = false, int port = 0)
        {
            if (!_started && configuration != null)
            {
                _adapters.Clear();
                _deviceConfigurationWatchers.Clear();
                var initializeDataItems = true;

                // Read Agent Information File
                var agentInformation = MTConnectAgentInformation.Read();
                if (agentInformation == null)
                {
                    agentInformation = new MTConnectAgentInformation();
                    agentInformation.Save();
                }

                // Create Observation File Buffer
                if (configuration.Durable)
                {
                    _observationBuffer = new MTConnectObservationFileBuffer(configuration);
                    _observationBuffer.UseCompression = true;
                    _observationBuffer.BufferLoadStarted += ObservationBufferStarted;
                    _observationBuffer.BufferLoadCompleted += ObservationBufferCompleted;
                    _observationBuffer.BufferRetentionCompleted += ObservationBufferRetentionCompleted;

                    // Create Asset File Buffer
                    _assetBuffer = new MTConnectAssetFileBuffer(configuration);
                    _assetBuffer.UseCompression = true;
                    _assetBuffer.BufferLoadStarted += AssetBufferStarted;
                    _assetBuffer.BufferLoadCompleted += AssetBufferCompleted;

                    // Read Buffer Observations
                    initializeDataItems = !_observationBuffer.Load();

                    // Read Buffer Assets
                    _assetBuffer.Load();
                }


                // Create MTConnectAgent
                _mtconnectAgent = new MTConnectAgent(configuration, null, _observationBuffer, _assetBuffer, agentInformation.Uuid, agentInformation.InstanceId, agentInformation.DeviceModelChangeTime, initializeDataItems);

                if (verboseLogging)
                {
                    _mtconnectAgent.DevicesRequestReceived += DevicesRequested;
                    _mtconnectAgent.DevicesResponseSent += DevicesSent;
                    _mtconnectAgent.StreamsRequestReceived += StreamsRequested;
                    _mtconnectAgent.StreamsResponseSent += StreamsSent;
                    _mtconnectAgent.AssetsRequestReceived += AssetsRequested;
                    _mtconnectAgent.DeviceAssetsRequestReceived += DeviceAssetsRequested;
                    _mtconnectAgent.AssetsResponseSent += AssetsSent;
                    _mtconnectAgent.ObservationAdded += ObservationAdded;
                    _mtconnectAgent.InvalidComponentAdded += InvalidComponent;
                    _mtconnectAgent.InvalidCompositionAdded += InvalidComposition;
                    _mtconnectAgent.InvalidDataItemAdded += InvalidDataItem;
                    _mtconnectAgent.InvalidObservationAdded += InvalidObservation;
                    _mtconnectAgent.InvalidAssetAdded += InvalidAsset;
                }

                // Read Device Configuration Files
                var devicesPath = configuration.Devices;
                if (string.IsNullOrEmpty(devicesPath) && !configuration.AllowShdrDevice) devicesPath = "devices";
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

                // Add Adapter Clients
                if (!configuration.Adapters.IsNullOrEmpty())
                {
                    if (!devices.IsNullOrEmpty())
                    {
                        // Device Specific Adapters (DeviceKey specified)
                        var deviceAdapters = configuration.Adapters.Where(o => o.DeviceKey != ShdrClientConfiguration.DeviceKeyWildcard);
                        if (!deviceAdapters.IsNullOrEmpty())
                        {
                            foreach (var adapter in deviceAdapters)
                            {
                                // Find Device matching DeviceKey
                                var device = devices?.FirstOrDefault(o => o.Uuid == adapter.DeviceKey || o.Name == adapter.DeviceKey);
                                if (device != null) AddAdapter(adapter, device, initializeDataItems);
                            }
                        }

                        // Wildcard Adapters (DeviceKey = '*')
                        var wildCardAdapters = configuration.Adapters.Where(o => o.DeviceKey == ShdrClientConfiguration.DeviceKeyWildcard);
                        if (!wildCardAdapters.IsNullOrEmpty())
                        {
                            foreach (var adapter in wildCardAdapters)
                            {
                                // Add Adapter for each Device (every device reads from the same adapter)
                                foreach (var device in devices) AddAdapter(adapter, device, initializeDataItems, device.Id);
                            }
                        }
                    }
                    else if (configuration.AllowShdrDevice) // Prevent accidental generic Adapter creation
                    {
                        foreach (var adapter in configuration.Adapters)
                        {
                            // Add a generic Adapter Client (no Device)
                            // Typically used if the Device Model is sent using SHDR
                            AddAdapter(adapter, null, initializeDataItems);
                        }
                    }
                }

                // Initialize Agent Current Observations/Conditions
                // This updates the MTConnectAgent's cache used to determine duplicate observations
                if (_observationBuffer != null)
                {
                    _mtconnectAgent.InitializeCurrentObservations(_observationBuffer.CurrentObservations.Values);
                    _mtconnectAgent.InitializeCurrentObservations(_observationBuffer.CurrentConditions.SelectMany(o => o.Value));
                }

                // Start Agent Metrics
                StartMetrics();

                // Intialize the Http Server
                _httpServer = new ShdrMTConnectHttpServer(configuration, _mtconnectAgent, null, port);

                // Setup Http Server Logging
                if (verboseLogging)
                {
                    _httpServer.ListenerStarted += HttpListenerStarted;
                    _httpServer.ListenerStopped += HttpListenerStopped;
                    _httpServer.ListenerException += HttpListenerException;
                    _httpServer.ClientConnected += HttpClientConnected;
                    _httpServer.ClientDisconnected += HttpClientDisconnected;
                    _httpServer.ClientException += HttpClientException;
                    _httpServer.ResponseSent += HttpResponseSent;
                }

                // Start the Http Server
                _httpServer.Start();


                if (configuration.MonitorConfigurationFiles)
                {
                    // Set the Agent Configuration File Watcher
                    if (_agentConfigurationWatcher != null) _agentConfigurationWatcher.Dispose();
                    _agentConfigurationWatcher = new AgentConfigurationFileWatcher<ShdrHttpAgentConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
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
                // Stop Adapter Clients
                if (!_adapters.IsNullOrEmpty())
                {
                    foreach (var adapter in _adapters) adapter.Stop();
                }

                // Stop Device Configuration FileWatchers
                if (!_deviceConfigurationWatchers.IsNullOrEmpty())
                {
                    foreach (var deviceConfigurationFileWatcher in _deviceConfigurationWatchers) deviceConfigurationFileWatcher.Dispose();
                }

                if (_httpServer != null) _httpServer.Stop();
                if (_mtconnectAgent != null) _mtconnectAgent.Dispose();
                if (_observationBuffer != null) _observationBuffer.Dispose();
                if (_assetBuffer != null) _assetBuffer.Dispose();
                if (_agentConfigurationWatcher != null) _agentConfigurationWatcher.Dispose();
                if (_metricsTimer != null) _metricsTimer.Dispose();

                Thread.Sleep(2000); // Delay 2 seconds to allow Http Server to stop

                _started = false;
            }
        }


        #region "Adapters"

        private void AddAdapter(ShdrAdapterConfiguration configuration, IDevice device, bool initializeDataItems = true, string idSuffix = null)
        {
            if (configuration != null)
            {
                var adapterComponent = new ShdrAdapterComponent(configuration, idSuffix);

                // Add Adapter Component to Agent Device
                _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                if (!adapterComponent.DataItems.IsNullOrEmpty())
                {
                    // Initialize Adapter URI Observation
                    var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                    if (adapterUriDataItem != null && initializeDataItems)
                    {
                        _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                    }
                }

                // Create new SHDR Adapter Client to read from SHDR stream
                var adapterClient = new ShdrAdapterClient(configuration, _mtconnectAgent, device, idSuffix);
                _adapters.Add(adapterClient);

                if (_verboseLogging)
                {
                    adapterClient.Connected += AdapterConnected;
                    adapterClient.Disconnected += AdapterDisconnected;
                    adapterClient.ConnectionError += AdapterConnectionError;
                    adapterClient.Listening += AdapterListening;
                    adapterClient.PingSent += AdapterPingSent;
                    adapterClient.PongReceived += AdapterPongReceived;
                    adapterClient.ProtocolReceived += AdapterProtocolReceived;
                }

                // Start the Adapter Client
                adapterClient.Start();
            }
        }

        #endregion

        #region "Agent Configuration"

        private void AgentConfigurationFileUpdated(object sender, ShdrHttpAgentConfiguration configuration)
        {
            if (configuration != null)
            {
                _applicationLogger.Info($"[Application] : Agent Configuration File Updated ({configuration.Path})");

                StopAgent();
                StartAgent(configuration, _verboseLogging, _port);
            }
        }

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

        #endregion

        #region "Help"

        private static void PrintHelp()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name.ToLower();

            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine($"     {name} [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file] [http_port]");
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
            Console.WriteLine("Arguments :");
            Console.WriteLine("--------------------");
            Console.WriteLine();
            Console.WriteLine(string.Format("{0,20}  |  {1,5}", "configuration_file", "Specifies the Agent Configuration file to load"));
            Console.WriteLine(string.Format("{0,20}     {1,5}", "", "Default : agent.config.json"));
            Console.WriteLine();
            Console.WriteLine(string.Format("{0,20}  |  {1,5}", "http_port", "Specifies the TCP Port to use for the HTTP Server"));
            Console.WriteLine(string.Format("{0,20}     {1,5}", "", "Note : This overrides what is read from the Configuration file"));
        }

        #endregion

        #region "Logging"

        private void DevicesRequested(string deviceName)
        {
            _agentLogger.Debug($"[Agent] : MTConnectDevices Requested : " + deviceName);
        }

        private void DevicesSent(IDevicesResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"[Agent] : MTConnectDevices Sent : " + document.Header.CreationTime);
            }
        }

        private void StreamsRequested(string deviceName)
        {
            _agentLogger.Debug($"[Agent] : MTConnectStreams Requested : " + deviceName);
        }

        private void StreamsSent(IStreamsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"[Agent] : MTConnectStreams Sent : " + document.Header.CreationTime);
            }
        }

        private void AssetsRequested(IEnumerable<string> assetIds)
        {
            var ids = "";
            if (!assetIds.IsNullOrEmpty())
            {
                string.Join(";", assetIds.ToArray());
            }

            _agentLogger.Debug($"[Agent] : MTConnectAssets Requested : AssetIds = " + ids);
        }

        private void DeviceAssetsRequested(string deviceUuid)
        {
            _agentLogger.Debug($"[Agent] : MTConnectAssets Requested : DeviceUuid = " + deviceUuid);
        }

        private void AssetsSent(IAssetsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"[Agent] : MTConnectAssets Sent : " + document.Header.CreationTime);
            }
        }

        private void AssetBufferStarted(object sender, EventArgs args)
        {
            _agentLogger.Info($"[Agent] : Loading Assets from File Buffer...");
        }

        private void AssetBufferCompleted(object sender, AssetBufferLoadArgs args)
        {
            _agentLogger.Info($"[Agent] : {args.Count} Assets Loaded from File Buffer in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
        }


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

        private void ObservationBufferStarted(object sender, EventArgs args)
        {
            _agentLogger.Info($"[Agent] : Loading Observations from File Buffer...");
        }

        private void ObservationBufferCompleted(object sender, ObservationBufferLoadArgs args)
        {
            _agentLogger.Info($"[Agent] : {args.Count} Observations Loaded from File Buffer in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
        }

        private void ObservationBufferRetentionCompleted(object sender, ObservationBufferRetentionArgs args)
        {
            _agentLogger.Debug($"[Agent] : Observations File Buffer Retention : Removing ({args.From} - {args.To})");

            if (args.Count > 0)
            {
                _agentLogger.Debug($"[Agent] : Observations File Buffer Retention : {args.Count} Buffer Files Removed in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
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


        private void AdapterConnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.ESTABLISHED);

            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterDisconnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.CLOSED);

            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterConnectionError(object sender, Exception exception)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + exception.Message);
        }

        private void AdapterListening(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;

            var dataItemId = DataItem.CreateId(adapterClient.Id, ConnectionStatusDataItem.NameId);
            _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, dataItemId, Observations.Events.Values.ConnectionStatus.LISTEN);

            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterPingSent(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Debug($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterPongReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Debug($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private void AdapterProtocolReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterShdrLogger.Trace($"[Adapter-SHDR] : ID = " + adapterClient.Id + " : " + message);
        }


        private void HttpListenerStarted(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listening at " + prefix + "..");
        }

        private void HttpListenerStopped(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listener Stopped for " + prefix);
        }

        private void HttpListenerException(object sender, Exception exception)
        {
            _httpLogger.Warn($"[Http Server] : Listener Exception : " + exception.Message);
        }

        private void HttpClientConnected(object sender, HttpListenerRequest request)
        {
            _httpLogger.Info($"[Http Server] : Http Client Connected : (" + request.HttpMethod + ") : " + request.LocalEndPoint + " : " + request.Url);
        }

        private void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            _httpLogger.Debug($"[Http Server] : Http Client Disconnected : " + remoteEndPoint);
        }

        private void HttpClientException(object sender, Exception exception)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Http Client Exception : " + exception.Message);
        }

        private void HttpResponseSent(object sender, MTConnectHttpResponse response)
        {
            _httpLogger.Info($"[Http Server] : Http Response Sent : {response.StatusCode} : {response.ContentType} : Agent Process Time {response.ResponseDuration}ms : Document Format Time {response.FormatDuration}ms : Total Response Time {response.ResponseDuration + response.FormatDuration}ms");

            // Format Messages
            if (!response.FormatMessages.IsNullOrEmpty())
            {
                foreach (var message in response.FormatMessages)
                {
                    _agentValidationLogger.Debug($"[Http Server] : Formatter Message : {message}");
                }
            }

            // Format Warnings
            if (!response.FormatWarnings.IsNullOrEmpty())
            {
                foreach (var warning in response.FormatWarnings)
                {
                    _agentValidationLogger.Warn($"[Http Server] : Formatter Warning : {warning}");
                }
            }

            // Format Errors
            if (!response.FormatErrors.IsNullOrEmpty())
            {
                foreach (var error in response.FormatErrors)
                {
                    _agentValidationLogger.Error($"[Http Server] : Formatter Error : {error}");
                }
            }
        }

        #endregion
    }
}
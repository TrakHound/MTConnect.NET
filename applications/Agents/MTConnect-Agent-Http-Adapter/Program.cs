// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Clients.Rest;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Http;
using MTConnect.Observations;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MTConnect.Applications
{
    public class Program
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP-Adapter";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Agent Adapter";
        private const string DefaultServiceDescription = "MTConnect Agent Adapter using HTTP to provide access to device information";

        private static readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        private static readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private static readonly Logger _agentMetricLogger = LogManager.GetLogger("agent-metric-logger");
        private static readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private static readonly Logger _httpLogger = LogManager.GetLogger("http-logger");

        private static LogLevel _logLevel = LogLevel.Debug;
        private static MTConnectAgent _mtconnectAgent;
        private static MTConnectObservationFileBuffer _observationBuffer;
        private static MTConnectAssetFileBuffer _assetBuffer;
        private static MTConnectHttpServer _httpServer;
        private static AgentConfigurationFileWatcher<AgentAdapterConfiguration> _agentConfigurationWatcher;
        private static readonly Dictionary<string, IObservationInput> _dataItems = new Dictionary<string, IObservationInput>();
        private static System.Timers.Timer _metricsTimer;
        private static bool _started = false;
        private static int _port = 0;
        private static bool _verboseLogging = true;


        /// <summary>
        /// Program Arguments [help|install|debug|run] [configuration_file] [http_port]
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            PrintHeader();

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
            var configuration = AgentConfiguration.Read<HttpShdrAgentConfiguration>(configFile);
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
                service = new Service(serviceName, serviceDisplayName, serviceDescription, serviceStart);
            }

            switch (command)
            {
                case "run":
                    _verboseLogging = false;
                    StartAgent(configuration, _verboseLogging, port);
                    while (true) System.Threading.Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

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
                    while (true) System.Threading.Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

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

        internal static void StartAgent(string configurationPath, bool verboseLogging = false, int port = 0)
        {
            // Read the Configuration File
            var configuration = AgentConfiguration.Read<AgentAdapterConfiguration>(configurationPath);

            // Start the Agent
            StartAgent(configuration, verboseLogging, port);
        }

        internal static void StartAgent(AgentAdapterConfiguration configuration, bool verboseLogging = false, int port = 0)
        {
            if (!_started && configuration != null)
            {
                //_adapters.Clear();
                //_deviceConfigurationWatchers.Clear();
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
                    _mtconnectAgent.AssetsResponseSent += AssetsSent;
                    _mtconnectAgent.ObservationAdded += ObservationAdded;
                    _mtconnectAgent.InvalidComponentAdded += InvalidComponent;
                    _mtconnectAgent.InvalidCompositionAdded += InvalidComposition;
                    _mtconnectAgent.InvalidDataItemAdded += InvalidDataItem;
                    _mtconnectAgent.InvalidObservationAdded += InvalidObservation;
                    _mtconnectAgent.InvalidAssetAdded += InvalidAsset;
                }

                //// Add Devices
                //var devices = DeviceConfiguration.FromFiles(configuration.Devices, DocumentFormat.XML);
                //if (!devices.IsNullOrEmpty())
                //{
                //    // Add Device(s) to Agent
                //    foreach (var device in devices)
                //    {
                //        _agentLogger.Info($"Device ({device.Name}) Read From File : {device.Path}");

                //        _mtconnectAgent.AddDevice(device, initializeDataItems);
                //    }

                //    if (configuration.MonitorConfigurationFiles)
                //    {
                //        // Set Device Configuration File Watcher
                //        var paths = devices.Select(o => o.Path).Distinct();
                //        foreach (var path in paths)
                //        {
                //            // Create a Device Configuration File Watcher
                //            var deviceConfigurationWatcher = new DeviceConfigurationFileWatcher(path, configuration.ConfigurationFileRestartInterval * 1000);
                //            deviceConfigurationWatcher.ConfigurationUpdated += DeviceConfigurationFileUpdated;
                //            deviceConfigurationWatcher.ErrorReceived += DeviceConfigurationFileError;
                //            _deviceConfigurationWatchers.Add(deviceConfigurationWatcher);
                //        }
                //    }
                //}
                //else
                //{
                //    _agentLogger.Warn($"No Devices Found : Reading from : {configuration.Devices}");
                //}

                // Add Http Adapter Client
                if (!configuration.Clients.IsNullOrEmpty())
                {
                    foreach (var clientConfiguration in configuration.Clients)
                    {
                        if (!string.IsNullOrEmpty(clientConfiguration.Address))
                        {
                            string baseUrl = null;
                            var deviceAddress = clientConfiguration.Address;
                            var devicePort = clientConfiguration.Port;

                            if (clientConfiguration.UseSSL) deviceAddress = deviceAddress.Replace("https://", "");
                            else deviceAddress = deviceAddress.Replace("http://", "");

                            // Create the MTConnect Agent Base URL
                            if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", AddPort(deviceAddress, port));
                            else baseUrl = string.Format("http://{0}", AddPort(deviceAddress, port));

                            var agentClient = new MTConnectClient(baseUrl, clientConfiguration.DeviceKey);
                            agentClient.Interval = clientConfiguration.Interval;
                            agentClient.Heartbeat = clientConfiguration.Heartbeat;

                            // Subscribe to the Event handlers to receive the MTConnect documents
                            agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(clientConfiguration.DeviceKey, doc);
                            agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(clientConfiguration.DeviceKey, doc);
                            agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(clientConfiguration.DeviceKey, doc);

                            agentClient.Start();
                        }
                    }


                    //var clientConfiguration = configuration.Clients.FirstOrDefault(o => o.ClientDeviceName == device);
                    //if (clientConfiguration != null && !string.IsNullOrEmpty(clientConfiguration.Address))
                    //{
                    //    string baseUrl = null;
                    //    var deviceAddress = clientConfiguration.Address;
                    //    var devicePort = clientConfiguration.Port;

                    //    if (clientConfiguration.UseSSL) deviceAddress = deviceAddress.Replace("https://", "");
                    //    else deviceAddress = deviceAddress.Replace("http://", "");

                    //    // Create the MTConnect Agent Base URL
                    //    if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", AddPort(deviceAddress, port));
                    //    else baseUrl = string.Format("http://{0}", AddPort(deviceAddress, port));

                    //    var agentClient = new MTConnectClient(baseUrl, clientConfiguration.ClientDeviceName);
                    //    agentClient.Interval = clientConfiguration.Interval;
                    //    agentClient.Heartbeat = clientConfiguration.Heartbeat;

                    //    // Subscribe to the Event handlers to receive the MTConnect documents
                    //    agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(device.Uuid, doc);
                    //    agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(device.Uuid, doc);
                    //    agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(device.Uuid, doc);

                    //    agentClient.Start();
                    //}
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
                    _agentConfigurationWatcher = new AgentConfigurationFileWatcher<AgentAdapterConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
                    _agentConfigurationWatcher.ConfigurationUpdated += AgentConfigurationFileUpdated;
                    _agentConfigurationWatcher.ErrorReceived += AgentConfigurationFileError;
                }

                _started = true;
            }
        }

        internal static void StopAgent()
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

                System.Threading.Thread.Sleep(2000); // Delay 2 seconds to allow Http Server to stop

                _started = false;
            }
        }


        #region "Processors"

        private static IEnumerable<IObservationInput> ProcessDataItems()
        {
            var observations = new List<IObservationInput>();

            var dataItems = _dataItems.Values;
            foreach (var dataItem in dataItems)
            {
                // Pass through all dataItems
                // NOTE: This would be where you could process the existing values
                observations.Add(dataItem);
            }

            return observations;
        }

        #endregion

        #region "Converters"

        private static IObservationInput ConvertObservation(IObservation observation)
        {
            var type = observation.Type.ToUnderscoreUpper();
            switch (type)
            {
                case ExecutionDataItem.TypeId: return ConvertExecution(observation);
            }

            return new ObservationInput(observation);
        }

        private static IObservationInput ConvertExecution(IObservation observation)
        {
            var convertedObservation = new ObservationInput(observation);

            // Get CDATA Value
            var cdata = convertedObservation.GetValue(ValueKeys.CDATA);

            // Change CDATA Value if "INTERRUPTED"
            // NOTE: This is just an example of how a value read from the Client Agent can be converted
            if (cdata == Execution.INTERRUPTED.ToString()) convertedObservation.AddValue(ValueKeys.CDATA, Execution.STOPPED);

            return convertedObservation;
        }

        #endregion

        #region "Client Event Handlers"

        private static void StreamsDocumentReceived(string deviceUuid, IStreamsResponseDocument document)
        {
            if (document != null && !document.Streams.IsNullOrEmpty())
            {
                foreach (var stream in document.Streams)
                {
                    var observations = stream.Observations;
                    if (!observations.IsNullOrEmpty())
                    {
                        foreach (var observation in observations)
                        {
                            // Convert the Observation (if needed)
                            var input = ConvertObservation(observation);

                            // Add new value to internal DataItems List
                            // (this is used similar to how most Adapters use a DataItems list)
                            _dataItems.Remove(input.DataItemKey);
                            _dataItems.Add(input.DataItemKey, input);
                        }

                        // Process the DataItems (converted) and create a new list of ObservationInputs
                        // (this is similar to how most Adapters process logic) 
                        var inputs = ProcessDataItems();
                        if (!inputs.IsNullOrEmpty())
                        {
                            // Add the Processed Observation to the Agent
                            _agent.AddObservations(deviceUuid, inputs);
                        }
                    }
                }
            }
        }

        private static void AssetsDocumentReceived(string deviceUuid, IAssetsResponseDocument document)
        {
            if (document != null && !document.Assets.IsNullOrEmpty())
            {
                foreach (var asset in document.Assets)
                {
                    _agent.AddAsset(deviceUuid, asset);
                }
            }
        }

        #endregion

        #region "Agent Configuration"

        private static void AgentConfigurationFileUpdated(object sender, AgentAdapterConfiguration configuration)
        {
            if (configuration != null)
            {
                _applicationLogger.Info($"[Application] : Agent Configuration File Updated ({configuration.Path})");

                StopAgent();
                StartAgent(configuration, _verboseLogging, _port);
            }
        }

        private static void AgentConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Agent Configuration File Error : {message}");
        }

        #endregion

        #region "Device Configuration"

        private static void DeviceConfigurationFileUpdated(object sender, DeviceConfiguration configuration)
        {
            if (configuration != null)
            {
                _applicationLogger.Info($"[Application] : Device Configuration File Updated ({configuration.Path})");

                // Add Device to MTConnect Agent
                _mtconnectAgent.AddDevice(configuration);
            }
        }

        private static void DeviceConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Device Configuration File Error : {message}");
        }

        #endregion

        #region "Metrics"

        private static void StartMetrics()
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

        #region "Console Output"

        private static void PrintHeader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            Console.WriteLine("Copyright 2022 TrakHound Inc., All Rights Reserved");
            Console.WriteLine("MTConnect HTTP Agent : Version " + version.ToString());
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the Apache Version 2.0 License (https://www.apache.org/licenses/LICENSE-2.0)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }

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

        private static void DevicesRequested(string deviceName)
        {
            _agentLogger.Debug($"[Agent] : MTConnectDevices Requested : " + deviceName);
        }

        private static void DevicesSent(IDevicesResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"[Agent] : MTConnectDevices Sent : " + document.Header.CreationTime);
            }
        }

        private static void StreamsRequested(string deviceName)
        {
            _agentLogger.Debug($"[Agent] : MTConnectStreams Requested : " + deviceName);
        }

        private static void StreamsSent(IStreamsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"[Agent] : MTConnectStreams Sent : " + document.Header.CreationTime);
            }
        }

        private static void AssetsRequested(string deviceName)
        {
            _agentLogger.Debug($"[Agent] : MTConnectAssets Requested : " + deviceName);
        }

        private static void AssetsSent(IAssetsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Log(_logLevel, $"[Agent] : MTConnectAssets Sent : " + document.Header.CreationTime);
            }
        }

        private static void AssetBufferStarted(object sender, EventArgs args)
        {
            _agentLogger.Info($"[Agent] : Loading Assets from File Buffer...");
        }

        private static void AssetBufferCompleted(object sender, AssetBufferLoadArgs args)
        {
            _agentLogger.Info($"[Agent] : {args.Count} Assets Loaded from File Buffer in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
        }


        private static void ObservationAdded(object sender, IObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                foreach (var value in observation.Values)
                {
                    _agentLogger.Debug($"[Agent] : Observation Added Successfully : {observation.DeviceUuid} : {observation.DataItemId} : {value.Key} = {value.Value}");
                }
            }
        }

        private static void ObservationBufferStarted(object sender, EventArgs args)
        {
            _agentLogger.Info($"[Agent] : Loading Observations from File Buffer...");
        }

        private static void ObservationBufferCompleted(object sender, ObservationBufferLoadArgs args)
        {
            _agentLogger.Info($"[Agent] : {args.Count} Observations Loaded from File Buffer in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
        }

        private static void ObservationBufferRetentionCompleted(object sender, ObservationBufferRetentionArgs args)
        {
            _agentLogger.Debug($"[Agent] : Observations File Buffer Retention : Removing ({args.From} - {args.To})");

            if (args.Count > 0)
            {
                _agentLogger.Debug($"[Agent] : Observations File Buffer Retention : {args.Count} Buffer Files Removed in ({TimeSpan.FromMilliseconds(args.Duration).TotalSeconds}s)");
            }
        }


        private static void InvalidComponent(string deviceUuid, IComponent component, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : ComponentId = {component.Id} : {result.Message}");
        }

        private static void InvalidComposition(string deviceUuid, IComposition composition, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : CompositionId = {composition.Id} : {result.Message}");
        }

        private static void InvalidDataItem(string deviceUuid, IDataItem dataItem, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : DataItemId = {dataItem.Id} : {result.Message}");
        }

        private static void InvalidObservation(string deviceUuid, string dataItemKey, ValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {deviceUuid} : DataItemKey = {dataItemKey} : {result.Message}");
        }

        private static void InvalidAsset(IAsset asset, AssetValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {result.Message}");
        }


        private static void HttpListenerStarted(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listening at " + prefix + "..");
        }

        private static void HttpListenerStopped(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listener Stopped for " + prefix);
        }

        private static void HttpListenerException(object sender, Exception exception)
        {
            _httpLogger.Warn($"[Http Server] : Listener Exception : " + exception.Message);
        }

        private static void HttpClientConnected(object sender, HttpListenerRequest request)
        {
            _httpLogger.Info($"[Http Server] : Http Client Connected : (" + request.HttpMethod + ") : " + request.LocalEndPoint + " : " + request.Url);
        }

        private static void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            _httpLogger.Debug($"[Http Server] : Http Client Disconnected : " + remoteEndPoint);
        }

        private static void HttpClientException(object sender, Exception exception)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Http Client Exception : " + exception.Message);
        }

        private static void HttpResponseSent(object sender, MTConnectHttpResponse response)
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


        private static string AddPort(string url, int port)
        {
            if (!string.IsNullOrEmpty(url) && port > 0)
            {
                if (url.Contains("/"))
                {
                    var p = url.Split('/');
                    if (p.Length > 1)
                    {
                        p[0] = $"{p[0]}:{port}";
                    }

                    return string.Join("/", p);
                }

                return $"{url}:{port}";
            }

            return url;
        }
    }
}

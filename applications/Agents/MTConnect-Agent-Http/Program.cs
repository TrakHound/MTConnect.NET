// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
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

namespace MTConnect.Applications
{
    public class Program
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Agent";
        private const string DefaultServiceDescription = "MTConnect Agent using HTTP to provide access to device information";

        private static readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        private static readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private static readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private static readonly Logger _httpLogger = LogManager.GetLogger("http-logger");
        private static readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");
        private static readonly Logger _adapterShdrLogger = LogManager.GetLogger("adapter-shdr-logger");

        private static readonly List<ShdrAdapterClient> _adapters = new List<ShdrAdapterClient>();

        private static LogLevel _logLevel = LogLevel.Info;
        private static MTConnectAgent _agent;
        private static MTConnectHttpServer _server;
        private static System.Timers.Timer _metricsTimer;


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

            // Read the Agent Configuation File
            var configuration = MTConnectAgentConfiguration.Read(configFile);
            if (configuration != null)
            {
                // Set Service Name
                if (!string.IsNullOrEmpty(configuration.ServiceName)) serviceName = configuration.ServiceName;

                // Set Service Auto Start
                serviceStart = configuration.ServiceAutoStart;
            }

            // Declare a new Service (to use Service commands)
            var service = new Service(serviceName, serviceDisplayName, serviceDescription, serviceStart);

            switch (command)
            {
                case "run":
                    StartAgent(configuration, false, port);
                    while (true) System.Threading.Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

                case "run-service":
                    ServiceBase.Run(service);
                    break;

                case "debug":
                    StartAgent(configuration, true, port);
                    while (true) System.Threading.Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

                case "install":
                    service.Stop();
                    service.Remove();
                    service.Install(configFile);
                    break;

                case "install-start":
                    service.Stop();
                    service.Remove();
                    service.Install(configFile);
                    service.Start();
                    break;

                case "remove":
                    service.Stop();
                    service.Remove();
                    break;

                case "start":
                    service.Start();
                    break;

                case "stop":
                    service.Stop();
                    break;

                case "help": 
                    PrintHelp();
                    break;
            }
        }


        internal static void StartAgent(string configurationPath, bool verboseLogging = false, int port = 0)
        {
            // Read the Configuration File
            var configuration = MTConnectAgentConfiguration.Read(configurationPath);

            // Start the Agent
            StartAgent(configuration, verboseLogging, port);
        }

        internal static void StartAgent(MTConnectAgentConfiguration configuration, bool verboseLogging = false, int port = 0)
        {
            if (configuration != null)
            {
                // Create MTConnectAgent
                _agent = new MTConnectAgent(configuration);

                if (verboseLogging)
                {
                    _agent.DevicesRequestReceived += DevicesRequested;
                    _agent.DevicesResponseSent += DevicesSent;
                    _agent.StreamsRequestReceived += StreamsRequested;
                    _agent.StreamsResponseSent += StreamsSent;
                    _agent.AssetsRequestReceived += AssetsRequested;
                    _agent.AssetsResponseSent += AssetsSent;
                    _agent.ObservationAdded += ObservationAdded;

                    _agent.InvalidDataItemAdded += InvalidDataItem;
                }

                // Add Devices
                var devices = Device.FromFile(configuration.Devices, DocumentFormat.XML);
                if (!devices.IsNullOrEmpty())
                {
                    // Add Device(s) to Agent
                    foreach (var device in devices)
                    {
                        _agentLogger.Log(_logLevel, $"Device Read From File : {device.Name}");

                        _agent.AddDevice(device);
                    }

                    // Add Adapter Clients
                    if (!configuration.Adapters.IsNullOrEmpty())
                    {
                        foreach (var adapterConfiguration in configuration.Adapters)
                        {
                            // Get the Device matching the "Device" configured in the AdapterConfiguration
                            var device = devices.FirstOrDefault(o => o.Name == adapterConfiguration.Device);
                            if (device != null)
                            {
                                var adapterId = $"_{StringFunctions.RandomString(10)}";

                                // Add Adapter Component to Agent Device
                                _agent.AddAdapterComponent(adapterId, adapterConfiguration);

                                // Create new SHDR Adapter Client to read from SHDR stream
                                var adapterClient = new ShdrAdapterClient(adapterId, adapterConfiguration, _agent, device);
                                _adapters.Add(adapterClient);

                                if (verboseLogging)
                                {
                                    adapterClient.AdapterConnected += AdapterConnected;
                                    adapterClient.AdapterDisconnected += AdapterDisconnected;
                                    adapterClient.AdapterConnectionError += AdapterConnectionError;
                                    adapterClient.PingSent += AdapterPingSent;
                                    adapterClient.PongReceived += AdapterPongReceived;
                                    adapterClient.ProtocolReceived += AdapterProtocolReceived;
                                }

                                // Start the Adapter Client
                                adapterClient.Start();
                            }
                        }
                    }
                }
                else
                {
                    _agentLogger.Warn($"No Devices Found : Reading from : {configuration.Devices}");
                }

                // Start Agent Metrics
                StartMetrics();

                // Intialize the Http Server
                _server = new ShdrMTConnectHttpServer(_agent, null, port);

                // Setup Http Server Logging
                if (verboseLogging)
                {
                    _server.ListenerStarted += HttpListenerStarted;
                    _server.ListenerStopped += HttpListenerStopped;
                    _server.ListenerException += HttpListenerException;
                    _server.ClientConnected += HttpClientConnected;
                    _server.ClientDisconnected += HttpClientDisconnected;
                    _server.ClientException += HttpClientException;
                    _server.ResponseSent += HttpResponseSent;
                }

                // Start the Http Server
                _server.Start();
            }
        }

        internal static void StopAgent()
        {
            // Stop Adapter Clients
            if (!_adapters.IsNullOrEmpty())
            {
                foreach (var adapter in _adapters) adapter.Stop();
            }

            if (_server != null) _server.Stop();
            if (_agent != null) _agent.Dispose();
            if (_metricsTimer != null) _metricsTimer.Dispose();
        }


        #region "Metrics"

        private static void StartMetrics()
        {
            int observationLastCount = 0;
            int observationDelta = 0;
            int assetLastCount = 0;
            int assetDelta = 0;
            var updateInterval = _agent.Metrics.UpdateInterval.TotalSeconds;
            var windowInterval = _agent.Metrics.WindowInterval.TotalMinutes;

            _metricsTimer = new System.Timers.Timer();
            _metricsTimer.Interval = updateInterval * 1000;
            _metricsTimer.Elapsed += (s, e) =>
            {
                // Observations
                var observationCount = _agent.Metrics.GetObservationCount();
                var observationAverage = _agent.Metrics.ObservationAverage;
                observationDelta = observationCount - observationLastCount;

                _agentLogger.Debug("[Agent] : Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                _agentLogger.Debug("[Agent] : Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                // Assets
                var assetCount = _agent.Metrics.GetAssetCount();
                var assetAverage = _agent.Metrics.AssetAverage;
                assetDelta = assetCount - assetLastCount;

                _agentLogger.Debug("[Agent] : Assets - Delta for last " + updateInterval + " seconds: " + assetDelta);
                _agentLogger.Debug("[Agent] : Assets - Average for last " + windowInterval + " minutes: " + Math.Round(assetAverage, 5));

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

        private static void InvalidDataItem(IDataItem dataItem, DataItemValidationResult result)
        {
            _agentValidationLogger.Warn($"[Agent-Validation] : {result.Message}");
        }


        private static void AdapterConnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _agent.UpdateAdapterConnectionStatus(adapterClient.Id, Observations.Events.Values.ConnectionStatus.ESTABLISHED);
            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterDisconnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _agent.UpdateAdapterConnectionStatus(adapterClient.Id, Observations.Events.Values.ConnectionStatus.CLOSED);
            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterConnectionError(object sender, Exception exception)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Log(_logLevel, $"[Adapter] : ID = " + adapterClient.Id + " : " + exception.Message);
        }

        private static void AdapterPingSent(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Debug($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterPongReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Debug($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterProtocolReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterShdrLogger.Trace($"[Adapter-SHDR] : ID = " + adapterClient.Id + " : " + message);
        }


        private static void HttpListenerStarted(object sender, string prefix)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Listening at " + prefix + "..");
        }

        private static void HttpListenerStopped(object sender, string prefix)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Listener Stopped for " + prefix);
        }

        private static void HttpListenerException(object sender, Exception exception)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Listener Exception : " + exception.Message);
        }

        private static void HttpClientConnected(object sender, HttpListenerRequest request)
        {
            _httpLogger.Debug($"[Http Server] : Client Connected : " + request.LocalEndPoint + " : " + request.Url);
        }

        private static void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            _httpLogger.Debug($"[Http Server] : Client Disconnected : " + remoteEndPoint);
        }

        private static void HttpClientException(object sender, Exception exception)
        {
            _httpLogger.Log(_logLevel, $"[Http Server] : Client Exception : " + exception.Message);
        }

        private static void HttpResponseSent(object sender, MTConnectHttpResponse response)
        {
            _httpLogger.Debug($"[Http Server] : Response Sent : {response.StatusCode} : {response.ContentType} : Agent Process Time {response.ResponseDuration}ms : Document Format Time {response.FormatDuration}ms : Total Response Time {response.ResponseDuration + response.FormatDuration}ms");
        }

        #endregion

    }
}

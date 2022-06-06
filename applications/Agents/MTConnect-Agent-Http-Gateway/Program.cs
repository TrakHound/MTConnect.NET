// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Applications.Configuration;
using MTConnect.Assets;
using MTConnect.Clients.Rest;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Http;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.ServiceProcess;

namespace MTConnect.Applications
{
    public class Program
    {
        private const string DefaultServiceName = "MTConnect-Agent-HTTP-Gateway";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Gateway Agent";
        private const string DefaultServiceDescription = "MTConnect Gateway Agent using HTTP to provide access to device information";

        private static readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        private static readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private static readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private static readonly Logger _httpLogger = LogManager.GetLogger("http-logger");

        private static readonly List<MTConnectClient> _clients = new List<MTConnectClient>();

        private static LogLevel _logLevel = LogLevel.Info;
        private static MTConnectAgent _agent;
        private static MTConnectHttpServer _server;
        private static AgentConfigurationFileWatcher<MTConnectAgentGatewayConfiguration> _configurationWatcher;
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

            string command = "debug";
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

            // Read the Agent Configuation File
            var configuration = MTConnectAgentGatewayConfiguration.Read(configFile);
            if (configuration != null)
            {
                // Set Service Name
                if (!string.IsNullOrEmpty(configuration.ServiceName)) serviceDisplayName = configuration.ServiceName;

                // Set Service Auto Start
                serviceStart = configuration.ServiceAutoStart;
            }

            // Declare a new Service (to use Service commands)
            var service = new Service(serviceName, serviceDisplayName, serviceDescription, serviceStart);

            switch (command)
            {
                case "run":
                    _verboseLogging = false;
                    StartAgent(configuration, _verboseLogging, port);
                    while (true) System.Threading.Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

                case "run-service":
                    ServiceBase.Run(service);
                    break;

                case "debug":
                    _verboseLogging = true;
                    StartAgent(configuration, _verboseLogging, port);
                    while (true) System.Threading.Thread.Sleep(100); // Block (exit console by 'Ctrl + C')

                case "install":
                    service.StopService();
                    service.RemoveService();
                    service.InstallService(configFile);
                    break;

                case "install-start":
                    service.StopService();
                    service.RemoveService();
                    service.InstallService(configFile);
                    service.StartService();
                    break;

                case "remove":
                    service.StopService();
                    service.RemoveService();
                    break;

                case "start":
                    service.StartService();
                    break;

                case "stop":
                    service.StopService();
                    break;

                case "help": 
                    PrintHelp();
                    break;
            }
        }


        internal static void StartAgent(string configurationPath, bool verboseLogging = false, int port = 0)
        {
            // Read the Configuration File
            var configuration = MTConnectAgentGatewayConfiguration.Read(configurationPath);

            // Start the Agent
            StartAgent(configuration, verboseLogging, port);
        }

        internal static void StartAgent(MTConnectAgentGatewayConfiguration configuration, bool verboseLogging = false, int port = 0)
        {
            if (!_started && configuration != null)
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

                // Add Agent Clients
                if (!configuration.Clients.IsNullOrEmpty())
                {
                    foreach (var clientConfiguration in configuration.Clients)
                    {
                        if (!string.IsNullOrEmpty(clientConfiguration.Address))
                        {
                            string baseUrl = null;
                            var clientAddress = clientConfiguration.Address;
                            var clientPort = clientConfiguration.Port;

                            if (clientConfiguration.UseSSL) clientAddress = clientAddress.Replace("https://", "");
                            else clientAddress = clientAddress.Replace("http://", "");

                            // Create the MTConnect Agent Base URL
                            if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", Url.AddPort(clientAddress, clientPort));
                            else baseUrl = string.Format("http://{0}", Url.AddPort(clientAddress, clientPort));

                            var agentClient = new MTConnectClient(baseUrl, clientConfiguration.DeviceName);
                            agentClient.Interval = clientConfiguration.Interval;
                            agentClient.Heartbeat = clientConfiguration.Heartbeat;
                            _clients.Add(agentClient);

                            // Subscribe to the Event handlers to receive the MTConnect documents
                            agentClient.OnProbeReceived += (s, doc) => DevicesDocumentReceived(doc);
                            agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(doc);
                            agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(doc);
                            agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(doc);

                            agentClient.Start();
                        }
                    }
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


                // Set the Configuration File Watcher
                if (_configurationWatcher != null) _configurationWatcher.Dispose();
                _configurationWatcher = new AgentConfigurationFileWatcher<MTConnectAgentGatewayConfiguration>(configuration.Path);
                _configurationWatcher.ConfigurationUpdated += ConfigurationFileUpdated;
                _configurationWatcher.ErrorReceived += ConfigurationFileError;

                _started = true;
            }
        }

        internal static void StopAgent()
        {
            if (_started)
            {
                // Stop MTConnect Clients
                if (!_clients.IsNullOrEmpty())
                {
                    foreach (var client in _clients) client.Stop();
                }

                if (_server != null) _server.Stop();
                if (_agent != null) _agent.Dispose();
                if (_configurationWatcher != null) _configurationWatcher.Dispose();
                if (_metricsTimer != null) _metricsTimer.Dispose();

                System.Threading.Thread.Sleep(2000); // Delay 2 seconds to allow Http Server to stop

                _started = false;
            }
        }


        #region "Configuration"

        private static void ConfigurationFileUpdated(object sender, MTConnectAgentGatewayConfiguration configuration)
        {
            if (configuration != null)
            {
                _applicationLogger.Info("[Application] : Configuration File Updated");

                StopAgent();
                StartAgent(configuration, _verboseLogging, _port);
            }
        }

        private static void ConfigurationFileError(object sender, string message)
        {
            _applicationLogger.Error($"[Application] : Configuration File Error : {message}");
        }

        #endregion

        #region "Client Event Handlers"

        private static void DevicesDocumentReceived(IDevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                foreach (var device in document.Devices)
                {
                    _agent.AddDevice(device);
                }
            }
        }

        private static void StreamsDocumentReceived(IStreamsResponseDocument document)
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
                            var input = new ObservationInput();
                            input.DeviceKey = stream.Uuid;
                            input.DataItemKey = observation.DataItemId;
                            input.Timestamp = observation.Timestamp.ToUnixTime();
                            input.Values = observation.Values;

                            _agent.AddObservation(stream.Uuid, input);
                        }
                    }
                }
            }
        }

        private static void AssetsDocumentReceived(IAssetsResponseDocument document)
        {
            if (document != null && !document.Assets.IsNullOrEmpty())
            {
                foreach (var asset in document.Assets)
                {
                    _agent.AddAsset(asset.DeviceUuid, asset);
                }
            }
        }

        #endregion

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
            Console.WriteLine("MTConnect HTTP Gateway Agent : Version " + version.ToString());
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
            Console.WriteLine(string.Format("{0,20}  |  {1,5}", "configuration_file", "Specified the Agent Configuration file to load"));
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

            // Format Messages
            if (!response.FormatMessages.IsNullOrEmpty())
            {
                foreach (var message in response.FormatMessages)
                {
                    _agentValidationLogger.Debug($"[Http Server] : Formatter Message : {message}");
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

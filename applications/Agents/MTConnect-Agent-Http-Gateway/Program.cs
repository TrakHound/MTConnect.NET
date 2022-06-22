// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Assets;
using MTConnect.Clients.Rest;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Http;
using MTConnect.Observations;
using MTConnect.Observations.Input;
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
        private const string DefaultServiceName = "MTConnect-Agent-HTTP-Gateway";
        private const string DefaultServiceDisplayName = "MTConnect HTTP Gateway Agent";
        private const string DefaultServiceDescription = "MTConnect Gateway Agent using HTTP to provide access to device information";

        private static readonly Logger _applicationLogger = LogManager.GetLogger("application-logger");
        private static readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private static readonly Logger _clientLogger = LogManager.GetLogger("client-logger");
        private static readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private static readonly Logger _httpLogger = LogManager.GetLogger("http-logger");

        private static readonly List<MTConnectClient> _clients = new List<MTConnectClient>();

        private static LogLevel _logLevel = LogLevel.Info;
        private static MTConnectAgent _mtconnectAgent;
        private static MTConnectHttpServer _httpServer;
        private static AgentConfigurationFileWatcher<AgentGatewayConfiguration> _configurationWatcher;
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

            // Read the Agent Configuation File
            var configuration = AgentConfiguration.Read<AgentGatewayConfiguration>(configFile);
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
            var configuration = AgentConfiguration.Read<AgentGatewayConfiguration>(configurationPath);

            // Start the Agent
            StartAgent(configuration, verboseLogging, port);
        }

        internal static void StartAgent(AgentGatewayConfiguration configuration, bool verboseLogging = false, int port = 0)
        {
            if (!_started && configuration != null)
            {
                // Create MTConnectAgent
                _mtconnectAgent = new MTConnectAgent(configuration);

                if (verboseLogging)
                {
                    _mtconnectAgent.DevicesRequestReceived += DevicesRequested;
                    _mtconnectAgent.DevicesResponseSent += DevicesSent;
                    _mtconnectAgent.StreamsRequestReceived += StreamsRequested;
                    _mtconnectAgent.StreamsResponseSent += StreamsSent;
                    _mtconnectAgent.AssetsRequestReceived += AssetsRequested;
                    _mtconnectAgent.AssetsResponseSent += AssetsSent;
                    _mtconnectAgent.ObservationAdded += ObservationAdded;

                    _mtconnectAgent.InvalidDataItemAdded += InvalidDataItem;
                }

                // Add Agent Clients
                if (!configuration.Clients.IsNullOrEmpty())
                {
                    foreach (var clientConfiguration in configuration.Clients)
                    {
                        if (!string.IsNullOrEmpty(clientConfiguration.Address))
                        {
                            //string baseUrl = null;
                            //var clientAddress = clientConfiguration.Address;
                            //var clientPort = clientConfiguration.Port;

                            //if (clientConfiguration.UseSSL) clientAddress = clientAddress.Replace("https://", "");
                            //else clientAddress = clientAddress.Replace("http://", "");

                            //// Create the MTConnect Agent Base URL
                            //if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", Url.AddPort(clientAddress, clientPort));
                            //else baseUrl = string.Format("http://{0}", Url.AddPort(clientAddress, clientPort));

                            var baseUri = HttpClientConfiguration.CreateBaseUri(clientConfiguration);

                            var adapterComponent = new HttpAdapterComponent(clientConfiguration);

                            // Add Adapter Component to Agent Device
                            _mtconnectAgent.Agent.AddAdapterComponent(adapterComponent);

                            if (!adapterComponent.DataItems.IsNullOrEmpty())
                            {
                                //// Initialize Connection Status Observation
                                //var connectionStatusDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == ConnectionStatusDataItem.TypeId);
                                //if (connectionStatusDataItem != null)
                                //{
                                //    _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, connectionStatusDataItem.Id, Observations.Events.Values.ConnectionStatus.LISTEN);
                                //}

                                // Initialize Adapter URI Observation
                                var adapterUriDataItem = adapterComponent.DataItems.FirstOrDefault(o => o.Type == AdapterUriDataItem.TypeId);
                                if (adapterUriDataItem != null)
                                {
                                    _mtconnectAgent.AddObservation(_mtconnectAgent.Uuid, adapterUriDataItem.Id, adapterComponent.Uri);
                                }
                            }


                            var agentClient = new MTConnectClient(baseUri, clientConfiguration.DeviceKey);
                            agentClient.Interval = clientConfiguration.Interval;
                            agentClient.Heartbeat = clientConfiguration.Heartbeat;
                            _clients.Add(agentClient);

                            // Subscribe to the Event handlers to receive the MTConnect documents
                            agentClient.OnClientStarted += (s, e) => AgentClientStarted();
                            agentClient.OnClientStopped += (s, e) => AgentClientStopped();
                            agentClient.OnStreamStarted += (s, query) => AgentClientStreamStarted(query);
                            agentClient.OnStreamStopped += (s, e) => AgentClientStreamStopped();
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


                // Set the Configuration File Watcher
                if (_configurationWatcher != null) _configurationWatcher.Dispose();
                _configurationWatcher = new AgentConfigurationFileWatcher<AgentGatewayConfiguration>(configuration.Path);
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

                if (_httpServer != null) _httpServer.Stop();
                if (_mtconnectAgent != null) _mtconnectAgent.Dispose();
                if (_configurationWatcher != null) _configurationWatcher.Dispose();
                if (_metricsTimer != null) _metricsTimer.Dispose();

                System.Threading.Thread.Sleep(2000); // Delay 2 seconds to allow Http Server to stop

                _started = false;
            }
        }


        #region "Configuration"

        private static void ConfigurationFileUpdated(object sender, AgentGatewayConfiguration configuration)
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

        private static void AgentClientStarted()
        {
            _clientLogger.Info($"[Client] : Client Started");
        }

        private static void AgentClientStopped()
        {
            _clientLogger.Info($"[Client] : Client Stopped");
        }

        private static void AgentClientStreamStarted(string query)
        {
            _clientLogger.Info($"[Client] : Client Stream Started : {query}");
        }

        private static void AgentClientStreamStopped()
        {
            _clientLogger.Info($"[Client] : Client Stream Stopped");
        }

        private static void DevicesDocumentReceived(IDevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[Client] : MTConnectDevices Received : " + document.Header.CreationTime);

                foreach (var device in document.Devices)
                {
                    _mtconnectAgent.AddDevice(device);
                }
            }
        }

        private static void StreamsDocumentReceived(IStreamsResponseDocument document)
        {
            if (document != null && !document.Streams.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[Client] : MTConnectStreams Received : " + document.Header.CreationTime);

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

                            _mtconnectAgent.AddObservation(stream.Uuid, input);
                        }
                    }
                }
            }
        }

        private static void AssetsDocumentReceived(IAssetsResponseDocument document)
        {
            if (document != null && !document.Assets.IsNullOrEmpty())
            {
                _clientLogger.Debug($"[Client] : MTConnectAssets Received : " + document.Header.CreationTime);

                foreach (var asset in document.Assets)
                {
                    _mtconnectAgent.AddAsset(asset.DeviceUuid, asset);
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

                _agentLogger.Debug("[Agent] : Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                _agentLogger.Debug("[Agent] : Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                // Assets
                var assetCount = _mtconnectAgent.Metrics.GetAssetCount();
                var assetAverage = _mtconnectAgent.Metrics.AssetAverage;
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

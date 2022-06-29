// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Assets;
using MTConnect.Clients.Rest;
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
        private static readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private static readonly Logger _agentMetricLogger = LogManager.GetLogger("agent-metric-logger");
        private static readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private static readonly Logger _httpLogger = LogManager.GetLogger("http-logger");
        private static MTConnectAgent _agent;
        private static readonly Dictionary<string, IObservationInput> _dataItems = new Dictionary<string, IObservationInput>();


        /// <summary>
        /// Program Arguments [help|install|debug|run] [configuration_file]
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            string command = "run";
            string configFile = null;

            // Read Command Line Arguments
            if (!args.IsNullOrEmpty())
            {
                command = args[0];

                // Configuration File Path
                if (args.Length > 1) configFile = args[1];
            }

            PrintHeader();

            // Read the Agent Configuation File
            var configuration = AgentAdapterConfiguration.Read(configFile);

            switch (command)
            {
                case "run": Init(configuration); break;

                case "debug": Init(configuration, true); break;
            }

            Console.ReadLine();
        }

        private static void Init(AgentAdapterConfiguration configuration, bool verboseLogging = false)
        {
            if (configuration != null)
            {
                // Create MTConnectAgent
                _agent = new MTConnectAgent(configuration);
                _agent.MTConnectVersion = MTConnectVersions.Max;

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
                var devices = DeviceConfiguration.FromFile(configuration.Devices, DocumentFormat.XML);
                if (!devices.IsNullOrEmpty())
                {
                    // Add Device(s) to Agent
                    foreach (var device in devices)
                    {
                        _agentLogger.Info($"Device Read From File : {device.Name}");

                        _agent.AddDevice(device);

                        // Add Adapter Client
                        if (!configuration.Clients.IsNullOrEmpty())
                        {
                            var clientConfiguration = configuration.Clients.FirstOrDefault(o => o.ClientDeviceName == device.Name);
                            if (clientConfiguration != null && !string.IsNullOrEmpty(clientConfiguration.Address))
                            {
                                string baseUrl = null;
                                var address = clientConfiguration.Address;
                                var port = clientConfiguration.Port;

                                if (clientConfiguration.UseSSL) address = address.Replace("https://", "");
                                else address = address.Replace("http://", "");

                                // Create the MTConnect Agent Base URL
                                if (clientConfiguration.UseSSL) baseUrl = string.Format("https://{0}", AddPort(address, port));
                                else baseUrl = string.Format("http://{0}", AddPort(address, port));

                                var agentClient = new MTConnectClient(baseUrl, clientConfiguration.ClientDeviceName);
                                agentClient.Interval = clientConfiguration.Interval;
                                agentClient.Heartbeat = clientConfiguration.Heartbeat;

                                // Subscribe to the Event handlers to receive the MTConnect documents
                                agentClient.OnCurrentReceived += (s, doc) => StreamsDocumentReceived(device.Uuid, doc);
                                agentClient.OnSampleReceived += (s, doc) => StreamsDocumentReceived(device.Uuid, doc);
                                agentClient.OnAssetsReceived += (s, doc) => AssetsDocumentReceived(device.Uuid, doc);

                                agentClient.Start();
                            }
                        }
                    }
                }

                // Start Agent Metrics
                StartMetrics();

                // Start the Http Server
                var server = new ShdrMTConnectHttpServer(configuration, _agent);

                if (verboseLogging)
                {
                    server.ListenerStarted += HttpListenerStarted;
                    server.ListenerStopped += HttpListenerStopped;
                    server.ListenerException += HttpListenerException;
                    server.ClientConnected += HttpClientConnected;
                    server.ClientDisconnected += HttpClientDisconnected;
                    server.ClientException += HttpClientException;
                    server.ResponseSent += HttpResponseSent;
                }

                server.Start();
            }
        }


        private static void StartMetrics()
        {
            int observationLastCount = 0;
            int observationDelta = 0;
            int assetLastCount = 0;
            int assetDelta = 0;
            var updateInterval = _agent.Metrics.UpdateInterval.TotalSeconds;
            var windowInterval = _agent.Metrics.WindowInterval.TotalMinutes;

            var metricsTimer = new System.Timers.Timer();
            metricsTimer.Interval = updateInterval * 1000;
            metricsTimer.Elapsed += (s, e) =>
            {
                // Observations
                var observationCount = _agent.Metrics.GetObservationCount();
                var observationAverage = _agent.Metrics.ObservationAverage;
                observationDelta = observationCount - observationLastCount;

                _agentLogger.Info("[Agent] : Observations - Delta for last " + updateInterval + " seconds: " + observationDelta);
                _agentLogger.Info("[Agent] : Observations - Average for last " + windowInterval + " minutes: " + Math.Round(observationAverage, 5));

                // Assets
                var assetCount = _agent.Metrics.GetAssetCount();
                var assetAverage = _agent.Metrics.AssetAverage;
                assetDelta = assetCount - assetLastCount;

                _agentLogger.Info("[Agent] : Assets - Delta for last " + updateInterval + " seconds: " + assetDelta);
                _agentLogger.Info("[Agent] : Assets - Average for last " + windowInterval + " minutes: " + Math.Round(assetAverage, 5));

                observationLastCount = observationCount;
                assetLastCount = assetCount;
            };
            metricsTimer.Start();
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

        private static void DevicesRequested(string deviceName)
        {
            _agentLogger.Info($"[Agent] : MTConnectDevices Requested : " + deviceName);
        }

        private static void DevicesSent(IDevicesResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Info($"[Agent] : MTConnectDevices Sent : " + document.Header.CreationTime);
            }
        }

        private static void StreamsRequested(string deviceName)
        {
            _agentLogger.Info($"[Agent] : MTConnectStreams Requested : " + deviceName);
        }

        private static void StreamsSent(IStreamsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Info($"[Agent] : MTConnectStreams Sent : " + document.Header.CreationTime);
            }
        }

        private static void AssetsRequested(string deviceName)
        {
            _agentLogger.Info($"[Agent] : MTConnectAssets Requested : " + deviceName);
        }

        private static void AssetsSent(IAssetsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Info($"[Agent] : MTConnectAssets Sent : " + document.Header.CreationTime);
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
            _httpLogger.Info($"[Http Server] : Listening at " + prefix + "..");
        }

        private static void HttpListenerStopped(object sender, string prefix)
        {
            _httpLogger.Info($"[Http Server] : Listener Stopped for " + prefix);
        }

        private static void HttpListenerException(object sender, Exception exception)
        {
            _httpLogger.Info($"[Http Server] : Listener Exception : " + exception.Message);
        }

        private static void HttpClientConnected(object sender, HttpListenerRequest request)
        {
            _httpLogger.Info($"[Http Server] : Client Connected : " + request.LocalEndPoint + " : " + request.Url);
        }

        private static void HttpClientDisconnected(object sender, string remoteEndPoint)
        {
            _httpLogger.Info($"[Http Server] : Client Disconnected : " + remoteEndPoint);
        }

        private static void HttpClientException(object sender, Exception exception)
        {
            _httpLogger.Info($"[Http Server] : Client Exception : " + exception.Message);
        }

        private static void HttpResponseSent(object sender, MTConnectHttpResponse response)
        {
            _httpLogger.Info($"[Http Server] : Response Sent : {response.StatusCode} : {response.ContentType} : Agent Process Time {response.ResponseDuration}ms : Document Format Time {response.FormatDuration}ms : Total Response Time {response.ResponseDuration + response.FormatDuration}ms");
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

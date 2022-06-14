// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MQTTnet;
using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Streams;
using NLog;
using System.Net;
using System.Reflection;

namespace MTConnect.Applications
{
    public class Program
    {
        private static readonly Logger _agentLogger = LogManager.GetLogger("agent-logger");
        private static readonly Logger _agentValidationLogger = LogManager.GetLogger("agent-validation-logger");
        private static readonly Logger _httpLogger = LogManager.GetLogger("http-logger");
        private static readonly Logger _adapterLogger = LogManager.GetLogger("adapter-logger");
        private static readonly Logger _adapterShdrLogger = LogManager.GetLogger("adapter-shdr-logger");
        private static MTConnectAgent _agent;


        /// <summary>
        /// Program Arguments [help|install|debug|run] [configuration_file]
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            string command = "debug";
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
            var configuration = MTConnectAgentConfiguration.Read(configFile);

            switch (command)
            {
                case "run": Init(configuration); break;

                case "debug": Init(configuration, true); break;
            }

            Console.ReadLine();
        }

        private static void Init(MTConnectAgentConfiguration configuration, bool verboseLogging = false)
        {
            if (configuration != null)
            {
                // Create MTConnectAgent
                _agent = new MTConnectAgent(configuration);
                _agent.Version = MTConnectVersions.Max;

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

                _ = Task.Run(async () =>
                {
                    var mqttFactory = new MqttFactory();
                    using (var mqttServer = mqttFactory.CreateMqttServer())
                    {
                        var broker = new MTConnectMqttBroker(_agent, mqttServer);
                        await broker.StartAsync(CancellationToken.None);

                        //var serverOptions = new MqttServerOptions();
                        //await mqttServer.StartAsync(serverOptions);

                        Console.WriteLine("Press Enter to exit.");
                        Console.ReadLine();

                        await broker.StopAsync(CancellationToken.None);

                        //// Stop and dispose the MQTT server if it is no longer needed!
                        //await mqttServer.StopAsync();
                    }
                });

                // Add Adapter Clients
                if (!configuration.Adapters.IsNullOrEmpty())
                {
                    var devices = DeviceConfiguration.FromFile(configuration.Devices, DocumentFormat.XML);
                    if (!devices.IsNullOrEmpty())
                    {
                        // Add Device(s) to Agent
                        foreach (var device in devices)
                        {
                            _agent.AddDevice(device);
                        }

                        foreach (var adapterConfiguration in configuration.Adapters)
                        {
                            var device = devices.FirstOrDefault(o => o.Name == adapterConfiguration.Device);
                            if (device != null)
                            {
                                // Add Adapter Component to Agent Device
                                _agent.AddAdapterComponent(adapterConfiguration);

                                // Create new SHDR Adapter Client to read from SHDR stream
                                var adapterClient = new ShdrAdapterClient(adapterConfiguration, _agent, device);

                                if (verboseLogging)
                                {
                                    adapterClient.AdapterConnected += AdapterConnected;
                                    adapterClient.AdapterDisconnected += AdapterDisconnected;
                                    adapterClient.AdapterConnectionError += AdapterConnectionError;
                                    adapterClient.PingSent += AdapterPingSent;
                                    adapterClient.PongReceived += AdapterPongReceived;
                                    adapterClient.ProtocolReceived += AdapterProtocolReceived;
                                }

                                adapterClient.Start();
                            }
                        }
                    }
                }

                // Start Agent Metrics
                StartMetrics();
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


        #region "Console Output"

        private static void PrintHeader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            Console.WriteLine("Copyright 2022 TrakHound Inc., All Rights Reserved");
            Console.WriteLine("MTConnect MQTT Agent : Version " + version.ToString());
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
            _agentLogger.Info($"[Agent] : MTConnectDevices Requested : " + deviceName);
        }

        private static void StreamsSent(IStreamsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Info($"[Agent] : MTConnectDevices Sent : " + document.Header.CreationTime);
            }
        }

        private static void AssetsRequested(string deviceName)
        {
            _agentLogger.Info($"[Agent] : MTConnectDevices Requested : " + deviceName);
        }

        private static void AssetsSent(IAssetsResponseDocument document)
        {
            if (document != null && document.Header != null)
            {
                _agentLogger.Info($"[Agent] : MTConnectDevices Sent : " + document.Header.CreationTime);
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

        //private static void InvalidDataItem(Devices.DataItem dataItem, string valueType, string value)
        //{
        //    _agentValidationLogger.Warn($"[Agent-Validation] : Validation Failed for {dataItem.Id} : {valueType} = \'{value}\' is Invalid for DataItem of Type \'{dataItem.Type}\'");
        //}

        //private static void InvalidDataItem(Devices.DataItem dataItem, Observations.IObservation observation)
        //{
        //    _agentValidationLogger.Warn($"[Agent-Validation] : Validation Failed for {dataItem.Id} : \'{observation.Value}\' is Invalid for DataItem of Type \'{dataItem.Type}\'");
        //}


        private static void AdapterConnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterDisconnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterConnectionError(object sender, Exception exception)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + exception.Message);
        }

        private static void AdapterPingSent(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterPongReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterLogger.Info($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        private static void AdapterProtocolReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _adapterShdrLogger.Debug($"[Adapter-SHDR] : ID = " + adapterClient.Id + " : " + message);
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

        #endregion
    }
}

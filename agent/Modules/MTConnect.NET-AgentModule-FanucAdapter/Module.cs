// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Fanuc;
using MTConnect.Input;
using MTConnect.Logging;

namespace MTConnect.Modules
{
    /// <summary>
    /// Agent module that reads a Fanuc CNC using FOCAS over Ethernet and adds the
    /// results to the Agent as observations for the configured Device.
    /// </summary>
    public class Module : MTConnectAgentModule
    {
        /// <summary>
        /// Token used in <c>agent.config.yaml</c> to bind this module
        /// (<c>fanuc-adapter</c>).
        /// </summary>
        public const string ConfigurationTypeId = "fanuc-adapter";
        private const string ModuleId = "Fanuc Adapter";

        private readonly FanucAdapterModuleConfiguration _configuration;
        private readonly IFocasClient _client;
        private FanucDataSource _dataSource;


        /// <summary>
        /// Initialises the module and binds the supplied configuration
        /// payload to <see cref="FanucAdapterModuleConfiguration"/>. The connection to
        /// the CNC is opened in <see cref="OnStartAfterLoad"/>.
        /// </summary>
        /// <param name="mtconnectAgent">Agent broker that observations are added to.</param>
        /// <param name="configuration">Raw configuration payload.</param>
        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : this(mtconnectAgent, configuration, null) { }

        internal Module(IMTConnectAgentBroker mtconnectAgent, object configuration, IFocasClient client) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<FanucAdapterModuleConfiguration>(configuration) ?? new FanucAdapterModuleConfiguration();
            _client = client ?? CreateClient();
        }


        /// <summary>
        /// Module lifecycle hook: adds the Fanuc Device model to the Agent
        /// when <see cref="FanucAdapterModuleConfiguration.CreateDevice"/> is enabled.
        /// The Device is updated with the paths, axes and spindles once the CNC is read.
        /// </summary>
        /// <param name="initializeDataItems">Forwarded to the Agent when adding the Device.</param>
        protected override void OnStartBeforeLoad(bool initializeDataItems)
        {
            if (_configuration.CreateDevice)
            {
                if (string.IsNullOrEmpty(_configuration.DeviceKey))
                {
                    Log(MTConnectLogLevel.Error, "Fanuc Adapter : DeviceKey is required when CreateDevice is enabled");
                    return;
                }

                Agent.AddDevice(FanucDevice.Create(_configuration.DeviceKey, _configuration.DeviceUuid, _configuration), initializeDataItems);
            }
        }

        /// <summary>
        /// Module lifecycle hook: creates the data source and starts reading the CNC.
        /// </summary>
        /// <param name="initializeDataItems">Not used.</param>
        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            if (string.IsNullOrEmpty(_configuration.DeviceKey))
            {
                Log(MTConnectLogLevel.Error, "Fanuc Adapter : DeviceKey is not configured");
                return;
            }

            Log(MTConnectLogLevel.Information, $"Fanuc Adapter : Reading {_configuration.DeviceKey} at {_configuration.Server}:{_configuration.Port}");

            _dataSource = new FanucDataSource(_client, _configuration);
            _dataSource.Configuration = new DataSourceConfiguration { ReadInterval = _configuration.ReadInterval };
            _dataSource.ObservationAdded += DataSourceObservationAdded;
            _dataSource.LogReceived += DataSourceLogReceived;
            _dataSource.MachineModelChanged += DataSourceMachineModelChanged;
            _dataSource.Start();
        }

        /// <summary>
        /// Module lifecycle hook: stops reading and closes the connection to the CNC.
        /// </summary>
        protected override void OnStop()
        {
            if (_dataSource != null)
            {
                _dataSource.Stop();
                _dataSource.ObservationAdded -= DataSourceObservationAdded;
                _dataSource.LogReceived -= DataSourceLogReceived;
                _dataSource.MachineModelChanged -= DataSourceMachineModelChanged;
                _dataSource = null;
            }

            _client.Dispose();
        }


        private IFocasClient CreateClient()
        {
            var client = new FocasClient();
            client.LogReceived += DataSourceLogReceived;
            return client;
        }

        private void DataSourceMachineModelChanged(object sender, FanucMachineModel machineModel)
        {
            // Update the Device with the paths, axes and spindles read from the CNC
            if (_configuration.CreateDevice && !string.IsNullOrEmpty(_configuration.DeviceKey))
            {
                Agent.AddDevice(FanucDevice.Create(_configuration.DeviceKey, _configuration.DeviceUuid, _configuration, machineModel), false);
            }
        }

        private void DataSourceObservationAdded(object sender, IObservationInput observation)
        {
            Agent.AddObservation(_configuration.DeviceKey, observation);
        }

        private void DataSourceLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId)
        {
            Log(logLevel, $"Fanuc Adapter : {_configuration.DeviceKey} : {message}", logId);
        }
    }
}

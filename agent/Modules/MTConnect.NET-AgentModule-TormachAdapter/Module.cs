// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Tormach;

namespace MTConnect.Modules
{
    /// <summary>
    /// Agent module that reads a Tormach PathPilot controller through the PathPilot
    /// status server and adds the results to the Agent as observations for the
    /// configured Device.
    /// </summary>
    public class Module : MTConnectAgentModule
    {
        /// <summary>
        /// Token used in <c>agent.config.yaml</c> to bind this module
        /// (<c>tormach-adapter</c>).
        /// </summary>
        public const string ConfigurationTypeId = "tormach-adapter";
        private const string ModuleId = "Tormach Adapter";

        private readonly TormachAdapterModuleConfiguration _configuration;
        private TormachDataSource _dataSource;


        /// <summary>
        /// Initialises the module and binds the supplied configuration
        /// payload to <see cref="TormachAdapterModuleConfiguration"/>. The connection to
        /// the status server is opened in <see cref="OnStartAfterLoad"/>.
        /// </summary>
        /// <param name="mtconnectAgent">Agent broker that observations are added to.</param>
        /// <param name="configuration">Raw configuration payload.</param>
        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<TormachAdapterModuleConfiguration>(configuration) ?? new TormachAdapterModuleConfiguration();
        }


        /// <summary>
        /// Module lifecycle hook: adds the Tormach Device model to the Agent
        /// when <see cref="TormachAdapterModuleConfiguration.CreateDevice"/> is enabled.
        /// The Device is updated with the configured axes once they are received.
        /// </summary>
        /// <param name="initializeDataItems">Forwarded to the Agent when adding the Device.</param>
        protected override void OnStartBeforeLoad(bool initializeDataItems)
        {
            if (_configuration.CreateDevice)
            {
                if (string.IsNullOrEmpty(_configuration.DeviceKey))
                {
                    Log(MTConnectLogLevel.Error, "Tormach Adapter : DeviceKey is required when CreateDevice is enabled");
                    return;
                }

                Agent.AddDevice(TormachDevice.Create(_configuration.DeviceKey, _configuration.DeviceUuid, _configuration), initializeDataItems);
            }
        }

        /// <summary>
        /// Module lifecycle hook: creates the data source and starts reading the status server.
        /// </summary>
        /// <param name="initializeDataItems">Not used.</param>
        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            if (string.IsNullOrEmpty(_configuration.DeviceKey))
            {
                Log(MTConnectLogLevel.Error, "Tormach Adapter : DeviceKey is not configured");
                return;
            }

            Log(MTConnectLogLevel.Information, $"Tormach Adapter : Reading {_configuration.DeviceKey} at {_configuration.Server}:{_configuration.Port}");

            _dataSource = new TormachDataSource(_configuration);
            _dataSource.Configuration = new DataSourceConfiguration { ReadInterval = _configuration.ReadInterval };
            _dataSource.ObservationAdded += DataSourceObservationAdded;
            _dataSource.LogReceived += DataSourceLogReceived;
            _dataSource.MachineInfoChanged += DataSourceMachineInfoChanged;
            _dataSource.Start();
        }

        /// <summary>
        /// Module lifecycle hook: stops reading and closes the connection.
        /// </summary>
        protected override void OnStop()
        {
            if (_dataSource != null)
            {
                _dataSource.Stop();
                _dataSource.ObservationAdded -= DataSourceObservationAdded;
                _dataSource.LogReceived -= DataSourceLogReceived;
                _dataSource.MachineInfoChanged -= DataSourceMachineInfoChanged;
                _dataSource = null;
            }
        }


        private void DataSourceMachineInfoChanged(object sender, TormachInfo machineInfo)
        {
            // Update the Device with the machine name and axes configured in PathPilot
            if (_configuration.CreateDevice && !string.IsNullOrEmpty(_configuration.DeviceKey))
            {
                Agent.AddDevice(TormachDevice.Create(_configuration.DeviceKey, _configuration.DeviceUuid, _configuration, machineInfo), false);
            }
        }

        private void DataSourceObservationAdded(object sender, IObservationInput observation)
        {
            Agent.AddObservation(_configuration.DeviceKey, observation);
        }

        private void DataSourceLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId)
        {
            Log(logLevel, $"Tormach Adapter : {_configuration.DeviceKey} : {message}", logId);
        }
    }
}

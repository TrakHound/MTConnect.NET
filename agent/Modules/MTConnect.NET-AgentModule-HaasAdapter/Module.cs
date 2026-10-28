// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Haas;
using MTConnect.Input;
using MTConnect.Logging;

namespace MTConnect.Modules
{
    /// <summary>
    /// Agent module that reads a Haas CNC control over Ethernet or RS-232
    /// (using Q-commands) and adds the results to the Agent as observations
    /// for the configured Device.
    /// </summary>
    public class Module : MTConnectAgentModule
    {
        /// <summary>
        /// Token used in <c>agent.config.yaml</c> to bind this module
        /// (<c>haas-adapter</c>).
        /// </summary>
        public const string ConfigurationTypeId = "haas-adapter";

        /// <summary>
        /// The class this module's <c>agent.config.yaml</c> settings are read into.
        /// Lets tools such as the Web UI describe the module's options.
        /// </summary>
        public static readonly System.Type ConfigurationType = typeof(HaasAdapterModuleConfiguration);
        private const string ModuleId = "Haas Adapter";

        private readonly HaasAdapterModuleConfiguration _configuration;
        private HaasDataSource _dataSource;


        /// <summary>
        /// Initialises the module and binds the supplied configuration
        /// payload to <see cref="HaasAdapterModuleConfiguration"/>. The connection to
        /// the Haas control is opened in <see cref="OnStartAfterLoad"/>.
        /// </summary>
        /// <param name="mtconnectAgent">Agent broker that observations are added to.</param>
        /// <param name="configuration">Raw configuration payload.</param>
        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<HaasAdapterModuleConfiguration>(configuration) ?? new HaasAdapterModuleConfiguration();
        }


        /// <summary>
        /// Module lifecycle hook: adds the Haas Device model to the Agent
        /// when <see cref="HaasAdapterModuleConfiguration.CreateDevice"/> is enabled.
        /// </summary>
        /// <param name="initializeDataItems">Forwarded to the Agent when adding the Device.</param>
        protected override void OnStartBeforeLoad(bool initializeDataItems)
        {
            if (_configuration.CreateDevice)
            {
                if (string.IsNullOrEmpty(_configuration.DeviceKey))
                {
                    Log(MTConnectLogLevel.Error, "Haas Adapter : DeviceKey is required when CreateDevice is enabled");
                    return;
                }

                Agent.AddDevice(HaasDevice.Create(_configuration.DeviceKey, _configuration.DeviceUuid, _configuration), initializeDataItems);
            }
        }

        /// <summary>
        /// Module lifecycle hook: creates the Ethernet or Serial data source
        /// and starts reading the Haas control.
        /// </summary>
        /// <param name="initializeDataItems">Not used.</param>
        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            if (string.IsNullOrEmpty(_configuration.DeviceKey))
            {
                Log(MTConnectLogLevel.Error, "Haas Adapter : DeviceKey is not configured");
                return;
            }

            if (_configuration.IsSerial)
            {
                _dataSource = new HaasSerialDataSource(_configuration.ToSerialConfiguration());
                Log(MTConnectLogLevel.Information, $"Haas Adapter : Reading {_configuration.DeviceKey} on {_configuration.ComPort}");
            }
            else
            {
                _dataSource = new HaasEthernetDataSource(_configuration.ToEthernetConfiguration());
                Log(MTConnectLogLevel.Information, $"Haas Adapter : Reading {_configuration.DeviceKey} at {_configuration.Server}:{_configuration.Port}");
            }

            _dataSource.Configuration = new DataSourceConfiguration { ReadInterval = _configuration.ReadInterval };
            _dataSource.ObservationAdded += DataSourceObservationAdded;
            _dataSource.LogReceived += DataSourceLogReceived;
            _dataSource.LogLevelEnabled = level => IsLogEnabled(level);
            _dataSource.MachineInfoChanged += DataSourceMachineInfoChanged;
            _dataSource.Start();
        }

        /// <summary>
        /// Module lifecycle hook: stops reading and closes the connection to
        /// the Haas control.
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


        private void DataSourceObservationAdded(object sender, IObservationInput observation)
        {
            Agent.AddObservation(_configuration.DeviceKey, observation);
        }

        private void DataSourceMachineInfoChanged(object sender, HaasMachineInfo machineInfo)
        {
            // Update the Device Description with the model and serial number read from the control
            if (_configuration.CreateDevice && !string.IsNullOrEmpty(_configuration.DeviceKey))
            {
                Agent.AddDevice(HaasDevice.Create(_configuration.DeviceKey, _configuration.DeviceUuid, _configuration, machineInfo), false);
            }
        }

        private void DataSourceLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId)
        {
            Log(logLevel, $"Haas Adapter : {_configuration.DeviceKey} : {message}", logId);
        }
    }
}

using MTConnect.Agents;
using MTConnect.Applications;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Events;

namespace MTConnectAgent
{
    /// <summary>Represents the program.</summary>
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new MTConnectAgentApplication();
            app.Run(args, true);
        }


        /// <summary>Represents the module configuration.</summary>
        public class ModuleConfiguration : DataSourceConfiguration
        {
            /// <summary>Gets or sets the plc address.</summary>
            public string PlcAddress { get; set; }
        }


        /// <summary>Represents the data source module.</summary>
        public class DataSourceModule : MTConnectInputAgentModule
        {
            /// <summary>The configuration type id.</summary>
            public const string ConfigurationTypeId = "datasource"; // This must match the module section in the 'agent.config.yaml' file
            /// <summary>The default id.</summary>
            public const string DefaultId = "DataSource Module"; // The ID is mainly just used for logging.
            private readonly ModuleConfiguration _configuration;


            /// <summary>Initialises a new instance of the data source module type.</summary>
            /// <param name="agent">The agent.</param>
            /// <param name="configuration">The configuration.</param>
            public DataSourceModule(IMTConnectAgentBroker agent, object configuration) : base(agent)
            {
                Id = DefaultId;

                _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
                Configuration = _configuration;
            }


            /// <summary>Runs the on add device operation.</summary>
            /// <returns>The result of the operation.</returns>
            protected override IDevice OnAddDevice()
            {
                var device = new Device();
                device.Uuid = "demo-agent-device";
                device.Id = "demo";
                device.Name = "demo";

                device.AddDataItem(new AvailabilityDataItem() { Name = "avail" });
                device.AddDataItem(new DateCodeDataItem() { Name = "date" });

                return device;
            }


            /// <summary>Runs the on read operation.</summary>
            protected override void OnRead()
            {
                Log(MTConnect.Logging.MTConnectLogLevel.Information, $"Read PLC Data from ({_configuration.PlcAddress})");

                AddValueObservation("avail", Availability.AVAILABLE);
                AddValueObservation("date", System.DateTime.Now.ToString("o"));
            }

        }
    }
}

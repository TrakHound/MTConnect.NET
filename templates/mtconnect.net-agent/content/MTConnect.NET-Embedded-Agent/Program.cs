using MTConnect.Agents;
using MTConnect.Applications;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Events;

namespace MTConnectAgent
{
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new MTConnectAgentApplication();
            app.Run(args, true);
        }


        public class ModuleConfiguration : DataSourceConfiguration
        {
            public string PlcAddress { get; set; }
        }


        public class DataSourceModule : MTConnectInputAgentModule
        {
            public const string ConfigurationTypeId = "datasource"; // This must match the module section in the 'agent.config.yaml' file
            public const string DefaultId = "DataSource Module"; // The ID is mainly just used for logging.
            private readonly ModuleConfiguration _configuration;


            public DataSourceModule(IMTConnectAgentBroker agent, object configuration) : base(agent)
            {
                Id = DefaultId;

                _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
                Configuration = _configuration;
            }


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


            protected override void OnRead()
            {
                Log(MTConnect.Logging.MTConnectLogLevel.Information, $"Read PLC Data from ({_configuration.PlcAddress})");

                AddValueObservation("avail", Availability.AVAILABLE);
                AddValueObservation("date", System.DateTime.Now.ToString("o"));
            }

        }
    }
}

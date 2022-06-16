using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Input;

namespace MTConnect.Applications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Read Agent Configuration File
            var configuration = AgentConfiguration.Read<HttpAgentConfiguration>();

            // Create MTConnect Agent
            var agent = new MTConnectAgent(configuration);
            agent.Version = new Version(1, 8);

            // Read Devices.xml File
            var devices = DeviceConfiguration.FromFile(configuration?.Devices, DocumentFormat.XML);

            // Add Devices to MTConnect Agent
            agent.AddDevices(devices);


            // Start the Http Server
            var server = new MTConnectHttpServer(configuration, agent);
            server.Start();


            // Start Adding Observations directly to Agent
            agent.AddObservation("VMC-3Axis", new ObservationInput("avail", Availability.AVAILABLE));
            agent.AddObservation("VMC-3Axis", new ObservationInput("estop", EmergencyStop.ARMED));
            agent.AddObservation("VMC-3Axis", new ObservationInput("mode", ControllerMode.AUTOMATIC));
            agent.AddObservation("VMC-3Axis", new ObservationInput("execution", Execution.ACTIVE));


            var i = 0;
            while (true)
            {
                agent.AddObservation("VMC-3Axis", new ObservationInput("Xact", i++));

                Console.ReadLine();
            }
        }
    }
}

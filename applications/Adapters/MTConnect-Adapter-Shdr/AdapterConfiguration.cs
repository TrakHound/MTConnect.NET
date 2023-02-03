using MTConnect.Configurations;

namespace MTConnect.Applications
{
    // This is a custom Configuration file for your own Adapter.
    // Simply add Properties and they will be Serialized/Deserialized in the configuration file

    // Example properties could include Address for PLC connection, Variables, Labels, etc.

    public class AdapterConfiguration : ShdrAdapterApplicationConfiguration
    {
        public string PlcAddress { get; set; }

        public int PlcPort { get; set; }
    }
}

using MTConnect.Adapters.Shdr;
using MTConnect.Applications.Adapters;
using MTConnect.Configurations;

namespace MTConnect.Applications
{
    // This is an implementation of the MTConnectShdrAdapterApplication using the custom configuration file type

    internal class AdapterApplication : MTConnectShdrAdapterApplication<AdapterConfiguration>
    {
        private AdapterConfiguration _configuration;


        public AdapterConfiguration Configuration => _configuration;


        public AdapterApplication(MTConnectShdrAdapterEngine<AdapterConfiguration> engine) : base(engine)
        {
            ConfigurationType = typeof(AdapterConfiguration);
        }


        protected override void OnStartAdapter()
        {
            // Start Engine
            if (Engine != null)
            {
                Engine.Configuration = Configuration;

                Engine.Start();
            }
        }

        protected override void OnStopAdapter()
        {
            // Stop Engine
            if (Engine != null) Engine.Stop();
        }


        protected override IShdrAdapterApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        {
            _configuration = ShdrAdapterApplicationConfiguration.Read<AdapterConfiguration>(configurationPath);
            return _configuration;
        }

        protected override void OnAdapterConfigurationUpdated(ShdrAdapterApplicationConfiguration configuration)
        {
            _configuration = configuration as AdapterConfiguration;
        }

        protected override void OnAdapterConfigurationWatcherInitialize(IShdrAdapterApplicationConfiguration configuration)
        {
            _adapterConfigurationWatcher = new AdapterConfigurationFileWatcher<AdapterConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        }
    }
}

using MTConnect.Adapters.Shdr;
using MTConnect.Applications.Adapters;
using MTConnect.Configurations;

namespace MTConnect.Applications
{
    // This is an implementation of the MTConnectShdrAdapterApplication using the custom configuration file type

    internal class AdapterApplication : MTConnectAdapterApplication
    {
        private AdapterConfiguration _configuration;


        public AdapterConfiguration Configuration => _configuration;


        public AdapterApplication(MTConnectAdapterEngine engine) : base(engine)
        {

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


        //protected override IAdapterApplicationConfiguration OnConfigurationFileRead(string configurationPath)
        //{
        //    _configuration = AdapterApplicationConfiguration.Read<AdapterConfiguration>(configurationPath);
        //    return _configuration;
        //}

        //protected override void OnAdapterConfigurationUpdated(AdapterApplicationConfiguration configuration)
        //{
        //    _configuration = configuration as AdapterConfiguration;
        //}

        //protected override void OnAdapterConfigurationWatcherInitialize(IAdapterApplicationConfiguration configuration)
        //{
        //    _adapterConfigurationWatcher = new AdapterConfigurationFileWatcher<AdapterConfiguration>(configuration.Path, configuration.ConfigurationFileRestartInterval * 1000);
        //}
    }
}

using MTConnect.Agents;
using MTConnect.Configurations;
using System;
using System.Collections.Concurrent;

namespace MTConnect
{
    public class MTConnectDataSources
    {
        private static readonly ConcurrentBag<Type> dataSourceTypes = new ConcurrentBag<Type>();
        private static readonly ConcurrentDictionary<string, IMTConnectDataSource> _dataSources = new ConcurrentDictionary<string, IMTConnectDataSource>();
        private static bool _firstRead = true;

        private readonly IAgentApplicationConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;


        public MTConnectDataSources(IAgentApplicationConfiguration configuration, IMTConnectAgentBroker mtconnectAgent)
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
        }

        public void Load()
        {
            InitializeControllers();

            var controllerTypes = dataSourceTypes.ToArray();
            if (!controllerTypes.IsNullOrEmpty())
            {
                foreach (var controllerType in controllerTypes)
                {
                    var configurationTypeId = GetConfigurationTypeId(controllerType);
                    if (configurationTypeId != null)
                    {
                        var controllerConfigurations = _configuration.GetDataSources(configurationTypeId);
                        if (!controllerConfigurations.IsNullOrEmpty())
                        {
                            foreach (var controllerConfiguration in controllerConfigurations)
                            {
                                try
                                {
                                    // Create new Instance of the DataSource and add to cached dictionary
                                    var controller = (IMTConnectDataSource)Activator.CreateInstance(controllerType, new object[] { _mtconnectAgent, controllerConfiguration });

                                    var controllerId = Guid.NewGuid().ToString();

                                    _dataSources.TryAdd(controllerId, controller);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        public void Start()
        {
            if (!_dataSources.IsNullOrEmpty())
            {
                foreach (var controller in _dataSources)
                {
                    controller.Value.Start();
                }
            }
        }

        public void Stop()
        {
            if (!_dataSources.IsNullOrEmpty())
            {
                foreach (var controller in _dataSources)
                {
                    controller.Value.Stop();
                }
            }
        }


        private static void InitializeControllers()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var assemblyTypes = assembly.GetTypes();
                        if (!assemblyTypes.IsNullOrEmpty())
                        {
                            foreach (var type in assemblyTypes)
                            {
                                if (typeof(IMTConnectDataSource).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                                {
                                    dataSourceTypes.Add(type);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private static string GetConfigurationTypeId(Type type)
        {
            if (type != null)
            {
                try
                {
                    var configurationTypeIdField = type.GetField("ConfigurationTypeId");

                    return configurationTypeIdField.GetValue(null)?.ToString();
                }
                catch { }
            }

            return null;
        }
    }
}

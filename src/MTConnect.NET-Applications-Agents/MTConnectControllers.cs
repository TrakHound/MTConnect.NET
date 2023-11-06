using MTConnect.Agents;
using MTConnect.Configurations;
using System;
using System.Collections.Concurrent;

namespace MTConnect
{
    public class MTConnectControllers
    {
        private static readonly ConcurrentBag<Type> _controllerTypes = new ConcurrentBag<Type>();
        private static readonly ConcurrentDictionary<string, IMTConnectController> _controllers = new ConcurrentDictionary<string, IMTConnectController>();
        private static bool _firstRead = true;

        private readonly IAgentApplicationConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;


        public MTConnectControllers(IAgentApplicationConfiguration configuration, IMTConnectAgentBroker mtconnectAgent)
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
        }

        public void Load()
        {
            InitializeControllers();

            var controllerTypes = _controllerTypes.ToArray();
            if (!controllerTypes.IsNullOrEmpty())
            {
                foreach (var controllerType in controllerTypes)
                {
                    var configurationTypeId = GetConfigurationTypeId(controllerType);
                    if (configurationTypeId != null)
                    {
                        var controllerConfigurations = _configuration.GetControllers(configurationTypeId);
                        if (!controllerConfigurations.IsNullOrEmpty())
                        {
                            foreach (var controllerConfiguration in controllerConfigurations)
                            {
                                try
                                {
                                    // Create new Instance of the Controller and add to cached dictionary
                                    var controller = (IMTConnectController)Activator.CreateInstance(controllerType, new object[] { _mtconnectAgent, controllerConfiguration });

                                    var controllerId = Guid.NewGuid().ToString();

                                    _controllers.TryAdd(controllerId, controller);
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
            if (!_controllers.IsNullOrEmpty())
            {
                foreach (var controller in _controllers)
                {
                    controller.Value.Start();
                }
            }
        }

        public void Stop()
        {
            if (!_controllers.IsNullOrEmpty())
            {
                foreach (var controller in _controllers)
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
                                if (typeof(IMTConnectController).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                                {
                                    _controllerTypes.Add(type);
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

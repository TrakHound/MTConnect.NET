// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Collections.Concurrent;

namespace MTConnect.Agents
{
    public class MTConnectAgentModules
    {
        private static readonly ConcurrentBag<Type> _moduleTypes = new ConcurrentBag<Type>();
        private static readonly ConcurrentDictionary<string, IMTConnectAgentModule> _modules = new ConcurrentDictionary<string, IMTConnectAgentModule>();

        private readonly IAgentApplicationConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;


        public MTConnectAgentModules(IAgentApplicationConfiguration configuration, IMTConnectAgentBroker mtconnectAgent)
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
        }

        public void Load()
        {
            InitializeModules();

            var moduleTypes = _moduleTypes.ToArray();
            if (!moduleTypes.IsNullOrEmpty())
            {
                foreach (var moduleType in moduleTypes)
                {
                    var configurationTypeId = GetConfigurationTypeId(moduleType);
                    if (configurationTypeId != null)
                    {
                        var moduleConfigurations = _configuration.GetModules(configurationTypeId);
                        if (!moduleConfigurations.IsNullOrEmpty())
                        {
                            foreach (var moduleConfiguration in moduleConfigurations)
                            {
                                try
                                {
                                    // Create new Instance of the Controller and add to cached dictionary
                                    var module = (IMTConnectAgentModule)Activator.CreateInstance(moduleType, new object[] { _mtconnectAgent, moduleConfiguration });

                                    var moduleId = Guid.NewGuid().ToString();

                                    _modules.TryAdd(moduleId, module);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        public void StartBeforeLoad()
        {
            if (!_modules.IsNullOrEmpty())
            {
                foreach (var module in _modules)
                {
                    module.Value.StartBeforeLoad();
                }
            }
        }

        public void StartAfterLoad()
        {
            if (!_modules.IsNullOrEmpty())
            {
                foreach (var module in _modules)
                {
                    module.Value.StartAfterLoad();
                }
            }
        }

        public void Stop()
        {
            if (!_modules.IsNullOrEmpty())
            {
                foreach (var module in _modules)
                {
                    module.Value.Stop();
                }

                _modules.Clear();
            }
        }


        private static void InitializeModules()
        {
            _moduleTypes.Clear();

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
                                if (typeof(IMTConnectAgentModule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                                {
                                    _moduleTypes.Add(type);
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

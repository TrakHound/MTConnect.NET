// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Logging;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    public class MTConnectAgentModules
    {
        private static readonly List<Type> _moduleTypes = new List<Type>();
        private static readonly Dictionary<string, IMTConnectAgentModule> _modules = new Dictionary<string, IMTConnectAgentModule>();
        private static readonly object _lock = new object();

        private readonly IAgentApplicationConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;


        public event EventHandler<IMTConnectAgentModule> ModuleLoaded;

        public event MTConnectLogEventHandler LogReceived;


        public MTConnectAgentModules(IAgentApplicationConfiguration configuration, IMTConnectAgentBroker mtconnectAgent)
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
        }

        public void Load()
        {
            InitializeModules();

            Type[] moduleTypes;
            lock (_lock) moduleTypes = _moduleTypes.ToArray();
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
                                    module.LogReceived += HandleModuleLogReceived;

                                    var moduleId = Guid.NewGuid().ToString();

                                    if (ModuleLoaded != null) ModuleLoaded.Invoke(this, module);

                                    lock (_lock) _modules.Add(moduleId, module);
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            if (_configuration.IsModuleConfigured(configurationTypeId))
                            {
								try
								{
									// Create new Instance of the Controller and add to cached dictionary
									var module = (IMTConnectAgentModule)Activator.CreateInstance(moduleType, new object[] { _mtconnectAgent, null });
									module.LogReceived += HandleModuleLogReceived;

									var moduleId = Guid.NewGuid().ToString();

									if (ModuleLoaded != null) ModuleLoaded.Invoke(this, module);

									lock (_lock) _modules.Add(moduleId, module);
								}
								catch { }
							}
                        }
                    }
                }
            }
        }

        public void StartBeforeLoad(bool initializeDataItems)
        {
            Dictionary<string, IMTConnectAgentModule> modules;
            lock (_lock) modules = _modules;
            if (!modules.IsNullOrEmpty())
            {
                foreach (var module in modules)
                {
                    module.Value.StartBeforeLoad(initializeDataItems);
                }
            }
        }

        public void StartAfterLoad(bool initializeDataItems)
        {
            Dictionary<string, IMTConnectAgentModule> modules;
            lock (_lock) modules = _modules;
            if (!modules.IsNullOrEmpty())
            {
                foreach (var module in modules)
                {
                    module.Value.StartAfterLoad(initializeDataItems);
                }
            }
        }

        public void Stop()
        {
            Dictionary<string, IMTConnectAgentModule> modules;
            lock (_lock) modules = _modules;
            if (!modules.IsNullOrEmpty())
            {
                foreach (var module in modules)
                {
                    module.Value.Stop();
                }

                lock (_lock) _modules.Clear();
            }
        }


        private static void InitializeModules()
        {
            lock (_lock) _moduleTypes.Clear();

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
                                    lock (_lock) _moduleTypes.Add(type);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void HandleModuleLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId = null)
        {
            if (LogReceived != null) LogReceived.Invoke(sender, logLevel, message, logId);
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

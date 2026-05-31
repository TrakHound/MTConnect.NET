// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Logging;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// Discovers <see cref="IMTConnectAgentModule"/> implementations from loaded assemblies, instantiates the ones enabled in the Agent configuration, and relays their lifecycle calls and log events.
    /// </summary>
    public class MTConnectAgentModules
    {
        private static readonly List<Type> _moduleTypes = new List<Type>();
        private static readonly Dictionary<string, IMTConnectAgentModule> _modules = new Dictionary<string, IMTConnectAgentModule>();
        private static readonly object _lock = new object();

        private readonly IAgentApplicationConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;


        /// <summary>
        /// Raised once for each module after it has been instantiated and is ready to be started.
        /// </summary>
        public event EventHandler<IMTConnectAgentModule> ModuleLoaded;

        /// <summary>
        /// Raised when any hosted module emits a log entry.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes a new instance bound to the given Agent configuration and Agent broker.
        /// </summary>
        /// <param name="configuration">The Agent configuration used to determine which modules are enabled and how many instances to create.</param>
        /// <param name="mtconnectAgent">The Agent broker passed to each module instance.</param>
        public MTConnectAgentModules(IAgentApplicationConfiguration configuration, IMTConnectAgentBroker mtconnectAgent)
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
        }

        /// <summary>
        /// Discover all available module types and create instances for every module that is enabled in the configuration.
        /// </summary>
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
                                var moduleCount = _configuration.GetModuleCount(configurationTypeId);
                                if (moduleCount > 0)
                                {
                                    for (int i = 0; i < moduleCount; i++)
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
            }
        }

        /// <summary>
        /// Invoke <see cref="IMTConnectAgentModule.StartBeforeLoad(bool)"/> on every loaded module.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, modules should initialize DataItems with their default observations.</param>
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

        /// <summary>
        /// Invoke <see cref="IMTConnectAgentModule.StartAfterLoad(bool)"/> on every loaded module.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, modules should initialize DataItems with their default observations.</param>
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

        /// <summary>
        /// Invoke <see cref="IMTConnectAgentModule.Stop"/> on every loaded module and clear the module cache.
        /// </summary>
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

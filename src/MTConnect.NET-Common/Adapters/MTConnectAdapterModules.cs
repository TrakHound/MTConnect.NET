// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    public class MTConnectAdapterModules
    {
        private static readonly List<Type> _moduleTypes = new List<Type>();
        private static readonly Dictionary<string, IMTConnectAdapterModule> _modules = new Dictionary<string, IMTConnectAdapterModule>();
        private static readonly object _lock = new object();

        private readonly IAdapterApplicationConfiguration _configuration;


        public IEnumerable<IMTConnectAdapterModule> Modules = _modules.Values;

        public event EventHandler<IMTConnectAdapterModule> ModuleLoaded;


        public MTConnectAdapterModules(IAdapterApplicationConfiguration configuration)
        {
            _configuration = configuration;
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
                                    var moduleId = Guid.NewGuid().ToString();

                                    // Create new Instance of the Module and add to cached dictionary
                                    var module = (IMTConnectAdapterModule)Activator.CreateInstance(moduleType, new object[] { moduleId, moduleConfiguration });

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

        public void Start()
        {
            Dictionary<string, IMTConnectAdapterModule> modules;
            lock (_lock) modules = _modules;
            if (!modules.IsNullOrEmpty())
            {
                foreach (var module in modules)
                {
                    module.Value.Start();
                }
            }
        }

        public void Stop()
        {
            Dictionary<string, IMTConnectAdapterModule> modules;
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
                                if (typeof(IMTConnectAdapterModule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
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

        private static string GetConfigurationTypeId(Type type)
        {
            if (type != null)
            {
                try
                {
                    var configurationTypeIdField = type.GetField("ConfigurationTypeId");
                    if (configurationTypeIdField != null)
                    {
                        return configurationTypeIdField.GetValue(null)?.ToString();
                    }
                }
                catch { }
            }

            return null;
        }
    }
}

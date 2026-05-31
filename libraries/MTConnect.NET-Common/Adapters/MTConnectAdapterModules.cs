// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    /// <summary>
    /// Discovers <see cref="IMTConnectAdapterModule"/> implementations across loaded assemblies, instantiates one per matching configuration entry, and drives their start/stop lifecycle as a group.
    /// </summary>
    public class MTConnectAdapterModules
    {
        private static readonly List<Type> _moduleTypes = new List<Type>();
        private static readonly Dictionary<string, IMTConnectAdapterModule> _modules = new Dictionary<string, IMTConnectAdapterModule>();
        private static readonly object _lock = new object();

        private readonly IAdapterApplicationConfiguration _configuration;


        /// <summary>
        /// The currently loaded module instances.
        /// </summary>
        public IEnumerable<IMTConnectAdapterModule> Modules = _modules.Values;

        /// <summary>
        /// Raised once for each module as it is instantiated during <see cref="Load"/>.
        /// </summary>
        public event EventHandler<IMTConnectAdapterModule> ModuleLoaded;


        /// <summary>
        /// Initializes the loader with the application configuration that supplies per-module settings.
        /// </summary>
        /// <param name="configuration">The adapter application configuration.</param>
        public MTConnectAdapterModules(IAdapterApplicationConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Scans loaded assemblies for module types and creates one instance per matching configuration entry, raising <see cref="ModuleLoaded"/> for each; instantiation failures are swallowed so one bad module does not block the rest.
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

        /// <summary>
        /// Starts every loaded module.
        /// </summary>
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

        /// <summary>
        /// Stops every loaded module and clears the loaded set.
        /// </summary>
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

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Observations.Input;
using System;
using System.Collections.Concurrent;

namespace MTConnect.Agents
{
    public class MTConnectAgentProcessors
    {
        private static readonly ConcurrentBag<Type> _processorTypes = new ConcurrentBag<Type>();
        private static readonly ConcurrentDictionary<string, IMTConnectAgentProcessor> _processors = new ConcurrentDictionary<string, IMTConnectAgentProcessor>();


        private readonly IAgentApplicationConfiguration _configuration;


        public MTConnectAgentProcessors(IAgentApplicationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Load()
        {
            InitializeProcessors();

            var processorTypes = _processorTypes.ToArray();
            if (!processorTypes.IsNullOrEmpty())
            {
                foreach (var processorType in processorTypes)
                {
                    var configurationTypeId = GetConfigurationTypeId(processorType);
                    if (configurationTypeId != null)
                    {
                        var processorConfigurations = _configuration.GetProcessors(configurationTypeId);
                        if (!processorConfigurations.IsNullOrEmpty())
                        {
                            foreach (var processorConfiguration in processorConfigurations)
                            {
                                try
                                {
                                    // Create new Instance of the Controller and add to cached dictionary
                                    var processor = (IMTConnectAgentProcessor)Activator.CreateInstance(processorType, new object[] { processorConfiguration });

                                    var processorId = Guid.NewGuid().ToString();

                                    _processors.TryAdd(processorId, processor);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }


        public IObservationInput Process(ProcessObservation observation)
        {
            IObservationInput outputObservation = null;

            if (observation != null && !_processors.IsNullOrEmpty())
            {
                foreach (var processor in _processors)
                {
                    outputObservation = processor.Value.Process(observation);
                }
            }

            return outputObservation;
        }

        public IAsset Process(IAsset asset)
        {
            var outputAsset = asset;

            if (asset != null && !_processors.IsNullOrEmpty())
            {
                foreach (var processor in _processors)
                {
                    outputAsset = processor.Value.Process(outputAsset);
                }
            }

            return outputAsset;
        }

        public void Dispose()
        {
            if (!_processors.IsNullOrEmpty())
            {
                _processors.Clear();
            }
        }


        private static void InitializeProcessors()
        {
            _processorTypes.Clear();

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
                                if (typeof(IMTConnectAgentProcessor).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                                {
                                    _processorTypes.Add(type);
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

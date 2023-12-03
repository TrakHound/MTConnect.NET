// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Input;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    public class MTConnectAgentProcessors
    {
        private static readonly List<Type> _processorTypes = new List<Type>();
        private static readonly Dictionary<string, IMTConnectAgentProcessor> _processors = new Dictionary<string, IMTConnectAgentProcessor>();
        private static readonly object _lock = new object();


        private readonly IAgentApplicationConfiguration _configuration;


        public event EventHandler<IMTConnectAgentProcessor> ProcessorLoaded;


        public MTConnectAgentProcessors(IAgentApplicationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Load()
        {
            InitializeProcessors();

            Type[] processorTypes;
            lock (_lock) processorTypes = _processorTypes.ToArray();
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

                                    if (ProcessorLoaded != null) ProcessorLoaded.Invoke(this, processor);

                                    lock (_lock) _processors.Add(processorId, processor);
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
            var defaultObservation = new ObservationInput();
            defaultObservation.DeviceKey = observation.DataItem.Device?.Uuid;
            defaultObservation.DataItemKey = observation.DataItem.Id;
            defaultObservation.Values = observation.Values;
            defaultObservation.Timestamp = observation.Timestamp.ToUnixTime();
            IObservationInput outputObservation = defaultObservation;

            Dictionary<string, IMTConnectAgentProcessor> processors;
            lock (_lock) processors = _processors;
            if (observation != null && !processors.IsNullOrEmpty())
            {
                foreach (var processor in processors)
                {
                    outputObservation = processor.Value.Process(observation);
                }
            }

            return outputObservation;
        }

        public IAsset Process(IAsset asset)
        {
            var outputAsset = asset;

            Dictionary<string, IMTConnectAgentProcessor> processors;
            lock (_lock) processors = _processors;
            if (asset != null && !processors.IsNullOrEmpty())
            {
                foreach (var processor in processors)
                {
                    outputAsset = processor.Value.Process(outputAsset);
                }
            }

            return outputAsset;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (!_processors.IsNullOrEmpty())
                {
                    _processors.Clear();
                }
            }
        }


        private static void InitializeProcessors()
        {
            lock (_lock) _processorTypes.Clear();

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
                                    lock (_lock) _processorTypes.Add(type);
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

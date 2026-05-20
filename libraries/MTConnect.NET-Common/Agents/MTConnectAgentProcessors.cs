// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Input;
using MTConnect.Logging;
using System;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// Discovers <see cref="IMTConnectAgentProcessor"/> implementations from loaded assemblies, instantiates the ones enabled in the Agent configuration, and runs incoming observations and Assets through the resulting processor chain.
    /// </summary>
    public class MTConnectAgentProcessors
    {
        private static readonly List<Type> _processorTypes = new List<Type>();
        private static readonly Dictionary<string, IMTConnectAgentProcessor> _processors = new Dictionary<string, IMTConnectAgentProcessor>();
        private static readonly object _lock = new object();


        private readonly IAgentApplicationConfiguration _configuration;


        /// <summary>
        /// Raised once for each processor after it has been instantiated and loaded.
        /// </summary>
        public event EventHandler<IMTConnectAgentProcessor> ProcessorLoaded;

        /// <summary>
        /// Raised when any hosted processor emits a log entry.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes a new instance bound to the given Agent configuration.
        /// </summary>
        /// <param name="configuration">The Agent configuration used to determine which processors are enabled.</param>
        public MTConnectAgentProcessors(IAgentApplicationConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Discover all available processor types and create and load instances for every processor that is enabled in the configuration.
        /// </summary>
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
                                    processor.LogReceived += HandleProcessorLogReceived;
                                    processor.Load();

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


        /// <summary>
        /// Run an incoming observation through every loaded processor in turn, returning the final transformed result.
        /// </summary>
        /// <param name="observation">The observation, together with its resolved Device and DataItem context, to process.</param>
        /// <returns>The observation input produced by the processor chain; when no processors are loaded, a faithful copy of the input observation.</returns>
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

        /// <summary>
        /// Run an incoming Asset through every loaded processor in turn, returning the final transformed result.
        /// </summary>
        /// <param name="asset">The Asset to process.</param>
        /// <returns>The Asset produced by the processor chain; when no processors are loaded, the input Asset unchanged.</returns>
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

        /// <summary>
        /// Release all loaded processors and clear the processor cache.
        /// </summary>
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

        private void HandleProcessorLogReceived(object sender, MTConnectLogLevel logLevel, string message, string logId = null)
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

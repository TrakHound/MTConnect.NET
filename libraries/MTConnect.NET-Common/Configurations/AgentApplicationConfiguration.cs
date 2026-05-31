// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent Application
    /// </summary>
    public class AgentApplicationConfiguration : AgentConfiguration, IAgentApplicationConfiguration
    {
        /// <summary>
        /// Optional static UUID to assign to the Agent meta-device. When set,
        /// this value overrides the per-boot <c>Guid.NewGuid()</c> default
        /// applied by <see cref="MTConnect.Agents.MTConnectAgentInformation"/>'s
        /// parameterless constructor and survives restarts without relying on
        /// <c>agent.information.json</c> being present on disk. Corresponds to
        /// <c>AgentDeviceUUID</c> in the cppagent reference implementation.
        /// Per MTConnect v2.7 XSD <c>UuidType</c>, the uuid identifies the
        /// element "for its entire life" — <c>Header.instanceId</c> is the
        /// per-boot discriminator.
        /// </summary>
        [JsonPropertyName("agentUuid")]
        public string AgentUuid { get; set; }


        /// <summary>
        /// The Path to look for the file(s) that represent the Device Information Models to load into the Agent.
        /// The path can either be a single file or a directory. The path can be absolute or relative to the executable's directory
        /// </summary>
        [JsonPropertyName("devices")]
        public string Devices { get; set; }


        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.
        /// </summary>
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// Changes the display name of the service. This helps with identification when multiple agents on run as services on the same machine.
        /// </summary>
        [JsonPropertyName("serviceDisplayName")]
        public string ServiceDisplayName { get; set; }

        /// <summary>
        /// Changes the description of the service. This helps with identification when multiple agents on run as services on the same machine.
        /// </summary>
        [JsonPropertyName("serviceDisplayName")]
        public string ServiceDescription { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        [JsonPropertyName("serviceAutoStart")]
        public bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether the Agent buffers are durable and retain state after restart
        /// </summary>
        [JsonPropertyName("durable")]
        public bool Durable { get; set; }

        /// <summary>
        /// The base path to the directory to write the File Buffers when 'durable = true'
        /// </summary>
        [JsonPropertyName("durableBufferPath")]
        public string DurableBufferPath { get; set; }

        /// <summary>
        /// Gets or Sets whether the durable Agent buffers use Compression
        /// </summary>
        [JsonPropertyName("useBufferCompression")]
        public bool UseBufferCompression { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart
        /// </summary>
        [JsonPropertyName("monitorConfigurationFiles")]
        public bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        [JsonPropertyName("configurationFileRestartInterval")]
        public int ConfigurationFileRestartInterval { get; set; }


        /// <summary>
        /// The raw, untyped module configuration sections declared for the agent, each later resolved to a strongly typed configuration on demand.
        /// </summary>
        [JsonPropertyName("modules")]
        public IEnumerable<object> Modules { get; set; }

        /// <summary>
        /// The raw, untyped processor configuration sections declared for the agent's observation/asset processing pipeline.
        /// </summary>
        [JsonPropertyName("processors")]
        public IEnumerable<object> Processors { get; set; }


        /// <summary>
        /// Initializes a new instance on top of the base agent defaults, with no device path or service identity and configuration file monitoring enabled.
        /// </summary>
        public AgentApplicationConfiguration() : base()
        {
            Devices = null;
            ServiceName = null;
            ServiceDisplayName = null;
            ServiceDescription = null;
            ServiceAutoStart = true;
            MonitorConfigurationFiles = true;
            ConfigurationFileRestartInterval = 2;
        }


        /// <summary>
        /// Flattens every declared module section into a single map keyed by module identifier, or null when no modules are configured.
        /// </summary>
        public Dictionary<object, object> GetModules()
        {
            if (!Modules.IsNullOrEmpty())
            {
                var configurations = new Dictionary<object, object>();

                foreach (var configurationObj in Modules)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        foreach (var entry in rootDictionary)
                        {
                            configurations.Add(entry.Key, entry.Value);
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }

        /// <summary>
        /// Returns every module section declared under the given key as untyped objects, or null when the key is empty or no modules are configured.
        /// </summary>
        /// <param name="key">The module identifier whose sections are requested.</param>
        public IEnumerable<object> GetModules(string key)
        {
            if (!string.IsNullOrEmpty(key) && !Modules.IsNullOrEmpty())
            {
                var configurations = new List<object>();

                foreach (var configurationObj in Modules)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        if (rootDictionary.ContainsKey(key))
                        {
                            var obj = rootDictionary[key];
                            if (obj != null)
                            {
                                configurations.Add(obj);
                            }
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }

        /// <summary>
        /// Counts the module sections declared under the given key; returns zero when the key is empty or no modules are configured.
        /// </summary>
        /// <param name="key">The module identifier to count sections for.</param>
        public int GetModuleCount(string key)
        {
            if (!string.IsNullOrEmpty(key) && !Modules.IsNullOrEmpty())
            {
                var count = 0;

                foreach (var configurationObj in Modules)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        if (rootDictionary.ContainsKey(key))
                        {
                            count++;
                        }
                    }
                    catch { }
                }

                return count;
            }

            return 0;
        }

        /// <summary>
        /// Returns every module section declared under the given key bound to <typeparamref name="TConfiguration"/>; sections present with no body yield a default-constructed instance, and sections that fail to bind are skipped.
        /// </summary>
        /// <typeparam name="TConfiguration">The strongly typed configuration the module sections are bound to.</typeparam>
        /// <param name="key">The module identifier whose sections are requested.</param>
        public IEnumerable<TConfiguration> GetModules<TConfiguration>(string key)
        {
            if (!string.IsNullOrEmpty(key) && !Modules.IsNullOrEmpty())
            {
                var configurations = new List<TConfiguration>();

                foreach (var configurationObj in Modules)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        if (rootDictionary.ContainsKey(key))
                        {
                            var obj = rootDictionary[key];
                            if (obj != null)
                            {
                                var serializerBuilder = new SerializerBuilder();
                                serializerBuilder.WithNamingConvention(CamelCaseNamingConvention.Instance);

                                var serializer = serializerBuilder.Build();
                                var yaml = serializer.Serialize(obj);
                                if (yaml != null)
                                {
                                    var deserializerBuilder = new DeserializerBuilder();
                                    deserializerBuilder.WithNamingConvention(CamelCaseNamingConvention.Instance);
                                    deserializerBuilder.IgnoreUnmatchedProperties();

                                    var deserializer = deserializerBuilder.Build();

                                    var configuration = deserializer.Deserialize<TConfiguration>(yaml);
                                    if (configuration != null)
                                    {
                                        configurations.Add(configuration);
                                    }
                                }
                            }
                            else
                            {
                                var constructor = typeof(TConfiguration).GetConstructor(new Type[] { });
                                var configuration = constructor.Invoke(new object[] { });
                                if (configuration != null)
                                {
                                    configurations.Add((TConfiguration)configuration);
                                }
                            }
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }

        /// <summary>
        /// Appends a new module section under the given key to the existing module list, creating the list if necessary. No-ops when the key is empty.
        /// </summary>
        /// <param name="key">The module identifier the section is registered under.</param>
        /// <param name="moduleConfiguration">The module configuration object to store.</param>
        public void AddModule(string key, object moduleConfiguration)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var rootDictionary = new Dictionary<object, object>();
                rootDictionary.Add(key, moduleConfiguration);

                var modules = Modules?.ToList();
                if (modules == null) modules = new List<object>();
                modules.Add(rootDictionary);
                Modules = modules;
            }
        }

        /// <summary>
        /// Indicates whether at least one module section is declared under the given key.
        /// </summary>
        /// <param name="key">The module identifier to test.</param>
        public bool IsModuleConfigured(string key)
        {
            if (!string.IsNullOrEmpty(key) && !Modules.IsNullOrEmpty())
            {
                foreach (var configurationObj in Modules)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        if (rootDictionary.ContainsKey(key)) return true;
                    }
                    catch { }
                }
            }

            return false;
        }



        /// <summary>
        /// Flattens every declared processor section into a single map keyed by processor identifier, or null when no processors are configured.
        /// </summary>
		public Dictionary<object, object> GetProcessors()
        {
            if (!Processors.IsNullOrEmpty())
            {
                var configurations = new Dictionary<object, object>();

                foreach (var configurationObj in Processors)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        foreach (var entry in rootDictionary)
                        {
                            configurations.Add(entry.Key, entry.Value);
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }

        /// <summary>
        /// Returns every processor section declared under the given key as untyped objects, or null when the key is empty or no processors are configured.
        /// </summary>
        /// <param name="key">The processor identifier whose sections are requested.</param>
        public IEnumerable<object> GetProcessors(string key)
        {
            if (!string.IsNullOrEmpty(key) && !Processors.IsNullOrEmpty())
            {
                var configurations = new List<object>();

                foreach (var configurationObj in Processors)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        if (rootDictionary.ContainsKey(key))
                        {
                            var obj = rootDictionary[key];
                            if (obj != null)
                            {
                                configurations.Add(obj);
                            }
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }

        /// <summary>
        /// Returns every processor section declared under the given key, round-tripped through YAML to bind each to <typeparamref name="TConfiguration"/>. Sections that fail to bind are skipped.
        /// </summary>
        /// <typeparam name="TConfiguration">The strongly typed configuration the processor sections are bound to.</typeparam>
        /// <param name="key">The processor identifier whose sections are requested.</param>
        public IEnumerable<TConfiguration> GetProcessors<TConfiguration>(string key)
        {
            if (!string.IsNullOrEmpty(key) && !Processors.IsNullOrEmpty())
            {
                var configurations = new List<TConfiguration>();

                foreach (var configurationObj in Processors)
                {
                    try
                    {
                        var rootDictionary = (Dictionary<object, object>)configurationObj;
                        if (rootDictionary.ContainsKey(key))
                        {
                            var obj = rootDictionary[key];
                            if (obj != null)
                            {
                                var serializerBuilder = new SerializerBuilder();
                                serializerBuilder.WithNamingConvention(CamelCaseNamingConvention.Instance);

                                var serializer = serializerBuilder.Build();
                                var yaml = serializer.Serialize(obj);
                                if (yaml != null)
                                {
                                    var deserializerBuilder = new DeserializerBuilder();
                                    deserializerBuilder.WithNamingConvention(CamelCaseNamingConvention.Instance);
                                    deserializerBuilder.IgnoreUnmatchedProperties();

                                    var deserializer = deserializerBuilder.Build();

                                    var configuration = deserializer.Deserialize<TConfiguration>(yaml);
                                    if (configuration != null)
                                    {
                                        configurations.Add(configuration);
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }


        /// <summary>
        /// Binds an arbitrary configuration object to <typeparamref name="TConfiguration"/> by round-tripping it through YAML; when the input is null a default-constructed instance is returned. Yields the type default when binding fails.
        /// </summary>
        /// <typeparam name="TConfiguration">The target configuration type.</typeparam>
        /// <param name="controllerConfiguration">The loosely typed configuration object to convert, or null to construct a default.</param>
        public static TConfiguration GetConfiguration<TConfiguration>(object controllerConfiguration)
        {
            if (controllerConfiguration != null)
            {
                try
                {
                    var serializerBuilder = new SerializerBuilder();
                    serializerBuilder.WithNamingConvention(CamelCaseNamingConvention.Instance);

                    var serializer = serializerBuilder.Build();
                    var yaml = serializer.Serialize(controllerConfiguration);
                    if (yaml != null)
                    {
                        if (yaml.StartsWith("-")) yaml = " " + yaml.TrimStart('-');

                        var deserializerBuilder = new DeserializerBuilder();
                        deserializerBuilder.WithNamingConvention(CamelCaseNamingConvention.Instance);
                        deserializerBuilder.IgnoreUnmatchedProperties();

                        var deserializer = deserializerBuilder.Build();

                        return deserializer.Deserialize<TConfiguration>(yaml);
                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    var constructor = typeof(TConfiguration).GetConstructor(new Type[] { });
                    var configuration = constructor.Invoke(new object[] { });
                    if (configuration != null)
                    {
                        return (TConfiguration)configuration;
                    }
                }
                catch { }
            }

            return default;
        }
    }
}
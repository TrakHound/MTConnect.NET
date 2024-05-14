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


        [JsonPropertyName("modules")]
        public IEnumerable<object> Modules { get; set; }

        [JsonPropertyName("processors")]
        public IEnumerable<object> Processors { get; set; }


        public AgentApplicationConfiguration() : base()
        {
            Devices = null;
            ServiceName = null;
            ServiceAutoStart = true;
            MonitorConfigurationFiles = true;
            ConfigurationFileRestartInterval = 2;
        }


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
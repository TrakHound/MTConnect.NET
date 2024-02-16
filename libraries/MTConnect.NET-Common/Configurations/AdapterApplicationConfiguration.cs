// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent Application
    /// </summary>
    public class AdapterApplicationConfiguration : IAdapterApplicationConfiguration
    {
        private const string BackupDirectoryName = "backup";


        public const string JsonFilename = "adapter.config.json";

        public const string DefaultJsonFilename = "adapter.config.default.json";


        public const string YamlFilename = "adapter.config.yaml";

        public const string DefaultYamlFilename = "adapter.config.default.yaml";


        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        [JsonIgnore]
        [YamlIgnore]
        public string Path { get; set; }


        /// <summary>
        /// Get a unique identifier for the Adapter
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The Name or UUID of the Device to create a connection for
        /// </summary>
        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }

        /// <summary>
        /// The interval (in milliseconds) at which new data is read from the Data Source
        /// </summary>
        [JsonPropertyName("readInterval")]
        public int ReadInterval { get; set; }

        /// <summary>
        /// The interval (in milliseconds) at which new data is sent to the Agent
        /// </summary>
        [JsonPropertyName("writeInterval")]
        public int WriteInterval { get; set; }

        /// <summary>
        /// Determines whether to filter out duplicate data
        /// </summary>
        [JsonPropertyName("filterDuplicates")]
        public bool FilterDuplicates { get; set; }

        /// <summary>
        /// Determines whether to output Timestamps for each SHDR line
        /// </summary>
        [JsonPropertyName("outputTimestamps")]
        public bool OutputTimestamps { get; set; }

        /// <summary>
        /// Determines whether to send all changes to data or only most recent at the specified Interval
        /// </summary>
        [JsonPropertyName("enableBuffer")]
        public bool EnableBuffer { get; set; }


        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple Adapters to run as services on the same machine.
        /// </summary>
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        [JsonPropertyName("serviceAutoStart")]
        public bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Adapter will restart
        /// </summary>
        [JsonPropertyName("monitorConfigurationFiles")]
        public bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Adapter restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        [JsonPropertyName("configurationFileRestartInterval")]
        public int ConfigurationFileRestartInterval { get; set; }


        [JsonPropertyName("engine")]
        public Dictionary<string, object> Engine { get; set; }

        [JsonPropertyName("modules")]
        public IEnumerable<object> Modules { get; set; }



        public AdapterApplicationConfiguration()
        {
            Id = StringFunctions.RandomString(6);
            ReadInterval = 100;
            WriteInterval = 100;
            FilterDuplicates = true;
            OutputTimestamps = true;
            ServiceName = null;
            ServiceAutoStart = true;
            MonitorConfigurationFiles = true;
            ConfigurationFileRestartInterval = 2;
            EnableBuffer = false;
        }


        public object GetEngineProperty(string propertyName)
        {
            if (!Engine.IsNullOrEmpty() && !string.IsNullOrEmpty(propertyName))
            {
                if (Engine.TryGetValue(propertyName, out var propertyValue))
                {
                    return propertyValue;
                }
            }

            return null;
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
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }


        public static AdapterApplicationConfiguration Read(string path = null) => Read<AdapterApplicationConfiguration>(path);

        public static AdapterApplicationConfiguration ReadJson(string path = null) => ReadJson<AdapterApplicationConfiguration>(path);

        public static AdapterApplicationConfiguration ReadYaml(string path = null) => ReadYaml<AdapterApplicationConfiguration>(path);



        public static T Read<T>(string path = null) where T : AdapterApplicationConfiguration
        {
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);

            // Test for JSON Configuration File
            if (File.Exists(jsonPath)) return ReadJson<T>(jsonPath);
            else
            {
                var yamlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);

                return ReadYaml<T>(yamlPath);
            }
        }

        public static AdapterApplicationConfiguration Read(Type type, string path = null)
        {
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);

            // Test for JSON Configuration File
            if (File.Exists(jsonPath)) return ReadJson(type, jsonPath);
            else
            {
                var yamlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);

                return ReadYaml(type, yamlPath);
            }
        }


        public static T ReadJson<T>(string path = null) where T : AdapterApplicationConfiguration
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var options = new JsonSerializerOptions()
                        {
                            ReadCommentHandling = JsonCommentHandling.Skip
                        };

                        var configuration = JsonSerializer.Deserialize<T>(text, options);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }

        public static AdapterApplicationConfiguration ReadJson(Type type, string path = null)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var options = new JsonSerializerOptions()
                        {
                            ReadCommentHandling = JsonCommentHandling.Skip
                        };

                        var configuration = (AdapterApplicationConfiguration)JsonSerializer.Deserialize(text, type, options);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }


        public static T ReadYaml<T>(string path = null) where T : AdapterApplicationConfiguration
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build();

                        var configuration = deserializer.Deserialize<T>(text);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }

        public static AdapterApplicationConfiguration ReadYaml(Type type, string path = null)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build();

                        var configuration = (AdapterApplicationConfiguration)deserializer.Deserialize(text, type);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }



        public void SaveJson(string path = null, bool createBackup = true)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (path != null) configurationPath = path;

            if (createBackup)
            {
                // Create Backup of Configuration File
                var backupDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BackupDirectoryName);
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
                var backupFilename = System.IO.Path.ChangeExtension(UnixDateTime.Now.ToString(), ".backup.json");
                var backupPath = System.IO.Path.Combine(backupDir, backupFilename);
                if (File.Exists(configurationPath))
                {
                    File.Copy(configurationPath, backupPath);
                }
            }

            // Update ChangeToken
            ChangeToken = Guid.NewGuid().ToString();

            try
            {
                var json = JsonSerializer.Serialize(this);
                File.WriteAllText(configurationPath, json);
            }
            catch { }
        }

        public void SaveYaml(string path = null, bool createBackup = true)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);
            if (path != null) configurationPath = path;

            if (createBackup)
            {
                // Create Backup of Configuration File
                var backupDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BackupDirectoryName);
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
                var backupFilename = System.IO.Path.ChangeExtension(UnixDateTime.Now.ToString(), ".backup.yaml");
                var backupPath = System.IO.Path.Combine(backupDir, backupFilename);
                if (File.Exists(configurationPath))
                {
                    File.Copy(configurationPath, backupPath);
                }
            }

            // Update ChangeToken
            ChangeToken = Guid.NewGuid().ToString();

            try
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(this);
                File.WriteAllText(configurationPath, yaml);
            }
            catch { }
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

            return default;
        }
    }
}
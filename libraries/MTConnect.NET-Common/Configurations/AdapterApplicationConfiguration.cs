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


        /// <summary>
        /// The conventional file name for a user-supplied JSON adapter configuration.
        /// </summary>
        public const string JsonFilename = "adapter.config.json";

        /// <summary>
        /// The file name of the shipped JSON configuration used as a fallback when no user JSON configuration is present.
        /// </summary>
        public const string DefaultJsonFilename = "adapter.config.default.json";


        /// <summary>
        /// The conventional file name for a user-supplied YAML adapter configuration.
        /// </summary>
        public const string YamlFilename = "adapter.config.yaml";

        /// <summary>
        /// The file name of the shipped YAML configuration used as a fallback when no user YAML configuration is present.
        /// </summary>
        public const string DefaultYamlFilename = "adapter.config.default.yaml";


        /// <summary>
        /// An opaque token regenerated each time the configuration is saved, allowing consumers to detect that the configuration has changed.
        /// </summary>
        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        /// <summary>
        /// The file system path the configuration was loaded from; not serialized, and used as the default target when the configuration is saved.
        /// </summary>
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


        /// <summary>
        /// Free-form key/value settings consumed by the adapter engine; keys correspond to engine property names with loosely typed values.
        /// </summary>
        [JsonPropertyName("engine")]
        public Dictionary<string, object> Engine { get; set; }

        /// <summary>
        /// The raw, untyped module configuration sections declared for the adapter, each later resolved to a strongly typed configuration on demand.
        /// </summary>
        [JsonPropertyName("modules")]
        public IEnumerable<object> Modules { get; set; }



        /// <summary>
        /// Initializes a new instance with adapter defaults (random 6-character id, 100 ms read/write intervals, duplicate filtering and timestamp output enabled, auto-start service, configuration file monitoring on).
        /// </summary>
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


        /// <summary>
        /// Returns the value of the named engine property, or null when no engine settings are present or the property is undefined.
        /// </summary>
        /// <param name="propertyName">The engine property key to look up.</param>
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
        /// Returns every module section declared under the given key, round-tripped through YAML to bind each to <typeparamref name="TConfiguration"/>. Sections that fail to bind are skipped.
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
                        }
                    }
                    catch { }
                }

                return configurations;
            }

            return null;
        }


        /// <summary>
        /// Loads an <see cref="AdapterApplicationConfiguration"/>, preferring a JSON file in the base directory before falling back to YAML.
        /// </summary>
        /// <param name="path">An explicit configuration path, or null to auto-detect.</param>
        public static AdapterApplicationConfiguration Read(string path = null) => Read<AdapterApplicationConfiguration>(path);

        /// <summary>
        /// Loads an <see cref="AdapterApplicationConfiguration"/> from a JSON file.
        /// </summary>
        /// <param name="path">An explicit JSON path, or null to use the conventional file in the base directory.</param>
        public static AdapterApplicationConfiguration ReadJson(string path = null) => ReadJson<AdapterApplicationConfiguration>(path);

        /// <summary>
        /// Loads an <see cref="AdapterApplicationConfiguration"/> from a YAML file.
        /// </summary>
        /// <param name="path">An explicit YAML path, or null to use the conventional file in the base directory.</param>
        public static AdapterApplicationConfiguration ReadYaml(string path = null) => ReadYaml<AdapterApplicationConfiguration>(path);



        /// <summary>
        /// Loads a derived configuration, preferring the conventional JSON file in the base directory and falling back to YAML.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">Reserved; resolution always uses the conventional file names in the base directory.</param>
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

        /// <summary>
        /// Loads a configuration of the given runtime type, preferring the conventional JSON file in the base directory and falling back to YAML.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">Reserved; resolution always uses the conventional file names in the base directory.</param>
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


        /// <summary>
        /// Deserializes a derived configuration from a JSON file, ignoring comments and recording the source path. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit JSON path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
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

        /// <summary>
        /// Deserializes a configuration of the given runtime type from a JSON file, ignoring comments and recording the source path. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit JSON path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
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


        /// <summary>
        /// Deserializes a derived configuration from a YAML file using camelCase naming, ignoring unmatched properties and recording the source path. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit YAML path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
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

        /// <summary>
        /// Deserializes a configuration of the given runtime type from a YAML file using camelCase naming, ignoring unmatched properties and recording the source path. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit YAML path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
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



        /// <summary>
        /// Serializes this configuration to JSON and writes it to disk, regenerating <see cref="ChangeToken"/> and optionally backing up any existing file. Write failures are swallowed.
        /// </summary>
        /// <param name="path">The destination path; when null the conventional JSON file in the base directory is used.</param>
        /// <param name="createBackup">When true, an existing file is copied into a timestamped backup before being overwritten.</param>
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

        /// <summary>
        /// Serializes this configuration to YAML and writes it to disk, regenerating <see cref="ChangeToken"/> and optionally backing up any existing file. Write failures are swallowed.
        /// </summary>
        /// <param name="path">The destination path; when null the conventional YAML file in the base directory is used.</param>
        /// <param name="createBackup">When true, an existing file is copied into a timestamped backup before being overwritten.</param>
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


        /// <summary>
        /// Binds an arbitrary configuration object to <typeparamref name="TConfiguration"/> by round-tripping it through YAML, tolerating a leading sequence marker. Returns the type default when the input is null or cannot be bound.
        /// </summary>
        /// <typeparam name="TConfiguration">The target configuration type.</typeparam>
        /// <param name="controllerConfiguration">The loosely typed configuration object to convert.</param>
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
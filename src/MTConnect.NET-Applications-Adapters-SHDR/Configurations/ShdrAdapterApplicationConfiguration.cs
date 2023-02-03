// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
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
    public class ShdrAdapterApplicationConfiguration : IShdrAdapterApplicationConfiguration
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
        /// The TCP Port used for communication
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The heartbeat used to maintain a connection between the Adapter and the Agent
        /// </summary>
        [JsonPropertyName("heartbeat")]
        public int Heartbeat { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) to allow for a connection attempt to the Agent
        /// </summary>
        [JsonPropertyName("timeout")]
        public int Timeout { get; set; }

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
        /// Use multiline Assets
        /// </summary>
        [JsonPropertyName("multilineAssets")]
        public bool MultilineAssets { get; set; }

        /// <summary>
        /// Use multiline Devices
        /// </summary>
        [JsonPropertyName("multilineDevices")]
        public bool MultilineDevices { get; set; }

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



        public ShdrAdapterApplicationConfiguration() : base()
        {
            Id = StringFunctions.RandomString(6);
            Port = 7878;
            Heartbeat = 5000;
            Timeout = 5000;
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


        public static ShdrAdapterApplicationConfiguration Read(string path = null) => Read<ShdrAdapterApplicationConfiguration>(path);

        public static ShdrAdapterApplicationConfiguration ReadJson(string path = null) => ReadJson<ShdrAdapterApplicationConfiguration>(path);

        public static ShdrAdapterApplicationConfiguration ReadYaml(string path = null) => ReadYaml<ShdrAdapterApplicationConfiguration>(path);



        public static T Read<T>(string path = null) where T : ShdrAdapterApplicationConfiguration
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

        public static ShdrAdapterApplicationConfiguration Read(Type type, string path = null)
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


        public static T ReadJson<T>(string path = null) where T : ShdrAdapterApplicationConfiguration
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

        public static ShdrAdapterApplicationConfiguration ReadJson(Type type, string path = null)
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

                        var configuration = (ShdrAdapterApplicationConfiguration)JsonSerializer.Deserialize(text, type, options);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }


        public static T ReadYaml<T>(string path = null) where T : ShdrAdapterApplicationConfiguration
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

        public static ShdrAdapterApplicationConfiguration ReadYaml(Type type, string path = null)
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

                        var configuration = (ShdrAdapterApplicationConfiguration)deserializer.Deserialize(text, type);
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
    }
}
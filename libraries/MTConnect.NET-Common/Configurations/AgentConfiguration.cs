// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Agent
    /// </summary>
    public class AgentConfiguration : IAgentConfiguration
    {
        private const string BackupDirectoryName = "backup";


        /// <summary>
        /// The conventional file name for a user-supplied JSON agent configuration.
        /// </summary>
        public const string JsonFilename = "agent.config.json";

        /// <summary>
        /// The file name of the shipped JSON configuration used as a fallback when no user JSON configuration is present.
        /// </summary>
        public const string DefaultJsonFilename = "agent.config.default.json";


        /// <summary>
        /// The conventional file name for a user-supplied YAML agent configuration.
        /// </summary>
        public const string YamlFilename = "agent.config.yaml";

        /// <summary>
        /// The file name of the shipped YAML configuration used as a fallback when no user YAML configuration is present.
        /// </summary>
        public const string DefaultYamlFilename = "agent.config.default.yaml";


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
        /// The maximum number of Observations the agent can hold in its buffer
        /// </summary>
        [JsonPropertyName("observationBufferSize")]
        public uint ObservationBufferSize { get; set; }

        /// <summary>
        /// The maximum number of assets the agent can hold in its buffer
        /// </summary>
        [JsonPropertyName("assetBufferSize")]
        public uint AssetBufferSize { get; set; }


        /// <summary>
        /// Sets the TimeZone to use when timestamps are output from the Agent
        /// </summary>
        [JsonPropertyName("timezoneOutput")]
        public string TimeZoneOutput { get; set; }

        /// <summary>
        /// Overwrite timestamps with the agent time. 
        /// This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("ignoreTimestamps")]
        public bool IgnoreTimestamps { get; set; }

        /// <summary>
        /// Gets or Sets the default MTConnect version to output response documents for.
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public Version DefaultVersion { get; set; }

        /// <summary>
        /// The string form of <see cref="DefaultVersion"/> used for serialization; assigning a parseable version string updates <see cref="DefaultVersion"/>.
        /// </summary>
        [JsonPropertyName("defaultVersion")]
        [YamlMember(Alias = "defaultVersion")]
        public string DefaultVersionValue
        {
            get => DefaultVersion?.ToString();
            set
            {
                if (value != null)
                {
                    if (Version.TryParse(value, out var version))
                    {
                        DefaultVersion = version;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the default for Converting Units when adding Observations
        /// </summary>
        [JsonPropertyName("convertUnits")]
        public bool ConvertUnits { get; set; }

        /// <summary>
        /// Gets or Sets the default for Ignoring the case of Observation values
        /// </summary>
        [JsonPropertyName("ignoreObservationCase")]
        public bool IgnoreObservationCase { get; set; }

        /// <summary>
        /// Gets or Sets whether validation information is output
        /// </summary>
        [JsonPropertyName("enableValidation")]
        public bool EnableValidation { get; set; }

        /// <summary>
        /// Gets or Sets the default Input (Observation or Asset) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        [JsonPropertyName("inputValidationLevel")]
        public InputValidationLevel InputValidationLevel { get; set; }


        /// <summary>
        /// Gets or Sets whether the Agent Device is output
        /// </summary>
        [JsonPropertyName("enableAgentDevice")]
        public bool EnableAgentDevice { get; set; }

        /// <summary>
        /// Gets or Sets whether Metrics are captured (ex. ObserationUpdateRate, AssetUpdateRate)
        /// </summary>
        [JsonPropertyName("enableMetrics")]
        public bool EnableMetrics { get; set; }


        /// <summary>
        /// Initializes a new instance with the default agent settings (128K observation buffer, 1K asset buffer, latest MTConnect version, warning-level validation, unit conversion and metrics enabled).
        /// </summary>
        public AgentConfiguration()
        {
            ObservationBufferSize = 131072;
            AssetBufferSize = 1024;
            DefaultVersion = MTConnectVersions.Max;
            InputValidationLevel = InputValidationLevel.Warning;
            ConvertUnits = true;
            IgnoreObservationCase = false;
            EnableAgentDevice = true;
            EnableMetrics = true;
        }


        /// <summary>
        /// Loads an <see cref="AgentConfiguration"/>, auto-detecting JSON or YAML; see <see cref="Read{T}(string)"/> for the resolution rules.
        /// </summary>
        /// <param name="path">An explicit configuration path, or null to probe the base directory for the conventional files.</param>
        public static AgentConfiguration Read(string path = null) => Read<AgentConfiguration>(path);

        /// <summary>
        /// Loads an <see cref="AgentConfiguration"/> from a JSON file.
        /// </summary>
        /// <param name="path">An explicit JSON path, or null to use the conventional file in the base directory.</param>
        public static AgentConfiguration ReadJson(string path = null) => ReadJson<AgentConfiguration>(path);

        /// <summary>
        /// Loads an <see cref="AgentConfiguration"/> from a YAML file.
        /// </summary>
        /// <param name="path">An explicit YAML path, or null to use the conventional file in the base directory.</param>
        public static AgentConfiguration ReadYaml(string path = null) => ReadYaml<AgentConfiguration>(path);


        /// <summary>
        /// Loads a derived configuration, treating an explicit path as YAML and otherwise preferring a JSON file in the base directory before falling back to YAML. Returns null when no file can be read.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit configuration path (resolved relative to the base directory when not rooted), or null to auto-detect.</param>
        public static T Read<T>(string path = null) where T : AgentConfiguration
        {
            if (!string.IsNullOrEmpty(path))
            {
                var configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }

                return ReadYaml<T>(configurationPath);
            }
            else
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
        }

        /// <summary>
        /// Loads a configuration of the given runtime type, preferring a JSON file in the base directory before falling back to YAML.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit configuration path, or null to auto-detect.</param>
        public static AgentConfiguration Read(Type type, string path = null)
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
        /// Deserializes a derived configuration from a JSON file, ignoring comments. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit JSON path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static T ReadJson<T>(string path = null) where T : AgentConfiguration
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
        /// Deserializes a configuration of the given runtime type from a JSON file, ignoring comments. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit JSON path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static AgentConfiguration ReadJson(Type type, string path = null)
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

                        var configuration = (AgentConfiguration)JsonSerializer.Deserialize(text, type, options);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return null;
        }


        /// <summary>
        /// Deserializes a derived configuration from a YAML file using camelCase naming, ignoring unmatched properties. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit YAML path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static T ReadYaml<T>(string path = null) where T : AgentConfiguration
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
        /// Deserializes a configuration of the given runtime type from a YAML file using camelCase naming, ignoring unmatched properties. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit YAML path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static AgentConfiguration ReadYaml(Type type, string path = null)
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

                        var configuration = (AgentConfiguration)deserializer.Deserialize(text, type);
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
    }
}
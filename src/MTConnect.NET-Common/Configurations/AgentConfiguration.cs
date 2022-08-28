// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Agent
    /// </summary>
    public class AgentConfiguration : IAgentConfiguration
    {
        private const string BackupDirectoryName = "backup";


        public const string Filename = "agent.config.json";

        public const string DefaultFilename = "agent.config.default.json";


        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        [JsonIgnore]
        public string Path { get; set; }


        /// <summary>
        /// The maximum number of Observations the agent can hold in its buffer
        /// </summary>
        [JsonPropertyName("observationBufferSize")]
        public int ObservationBufferSize { get; set; }

        /// <summary>
        /// The maximum number of assets the agent can hold in its buffer
        /// </summary>
        [JsonPropertyName("assetBufferSize")]
        public int AssetBufferSize { get; set; }


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
        [JsonPropertyName("defaultVersion")]
        public Version DefaultVersion { get; set; }

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
        /// Gets or Sets the default Input (Observation or Asset) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict
        /// </summary>
        [JsonPropertyName("inputValidationLevel")]
        public InputValidationLevel InputValidationLevel { get; set; }


        /// <summary>
        /// Gets or Sets whether Metrics are captured (ex. ObserationUpdateRate, AssetUpdateRate)
        /// </summary>
        [JsonPropertyName("enableMetrics")]
        public bool EnableMetrics { get; set; }


        public AgentConfiguration()
        {
            ObservationBufferSize = 131072;
            AssetBufferSize = 1024;
            DefaultVersion = MTConnectVersions.Max;
            InputValidationLevel = InputValidationLevel.Warning;
            ConvertUnits = true;
            IgnoreObservationCase = true;
            EnableMetrics = true;
        }


        public static AgentConfiguration Read(string path = null) => Read<AgentConfiguration>(path);

        public static T Read<T>(string path = null) where T : AgentConfiguration
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
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

        public void Save(string path = null, bool createBackup = true)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
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
    }
}

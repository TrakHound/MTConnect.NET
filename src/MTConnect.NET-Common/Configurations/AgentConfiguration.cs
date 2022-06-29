// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Agent
    /// </summary>
    public class AgentConfiguration
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
        /// The XML file to load that specifies the devices and is supplied as the result of a probe request. 
        /// If the key is not found the defaults are tried.
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
        /// Gets or Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict
        /// </summary>
        [JsonPropertyName("validationLevel")]
        public ValidationLevel ValidationLevel { get; set; }


        /// <summary>
        /// Gets or Sets the default response document indendation
        /// </summary>
        [JsonPropertyName("indentOutput")]
        public bool IndentOutput { get; set; }

        /// <summary>
        /// Gets or Sets the default response document comments output. Comments contain descriptions from the MTConnect standard
        /// </summary>
        [JsonPropertyName("outputComments")]
        public bool OutputComments { get; set; }


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



        [JsonPropertyName("devicesNamespaces")]
        public List<NamespaceConfiguration> DevicesNamespaces { get; set; }

        [JsonPropertyName("streamsNamespaces")]
        public List<NamespaceConfiguration> StreamsNamespaces { get; set; }

        [JsonPropertyName("assetsNamespaces")]
        public List<NamespaceConfiguration> AssetsNamespaces { get; set; }

        [JsonPropertyName("errorNamespaces")]
        public List<NamespaceConfiguration> ErrorNamespaces { get; set; }


        [JsonPropertyName("devicesStyle")]
        public StyleConfiguration DevicesStyle { get; set; }

        [JsonPropertyName("streamsStyle")]
        public StyleConfiguration StreamsStyle { get; set; }

        [JsonPropertyName("assetsStyle")]
        public StyleConfiguration AssetsStyle { get; set; }

        [JsonPropertyName("errorStyle")]
        public StyleConfiguration ErrorStyle { get; set; }



        public AgentConfiguration()
        {
            ObservationBufferSize = 131072;
            AssetBufferSize = 1024;
            Devices = "devices.xml";
            ServiceName = null;
            ServiceAutoStart = true;
            DefaultVersion = MTConnectVersions.Max;
            ValidationLevel = ValidationLevel.Ignore;
            ConvertUnits = true;
            IgnoreObservationCase = true;
            MonitorConfigurationFiles = true;
            ConfigurationFileRestartInterval = 2;
            IndentOutput = true;
            OutputComments = false;
        }


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
                        var configuration = JsonSerializer.Deserialize<T>(text);
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

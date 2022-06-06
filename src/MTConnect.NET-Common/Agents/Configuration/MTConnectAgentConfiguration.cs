// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Agents.Configuration
{
    public class MTConnectAgentConfiguration
    {
        private const string SectionName = "MTConnectAgent";
        private const string BackupDirectoryName = "backup";


        public const string Filename = "agent.config.json";

        public const string DefaultFilename = "agent.config.default.json";


        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        [JsonIgnore]
        public string Path { get; set; }


        /// <summary>
        /// The 2^X number of slots available in the circular buffer for samples, events, and conditions.
        /// </summary>
        [JsonPropertyName("bufferSize")]
        public int BufferSize { get; set; }

        /// <summary>
        /// The maximum number of assets the agent can hold in its buffer. The number is the actual count, not an exponent.
        /// </summary>
        [JsonPropertyName("maxAssets")]
        public int MaxAssets { get; set; }

        /// <summary>
        /// The frequency checkpoints are created in the stream. 
        /// This is used for current with the at argument. 
        /// This is an advanced configuration item and should not be changed unless you understand the internal workings of the agent.
        /// </summary>
        [JsonPropertyName("checkpointFrequency")]
        public int CheckpointFrequency { get; set; }

        /// <summary>
        /// The XML file to load that specifies the devices and is supplied as the result of a probe request. 
        /// If the key is not found the defaults are tried.
        /// </summary>
        [JsonPropertyName("devices")]
        public string Devices { get; set; }

        /// <summary>
        /// UNIX only. The full path of the file that contains the process id of the daemon. This is not supported in Windows.
        /// </summary>
        [JsonPropertyName("pidFile")]
        public string PidFile { get; set; }

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
        /// The port number the agent binds to for requests.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The server IP Address to bind to. Can be used to select the interface in IPV4 or IPV6.
        /// </summary>
        [JsonPropertyName("serverIp")]
        public string ServerIp { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST of data item values or assets.
        /// </summary>
        [JsonPropertyName("allowPut")]
        public bool AllowPut { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST from a specific host or list of hosts. 
        /// Lists are comma (,) separated and the host names will be validated by translating them into IP addresses.
        /// </summary>
        [JsonPropertyName("allowPutFrom")]
        public List<string> AllowPutFrom { get; set; }

        /// <summary>
        /// The default length of time an adapter can be silent before it is disconnected. 
        /// This is only for legacy adapters that do not support heartbeats.
        /// </summary>
        [JsonPropertyName("legacyTimeout")]
        public int LegacyTimeout { get; set; }

        /// <summary>
        /// The amount of time between adapter reconnection attempts. 
        /// This is useful for implementation of high performance adapters where availability needs to be tracked in near-real-time. 
        /// Time is specified in milliseconds (ms).
        /// </summary>
        [JsonPropertyName("reconnectInterval")]
        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Overwrite timestamps with the agent time. 
        /// This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("ignoreTimestamps")]
        public bool IgnoreTimestamps { get; set; }

        /// <summary>
        /// Do not overwrite the UUID with the UUID from the adapter, preserve the UUID in the Devices.xml file. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("preserveUuid")]
        public bool PreserveUuid { get; set; }

        /// <summary>
        /// Change the schema version to a different version number.
        /// </summary>
        [JsonPropertyName("schemaVersion")]
        public Version DefaultVersion { get; set; }

        /// <summary>
        /// Global default for data item units conversion in the agent. Assumes the adapter has already done unit conversion.
        /// </summary>
        [JsonPropertyName("conversionRequired")]
        public bool ConversionRequired { get; set; }

        /// <summary>
        /// Always converts the value of the data items to upper case.
        /// </summary>
        [JsonPropertyName("upcaseDataItemValue")]
        public bool UpcaseDataItemValue { get; set; }

        /// <summary>
        /// Monitor agent.cfg and Devices.xml files and restart agent if they change.
        /// </summary>
        [JsonPropertyName("monitorConfigFiles")]
        public bool MonitorConfigFiles { get; set; }

        /// <summary>
        /// The minimum age of a config file before an agent reload is triggered (seconds).
        /// </summary>
        [JsonPropertyName("minimumConfigReloadAge")]
        public int MinimumConfigReloadAge { get; set; }

        /// <summary>
        /// Pretty print the output with indententation
        /// </summary>
        [JsonPropertyName("pretty")]
        public bool Pretty { get; set; }

        /// <summary>
        /// Specifies the SHDR protocol version used by the adapter. 
        /// When greater than one (1), allows multiple complex observations, like Condition and Message on the same line. 
        /// If it equials one (1), then any observation requiring more than a key/value pair need to be on separate lines. 
        /// This is the default for all adapters.
        /// </summary>
        [JsonPropertyName("shdrVersion")]
        public string ShdrVersion { get; set; }

        /// <summary>
        /// Suppress the Adapter IP Address and port when creating the Agent Device ids and names for 1.7. This applies to all adapters.
        /// </summary>
        [JsonPropertyName("suppressIpAddress")]
        public bool SuppressIpAddress { get; set; }

        /// <summary>
        /// Adapters begins a list of device blocks. If the Adapters are not specified and the Devices file only contains one device, 
        /// a default device entry will be created with an adapter located on the localhost and port 7878 associated with the device in the devices file.
        /// </summary>
        [JsonPropertyName("adapters")]
        public List<AdapterConfiguration> Adapters { get; set; }


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



        public MTConnectAgentConfiguration()
        {
            BufferSize = 131072;
            MaxAssets = 1024;
            CheckpointFrequency = 1000;
            Devices = "devices.xml";
            PidFile = "agent.pid";
            ServiceName = null;
            ServiceAutoStart = true;
            ServerIp = "127.0.0.1";
            Port = 5000;
            AllowPut = false;
            AllowPutFrom = null;
            LegacyTimeout = 600;
            ReconnectInterval = 10000;
            IgnoreTimestamps = false;
            PreserveUuid = true;
            DefaultVersion = MTConnectVersions.Max;
            ConversionRequired = true;
            UpcaseDataItemValue = true;
            MonitorConfigFiles = true;
            MinimumConfigReloadAge = 2;
            Pretty = true;
            ShdrVersion = "1";
            SuppressIpAddress = false;
        }


        public static MTConnectAgentConfiguration Read(string path = null)
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
                        var configuration = JsonSerializer.Deserialize<MTConnectAgentConfiguration>(text);
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

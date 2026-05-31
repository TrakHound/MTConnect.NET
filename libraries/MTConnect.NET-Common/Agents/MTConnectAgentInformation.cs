// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Agents
{
    /// <summary>
    /// Persistent identity and buffer-index state for an MTConnect Agent that is saved to and restored from a JSON file so the Agent retains its UUID, InstanceId, and buffer slot assignments across restarts.
    /// </summary>
    public class MTConnectAgentInformation
    {
        /// <summary>
        /// The default file name used to persist the Agent information.
        /// </summary>
        public const string Filename = "agent.information.json";


        /// <summary>
        /// A token regenerated on every save, used to detect when the persisted information has changed.
        /// </summary>
        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        /// <summary>
        /// The stable unique identifier of the Agent.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// The InstanceId reported by the Agent, identifying the lifetime of its buffer.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        /// <summary>
        /// The timestamp, in Unix ticks, of the most recent change to the Device model.
        /// </summary>
        [JsonPropertyName("deviceModelChangeTime")]
        public long DeviceModelChangeTime { get; set; }

        /// <summary>
        /// The persisted lookup of Device UUID to its assigned buffer index, restored so buffer slots remain stable across restarts.
        /// </summary>
        [JsonPropertyName("devices")]
        public Dictionary<string, int> DeviceIndexes { get; set; }

        /// <summary>
        /// The persisted lookup of DataItem key to its assigned buffer index, restored so buffer slots remain stable across restarts.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public Dictionary<string, int> DataItemIndexes { get; set; }


        /// <summary>
        /// Initializes a new instance with a freshly generated UUID and an InstanceId set to the current time.
        /// </summary>
        public MTConnectAgentInformation()
        {
            Uuid = Guid.NewGuid().ToString();
            InstanceId = (ulong)UnixDateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance with the given identity values.
        /// </summary>
        /// <param name="uuid">The stable unique identifier of the Agent.</param>
        /// <param name="instanceId">The InstanceId identifying the lifetime of the Agent buffer.</param>
        /// <param name="deviceModelChangeTime">The timestamp, in Unix ticks, of the most recent Device model change.</param>
        public MTConnectAgentInformation(string uuid, ulong instanceId = 0, long deviceModelChangeTime = 0)
        {
            Uuid = uuid;
            InstanceId = instanceId;
            DeviceModelChangeTime = deviceModelChangeTime;
        }


        /// <summary>
        /// Read persisted Agent information from a JSON file.
        /// </summary>
        /// <param name="path">An optional file path; when omitted or relative, it is resolved against the application base directory and defaults to <see cref="Filename"/>.</param>
        /// <returns>The deserialized Agent information, or <c>null</c> if the file is missing, empty, or cannot be read.</returns>
        public static MTConnectAgentInformation Read(string path = null)
        {
            var configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!Path.IsPathRooted(configurationPath))
                {
                    configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        return JsonSerializer.Deserialize<MTConnectAgentInformation>(text);
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Persist this Agent information to a JSON file, regenerating <see cref="ChangeToken"/> before writing.
        /// </summary>
        /// <param name="path">An optional file path; when omitted, the file is written to the application base directory as <see cref="Filename"/>.</param>
        public void Save(string path = null)
        {
            var configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
            if (path != null) configurationPath = path;

            // Update ChangeToken
            ChangeToken = Guid.NewGuid().ToString();

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(configurationPath, json);
            }
            catch { }
        }
    }
}
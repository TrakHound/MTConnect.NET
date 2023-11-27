// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Agents
{
    public class MTConnectAgentInformation
    {
        public const string Filename = "agent.information.json";


        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        [JsonPropertyName("deviceModelChangeTime")]
        public long DeviceModelChangeTime { get; set; }

        [JsonPropertyName("devices")]
        public Dictionary<string, int> DeviceIndexes { get; set; }

        [JsonPropertyName("dataItems")]
        public Dictionary<string, int> DataItemIndexes { get; set; }


        public MTConnectAgentInformation()
        {
            Uuid = Guid.NewGuid().ToString();
            InstanceId = UnixDateTime.Now;
        }

        public MTConnectAgentInformation(string uuid, long instanceId = 0, long deviceModelChangeTime = 0)
        {
            Uuid = uuid;
            InstanceId = instanceId;
            DeviceModelChangeTime = deviceModelChangeTime;
        }


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
// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Agents
{
    /// <summary>
    /// Contains information about a client connection such as the Agent Instance ID and the Last Sequence read
    /// </summary>
    public class MTConnectClientInformation
    {
        public const string FilenamePrefix = "client.information.";
        public const string FilenameExtension = ".json";


        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }

        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        [JsonPropertyName("lastSequence")]
        public long LastSequence { get; set; }


        public MTConnectClientInformation() { }

        public MTConnectClientInformation(string deviceUuid)
        {
            DeviceKey = deviceUuid;
        }

        public MTConnectClientInformation(string deviceUuid, long instanceId, long lastSequence = 0)
        {
            DeviceKey = deviceUuid;
            InstanceId = instanceId;
            LastSequence = lastSequence;
        }


        public static MTConnectClientInformation Read(string deviceKey, string path = null)
        {
            var configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clients", GenerateFilename(deviceKey));
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
                        return JsonSerializer.Deserialize<MTConnectClientInformation>(text);
                    }
                }
                catch { }
            }

            return null;
        }

        public void Save(string path = null)
        {
            // Update ChangeToken
            ChangeToken = Guid.NewGuid().ToString();

            try
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clients");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var configurationPath = Path.Combine(dir, GenerateFilename(DeviceKey));
                if (path != null) configurationPath = path;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(configurationPath, json);
            }
            catch { }
        }


        private static string GenerateFilename(string deviceKey)
        {
            if (!string.IsNullOrEmpty(deviceKey))
            {
                return $"{FilenamePrefix}{deviceKey}{FilenameExtension}";
            }

            return null;
        }
    }
}

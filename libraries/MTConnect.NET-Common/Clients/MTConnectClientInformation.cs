// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Clients
{
    /// <summary>
    /// Contains information about a client connection such as the Agent Instance ID and the Last Sequence read
    /// </summary>
    public class MTConnectClientInformation
    {
        /// <summary>
        /// The filename prefix ("client.information.") used to persist this state per device under the "clients" directory.
        /// </summary>
        public const string FilenamePrefix = "client.information.";

        /// <summary>
        /// The filename extension (".json") used for persisted client information.
        /// </summary>
        public const string FilenameExtension = ".json";


        /// <summary>
        /// A token regenerated on every save, used to detect that the persisted state has changed since it was last loaded.
        /// </summary>
        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        /// <summary>
        /// The device key (UUID) this connection state belongs to.
        /// </summary>
        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }

        /// <summary>
        /// The Agent buffer instance the last sequence was read against; a change here signals the Agent restarted and the sequence is no longer valid.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        /// <summary>
        /// The sequence number of the last observation successfully read, so a reconnecting client can resume without gaps or duplicates.
        /// </summary>
        [JsonPropertyName("lastSequence")]
        public long LastSequence { get; set; }


        /// <summary>
        /// Initializes empty client information, typically for deserialization.
        /// </summary>
        public MTConnectClientInformation() { }

        /// <summary>
        /// Initializes client information bound to the given device, with no instance or sequence recorded yet.
        /// </summary>
        /// <param name="deviceUuid">The device UUID this state tracks.</param>
        public MTConnectClientInformation(string deviceUuid)
        {
            DeviceKey = deviceUuid;
        }

        /// <summary>
        /// Initializes client information bound to the given device with a known Agent instance and resume sequence.
        /// </summary>
        /// <param name="deviceUuid">The device UUID this state tracks.</param>
        /// <param name="instanceId">The Agent buffer instance the sequence was read against.</param>
        /// <param name="lastSequence">The last sequence number successfully read.</param>
        public MTConnectClientInformation(string deviceUuid, long instanceId, long lastSequence = 0)
        {
            DeviceKey = deviceUuid;
            InstanceId = instanceId;
            LastSequence = lastSequence;
        }


        /// <summary>
        /// Loads persisted client information for the given device, defaulting to the per-device file under the "clients" directory or an explicit (optionally relative) path; returns null when the file is missing, empty, or cannot be read.
        /// </summary>
        /// <param name="deviceKey">The device key whose state to load.</param>
        /// <param name="path">An optional override path; relative paths are resolved against the application base directory.</param>
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

        /// <summary>
        /// Persists this client information as indented JSON, regenerating <see cref="ChangeToken"/> first; writes to the per-device file under the "clients" directory (created if absent) unless an explicit path is given. Failures are swallowed.
        /// </summary>
        /// <param name="path">An optional override path for the saved file.</param>
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
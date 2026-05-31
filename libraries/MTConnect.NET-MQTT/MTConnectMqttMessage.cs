// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// Factory helpers that translate MTConnect entities (agent metadata, devices, assets, and
    /// observations) into the matching <see cref="MqttApplicationMessage"/> objects with the
    /// canonical topic layout used by the MTConnect MQTT broker. The methods cover the
    /// <c>MTConnect/Agents/{uuid}/Information</c> message, the heartbeat timestamp, device and
    /// asset publishes, and individual observation publishes in either flat or hierarchy
    /// topic format.
    /// </summary>
    public static class MTConnectMqttMessage
    {
        private static MqttApplicationMessage CreateMessage(string topic, string payload, bool retain = false)
        {
            try
            {
                var messageBuilder = new MqttApplicationMessageBuilder();
                messageBuilder.WithTopic(topic);
                messageBuilder.WithPayload(payload);
                messageBuilder.WithRetainFlag(retain);
                return messageBuilder.Build();
            }
            catch { }

            return null;
        }

        private static MqttApplicationMessage CreateMessage(string topic, Stream payload, bool retain = false)
        {
            try
            {
                var messageBuilder = new MqttApplicationMessageBuilder();
                messageBuilder.WithTopic(topic);
                messageBuilder.WithPayload(payload);
                messageBuilder.WithRetainFlag(retain);
                return messageBuilder.Build();
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Builds the <c>MTConnect/Agents/{uuid}/Information</c> MQTT message that announces the
        /// agent's identity (UUID, instance id, version, sender), its
        /// <c>DeviceModelChangeTime</c>, the heartbeat interval, the list of observation
        /// intervals it publishes on, and the UUIDs of the devices it owns. Returns a single
        /// message wrapped in a list so the caller can extend it without changing the signature.
        /// </summary>
        /// <param name="agent">The agent to describe; null returns null.</param>
        /// <param name="observationIntervals">The interval-bucket lengths (ms) on which the agent publishes batched observation topics.</param>
        /// <param name="heartbeat">Heartbeat interval in milliseconds; consumers use this to time out the agent if no <c>HeartbeatTimestamp</c> updates arrive.</param>
        /// <param name="retain">If true, the message is published with the MQTT retain flag so newly connecting clients get the current information immediately.</param>
        public static IEnumerable<MqttApplicationMessage> Create(IMTConnectAgent agent, IEnumerable<int> observationIntervals, int heartbeat, bool retain = false)
        {
            if (agent != null)
            {
                var messages = new List<MqttApplicationMessage>();

                try
                {
                    var information = new MTConnectMqttAgentInformation();
                    information.Uuid = agent.Uuid;
                    information.InstanceId = agent.InstanceId;
                    information.Version = agent.Version;
                    information.Sender = agent.Sender;
                    information.DeviceModelChangeTime = agent.DeviceModelChangeTime;
                    information.HeartbeatInterval = heartbeat;

                    // Set Observation Intervals
                    information.ObservationIntervals = observationIntervals;

                    // Set Devices (list of Device UUID's associated with the Agent)
                    var devices = agent.GetDevices();
                    if (!devices.IsNullOrEmpty())
                    {
                        information.Devices = devices.Select(o => o.Uuid);
                    }

                    var topic = $"MTConnect/Agents/{agent.Uuid}/Information";
                    var json = JsonSerializer.Serialize(information, new JsonSerializerOptions { WriteIndented = true });

                    messages.Add(CreateMessage(topic, json, retain));
                }
                catch { }



                //// UUID
                //var topic = $"MTConnect/Agents/{agent.Uuid}/UUID";
                //messages.Add(CreateMessage(topic, agent.Uuid, retain));

                //// InstanceId
                //topic = $"MTConnect/Agents/{agent.Uuid}/InstanceId";
                //messages.Add(CreateMessage(topic, agent.InstanceId.ToString(), retain));

                //// Agent Application Version
                //topic = $"MTConnect/Agents/{agent.Uuid}/Version";
                //messages.Add(CreateMessage(topic, agent.Version.ToString(), retain));

                //// Sender
                //topic = $"MTConnect/Agents/{agent.Uuid}/Sender";
                //messages.Add(CreateMessage(topic, agent.Sender, retain));

                //// DeviceModelChangeTime
                //topic = $"MTConnect/Agents/{agent.Uuid}/DeviceModelChangeTime";
                //messages.Add(CreateMessage(topic, agent.DeviceModelChangeTime.ToString("o"), retain));

                //// Heartbeat Interval
                //topic = $"MTConnect/Agents/{agent.Uuid}/HeartbeatInterval";
                //messages.Add(CreateMessage(topic, heartbeat.ToString(), true));

                return messages;
            }

            return null;
        }

        //public static IEnumerable<MqttApplicationMessage> Create(IMTConnectAgent agent, int heartbeat, bool retain = false)
        //{
        //    if (agent != null)
        //    {
        //        var messages = new List<MqttApplicationMessage>();

        //        // UUID
        //        var topic = $"MTConnect/Agents/{agent.Uuid}/UUID";
        //        messages.Add(CreateMessage(topic, agent.Uuid, retain));

        //        // InstanceId
        //        topic = $"MTConnect/Agents/{agent.Uuid}/InstanceId";
        //        messages.Add(CreateMessage(topic, agent.InstanceId.ToString(), retain));

        //        // Agent Application Version
        //        topic = $"MTConnect/Agents/{agent.Uuid}/Version";
        //        messages.Add(CreateMessage(topic, agent.Version.ToString(), retain));

        //        // Sender
        //        topic = $"MTConnect/Agents/{agent.Uuid}/Sender";
        //        messages.Add(CreateMessage(topic, agent.Sender, retain));

        //        // DeviceModelChangeTime
        //        topic = $"MTConnect/Agents/{agent.Uuid}/DeviceModelChangeTime";
        //        messages.Add(CreateMessage(topic, agent.DeviceModelChangeTime.ToString("o"), retain));

        //        // Heartbeat Interval
        //        topic = $"MTConnect/Agents/{agent.Uuid}/HeartbeatInterval";
        //        messages.Add(CreateMessage(topic, heartbeat.ToString(), true));

        //        return messages;
        //    }

        //    return null;
        //}

        /// <summary>
        /// Builds the heartbeat publish, an <c>MTConnect/Agents/{uuid}/HeartbeatTimestamp</c>
        /// message carrying the current Unix-epoch timestamp in milliseconds. The agent is
        /// expected to emit this on every heartbeat tick so consumers can detect a stalled agent.
        /// </summary>
        /// <param name="agent">The agent owning the heartbeat; null returns null.</param>
        /// <param name="timestamp">The current timestamp in Windows ticks (100 ns units); converted to milliseconds for the payload.</param>
        public static IEnumerable<MqttApplicationMessage> CreateHeartbeat(IMTConnectAgent agent, long timestamp)
        {
            if (agent != null)
            {
                var messages = new List<MqttApplicationMessage>();

                // Conver to Milliseconds (from Ticks)
                var ts = timestamp / 10000;

                // Heartbeat Timestamp
                var topic = $"MTConnect/Agents/{agent.Uuid}/HeartbeatTimestamp";
                messages.Add(CreateMessage(topic, ts.ToString()));

                return messages;
            }

            return null;
        }

        /// <summary>
        /// Builds the device-publish messages: an <c>MTConnect/Devices/{deviceUuid}/AgentUuid</c>
        /// retained pointer back to the owning agent, and an
        /// <c>MTConnect/Devices/{deviceUuid}/Device</c> message containing the serialised device
        /// model in <paramref name="documentFormatterId"/>.
        /// </summary>
        /// <param name="device">The device to publish; null or empty <paramref name="documentFormatterId"/> returns null.</param>
        /// <param name="agentUuid">The UUID of the agent the device belongs to; published as the back-pointer payload.</param>
        /// <param name="documentFormatterId">The document format used to serialise the device model (defaults to XML).</param>
        /// <param name="retain">If true, the device message itself is published with the MQTT retain flag (the agent UUID pointer is always retained).</param>
        public static IEnumerable<MqttApplicationMessage> Create(IDevice device, string agentUuid, string documentFormatterId = DocumentFormat.XML, bool retain = false)
        {
            if (device != null && !string.IsNullOrEmpty(documentFormatterId))
            {
                var messages = new List<MqttApplicationMessage>();

                // Create Agent UUID Message
                var topic = $"MTConnect/Devices/{device.Uuid}/AgentUuid";
                messages.Add(CreateMessage(topic, agentUuid, true));

                //// Create DeviceIndex UUID Message
                //topic = $"MTConnect/DeviceIndex/{device.Uuid}";
                //messages.Add(CreateMessage(topic, device.Uuid, true));

                //// Create DeviceIndex ID Message
                //topic = $"MTConnect/DeviceIndex/{device.Id}";
                //messages.Add(CreateMessage(topic, device.Uuid, true));

                //// Create DeviceIndex Name Message
                //topic = $"MTConnect/DeviceIndex/{device.Name}";
                //messages.Add(CreateMessage(topic, device.Uuid, true));

                // Create Device Message
                topic = $"MTConnect/Devices/{device.Uuid}/Device";
                var formatResponse = Formatters.EntityFormatter.Format(documentFormatterId, device);
                if (formatResponse.Success && formatResponse.Content != null)
                {
                    messages.Add(CreateMessage(topic, formatResponse.Content, retain));
                }

                return messages;
            }

            return null;
        }

        /// <summary>
        /// Builds the two asset publishes for an MTConnect asset: one at
        /// <c>MTConnect/Assets/{type}/{assetId}</c> (asset-type-scoped) and one at
        /// <c>MTConnect/Devices/{deviceUuid}/Assets/{type}/{assetId}</c> (device-scoped). Both
        /// carry the same serialised payload in the requested <paramref name="documentFormatterId"/>.
        /// </summary>
        /// <param name="asset">The asset to publish; null returns null.</param>
        /// <param name="documentFormatterId">The document format used to serialise the asset (defaults to XML).</param>
        /// <param name="retain">If true, both messages are published with the MQTT retain flag.</param>
        public static IEnumerable<MqttApplicationMessage> Create(IAsset asset, string documentFormatterId = DocumentFormat.XML, bool retain = false)
        {
            if (asset != null)
            {
                var messages = new List<MqttApplicationMessage>();

                var formatResponse = Formatters.EntityFormatter.Format(documentFormatterId, asset);
                if (formatResponse.Success && formatResponse.Content != null)
                {
                    messages.Add(CreateMessage($"MTConnect/Assets/{asset.Type}/{asset.AssetId}", formatResponse.Content, retain));
                    messages.Add(CreateMessage($"MTConnect/Devices/{asset.DeviceUuid}/Assets/{asset.Type}/{asset.AssetId}", formatResponse.Content, retain));
                }

                return messages;
            }

            return null;
        }

        /// <summary>
        /// Builds the publish for a single MTConnect observation. The topic is computed by
        /// <see cref="MTConnectMqttFormat"/> from the data item type, sub-type, category, and
        /// container; the payload is the observation serialised with the entity formatter,
        /// always including category and instance-id options because MQTT consumers cannot
        /// otherwise recover those properties from the topic alone.
        /// </summary>
        /// <param name="observation">The observation; null returns null. The observation must have a device UUID, a data item with a container, and at least one value.</param>
        /// <param name="format">Flat or hierarchy topic layout.</param>
        /// <param name="documentFormatterId">The document format used to serialise the observation (defaults to XML).</param>
        /// <param name="retain">If true, the publish is retained (used for the last-known-value channel).</param>
        /// <param name="interval">When non-zero, identifies the observation-interval bucket the publish belongs to and is reflected in the topic.</param>
        public static MqttApplicationMessage Create(IObservation observation, MTConnectMqttFormat format, string documentFormatterId = DocumentFormat.XML, bool retain = false, int interval = 0)
        {
            if (observation != null && !string.IsNullOrEmpty(observation.DeviceUuid) && observation.DataItem != null && observation.DataItem.Container != null && !observation.Values.IsNullOrEmpty())
            {
                var topic = CreateTopic(observation, format, interval);

                var formatOptions = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("categoryOutput", "true"),
                    new KeyValuePair<string, string>("instanceIdOutput", "true")
                };

                var formatResponse = Formatters.EntityFormatter.Format(documentFormatterId, observation, formatOptions);
                if (formatResponse.Success && formatResponse.Content != null)
                {
                    return CreateMessage(topic, formatResponse.Content, retain);
                }
            }

            return null;
        }

        /// <summary>
        /// Builds a publish for a batch of observations that all share the same data item (and
        /// therefore the same MQTT topic). The first observation is inspected to derive the
        /// topic; the payload is the formatter's serialisation of the whole batch.
        /// </summary>
        /// <param name="observations">The observation batch; null or empty returns null.</param>
        /// <param name="format">Flat or hierarchy topic layout.</param>
        /// <param name="documentFormatterId">The document format used to serialise the observations (defaults to XML).</param>
        /// <param name="retain">If true, the publish is retained.</param>
        /// <param name="interval">When non-zero, identifies the observation-interval bucket the publish belongs to.</param>
        public static MqttApplicationMessage Create(IEnumerable<IObservation> observations, MTConnectMqttFormat format, string documentFormatterId = DocumentFormat.XML, bool retain = false, int interval = 0)
        {
            if (!observations.IsNullOrEmpty())
            {
                // Take the first Observation (should all be for the same DataItem)
                var firstObservation = observations.FirstOrDefault();
                if (firstObservation != null)
                {
                    var topic = CreateTopic(firstObservation, format, interval);

                    var formatOptions = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("categoryOutput", "true"),
                        new KeyValuePair<string, string>("instanceIdOutput", "true")
                    };

                    var formatResponse = Formatters.EntityFormatter.Format(documentFormatterId, observations, formatOptions);
                    if (formatResponse.Success && formatResponse.Content != null)
                    {
                        return CreateMessage(topic, formatResponse.Content, retain);
                    }
                }
            }

            return null;
        }


        private static string CreateTopic(IObservation observation, MTConnectMqttFormat format, int interval = 0)
        {
            if (observation != null)
            {
                var type = DataItem.GetPascalCaseType(observation.DataItem.Type);
                var subtype = DataItem.GetPascalCaseType(observation.DataItem.SubType);

                var category = observation.Category.ToString().ToTitleCase() + "s";

                // Add Prefixes
                var prefixes = new List<string>();
                prefixes.Add("MTConnect");
                prefixes.Add("Devices");
                prefixes.Add(observation.DeviceUuid);

                if (interval > 0)
                {
                    prefixes.Add($"Observations[{interval}]");
                }
                else
                {
                    prefixes.Add("Observations");
                }


                var paths = new List<string>();

                // Add Container
                paths.Add(RemoveNamespacePrefix(observation.DataItem.Container.Type));
                paths.Add(observation.DataItem.Container.Id);
                paths.Add(category);

                // Add Type
                paths.Add(RemoveNamespacePrefix(type));

                // Add SubType
                if (!string.IsNullOrEmpty(subtype))
                {
                    paths.Add("SubTypes");
                    paths.Add(RemoveNamespacePrefix(subtype));
                }

                switch (format)
                {
                    case MTConnectMqttFormat.Hierarchy:

                        return string.Join("/", new string[]
                        {
                            string.Join("/", prefixes),
                            string.Join("/", paths),
                            observation.DataItemId
                        });

                    default:
                        return string.Join("/", new string[]
{
                            string.Join("/", prefixes),
                            observation.DataItemId
});
                }
            }

            return null;
        }


        private static string RemoveNamespacePrefix(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Contains(':'))
                {
                    return type.Substring(type.IndexOf(':') + 1);
                }

                return type;
            }

            return null;
        }
    }
}

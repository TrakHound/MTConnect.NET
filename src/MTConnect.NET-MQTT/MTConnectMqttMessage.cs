// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Mqtt
{
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

        public static IEnumerable<MqttApplicationMessage> Create(IMTConnectAgent agent, bool retain = false)
        {
            if (agent != null)
            {
                var messages = new List<MqttApplicationMessage>();

                // UUID
                var topic = $"MTConnect/Agents/{agent.Uuid}/UUID";
                messages.Add(CreateMessage(topic, agent.Uuid, retain));

                // InstanceId
                topic = $"MTConnect/Agents/{agent.Uuid}/InstanceId";
                messages.Add(CreateMessage(topic, agent.InstanceId.ToString(), retain));

                // Agent Application Version
                topic = $"MTConnect/Agents/{agent.Uuid}/Version";
                messages.Add(CreateMessage(topic, agent.Version.ToString(), retain));

                // Sender
                topic = $"MTConnect/Agents/{agent.Uuid}/Sender";
                messages.Add(CreateMessage(topic, agent.Sender, retain));

                // DeviceModelChangeTime
                topic = $"MTConnect/Agents/{agent.Uuid}/DeviceModelChangeTime";
                messages.Add(CreateMessage(topic, agent.DeviceModelChangeTime.ToString("o"), retain));

                return messages;
            }

            return null;
        }

        public static IEnumerable<MqttApplicationMessage> Create(IDevice device, string documentFormatterId = DocumentFormat.XML, bool retain = false)
        {
            if (device != null && !string.IsNullOrEmpty(documentFormatterId))
            {
                var messages = new List<MqttApplicationMessage>();

                var topic = $"MTConnect/Devices/{device.Uuid}/Device";

                var payload = Formatters.EntityFormatter.Format(documentFormatterId, device);
                if (!string.IsNullOrEmpty(payload))
                {
                    messages.Add(CreateMessage(topic, payload, retain));
                }

                return messages;
            }

            return null;
        }

        public static IEnumerable<MqttApplicationMessage> Create(IAsset asset, string documentFormatterId = DocumentFormat.XML, bool retain = false)
        {
            if (asset != null)
            {
                var messages = new List<MqttApplicationMessage>();

                var payload = Formatters.EntityFormatter.Format(documentFormatterId, asset);
                if (!string.IsNullOrEmpty(payload))
                {
                    messages.Add(CreateMessage($"MTConnect/Assets/{asset.Type}/{asset.AssetId}", payload, retain));
                    messages.Add(CreateMessage($"MTConnect/Devices/{asset.DeviceUuid}/Assets/{asset.Type}/{asset.AssetId}", payload, retain));
                }

                return messages;
            }

            return null;
        }

        public static MqttApplicationMessage Create(IObservation observation, MTConnectMqttFormat format, string documentFormatterId = DocumentFormat.XML, bool retain = false)
        {
            if (observation != null && !string.IsNullOrEmpty(observation.DeviceUuid) && observation.DataItem != null && observation.DataItem.Container != null && !observation.Values.IsNullOrEmpty())
            {
                var topic = CreateTopic(observation, format);

                var payload = Formatters.EntityFormatter.Format(documentFormatterId, observation);
                if (!string.IsNullOrEmpty(payload))
                {
                    return CreateMessage(topic, payload, retain);
                }
            }

            return null;
        }

        public static MqttApplicationMessage Create(IEnumerable<IObservation> observations, MTConnectMqttFormat format, string documentFormatterId = DocumentFormat.XML, bool retain = false)
        {
            if (!observations.IsNullOrEmpty())
            {
                // Take the first Observation (should all be for the same DataItem)
                var firstObservation = observations.FirstOrDefault();
                if (firstObservation != null)
                {
                    var topic = CreateTopic(firstObservation, format);

                    var payload = Formatters.EntityFormatter.Format(documentFormatterId, observations);
                    if (!string.IsNullOrEmpty(payload))
                    {
                        return CreateMessage(topic, payload, retain);
                    }
                }
            }

            return null;
        }


        private static string CreateTopic(IObservation observation, MTConnectMqttFormat format)
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
                prefixes.Add("Observations");

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

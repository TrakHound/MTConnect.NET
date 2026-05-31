// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Settings consumed by <see cref="MTConnect.Mqtt.MTConnectMqttEntityServer"/> that publish
    /// individual MTConnect entities (devices, observations, assets) under per-entity topics
    /// rather than as packaged response documents. Configures the topic prefix, the serialisation
    /// format, and the MQTT QoS for all entity publishes.
    /// </summary>
    public interface IMTConnectMqttEntityServerConfiguration
    {
        /// <summary>The MQTT topic prefix the server publishes under (e.g. <c>MTConnect</c>).</summary>
        string TopicPrefix { get; }

        /// <summary>The serialisation format used for each entity payload (e.g. <c>JSON</c>, <c>XML</c>).</summary>
        string DocumentFormat { get; }

        /// <summary>The MQTT Quality of Service level (0, 1, or 2) applied to every publish.</summary>
        int Qos { get; set; }
    }
}

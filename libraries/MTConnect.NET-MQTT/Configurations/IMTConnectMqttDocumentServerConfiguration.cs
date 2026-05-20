// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Settings consumed by <see cref="MTConnect.Mqtt.MTConnectMqttDocumentServer"/> that publish
    /// the full MTConnect response documents (Probe, Current, Sample, Asset) over MQTT rather
    /// than individual observation topics. Configures only the publish cadence; the topic layout
    /// is defined by the concrete configuration class.
    /// </summary>
    public interface IMTConnectMqttDocumentServerConfiguration
    {
        /// <summary>
        /// Sets the Interval (in milliseconds) to send Current messages at
        /// </summary>
        int CurrentInterval { get; }

        /// <summary>
        /// Sets the Interval (in milliseconds) to send Sample messages at
        /// </summary>
        int SampleInterval { get; }
    }
}

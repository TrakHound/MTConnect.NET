// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Tracks whether an MTConnect MQTT client or broker currently has an active session with its
    /// remote peer. Used by status events on <see cref="MTConnect.Clients.MTConnectMqttClient"/>
    /// and <see cref="MTConnect.Mqtt.MTConnectMqttBroker"/> so consumers can react to connect
    /// and disconnect transitions without inspecting the underlying MQTTnet client.
    /// </summary>
    public enum MTConnectMqttConnectionStatus
    {
        /// <summary>No MQTT session is currently established; pending publishes are queued or dropped depending on configuration.</summary>
        Disconnected,

        /// <summary>An MQTT session has been negotiated; subscriptions are active and publishes flow through immediately.</summary>
        Connected
    }
}

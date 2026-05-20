// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Selects how observation topics are laid out under each device when the MTConnect MQTT
    /// broker publishes data items. The two modes are wire-compatible with each other but
    /// represent very different consumer subscription patterns.
    /// </summary>
    public enum MTConnectMqttFormat
    {
        /// <summary>
        /// Flat layout: every observation is published at
        /// <c>MTConnect/Devices/{deviceUuid}/Observations/{dataItemId}</c>. Subscribers see all
        /// data items under one branch and must inspect each payload to learn the type.
        /// </summary>
        Flat,

        /// <summary>
        /// Hierarchy layout: each observation is published under a path that mirrors the device
        /// model (container type, container id, category, type, optional sub-type). Subscribers
        /// can wildcard on a single type (for example
        /// <c>MTConnect/Devices/+/Observations/.../Samples/Temperature/#</c>) without reading
        /// every payload.
        /// </summary>
        Hierarchy
    }
}

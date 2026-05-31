// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// MQTT topic layout selector for the broker module.
    /// </summary>
    public enum MqttTopicStructure
    {
        /// <summary>
        /// Document-shaped: one MTConnect response document
        /// (<c>probe</c> / <c>current</c> / <c>sample</c> / <c>assets</c>)
        /// per topic; mirrors the HTTP request types.
        /// </summary>
        Document,

        /// <summary>
        /// Entity-shaped: one observation / device / asset per topic,
        /// with the device UUID and data-item identifier embedded in
        /// the topic path. Lower per-message bandwidth but more topics.
        /// </summary>
        Entity
    }
}

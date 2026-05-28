// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// Adapter helpers for the MQTTnet 4.3 payload API.
    ///
    /// MQTTnet deprecated <see cref="MqttApplicationMessage.Payload"/> (byte[])
    /// in favour of <see cref="MqttApplicationMessage.PayloadSegment"/>
    /// (ArraySegment&lt;byte&gt;?). These helpers wrap the segment so the
    /// callsites read like the old byte[] API but the underlying call uses
    /// the supported member - clearing CS0618 without touching every
    /// MemoryStream/Encoding/Deserialize call.
    /// </summary>
    internal static class MqttApplicationMessageExtensions
    {
        /// <summary>
        /// Returns the message payload as a byte[], or null if the segment
        /// is unset or empty. Matches the semantics of the obsolete
        /// <c>MqttApplicationMessage.Payload</c> getter.
        /// </summary>
        public static byte[] GetPayload(this MqttApplicationMessage message)
        {
            if (message == null) return null;
            var segment = message.PayloadSegment;
            if (segment.Count == 0) return null;
            if (segment.Offset == 0 && segment.Array != null && segment.Count == segment.Array.Length)
            {
                return segment.Array;
            }
            var copy = new byte[segment.Count];
            System.Buffer.BlockCopy(segment.Array, segment.Offset, copy, 0, segment.Count);
            return copy;
        }

        /// <summary>
        /// True when the message has a non-empty payload segment.
        /// Equivalent to the obsolete <c>message.Payload != null</c> check.
        /// </summary>
        public static bool HasPayload(this MqttApplicationMessage message)
        {
            if (message == null) return false;
            return message.PayloadSegment.Count > 0;
        }
    }
}

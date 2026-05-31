// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Formatters
{
    /// <summary>
    /// MQTT entity formatter that reuses the
    /// <see cref="JsonHttpEntityFormatter"/> cppagent serialization
    /// behaviour and only overrides the formatter identifier so the
    /// MQTT publishing layer can distinguish the two transports.
    /// </summary>
    public class JsonMqttEntityFormatter : JsonHttpEntityFormatter
    {
        /// <summary>
        /// The formatter identifier exposed to the MQTT publishing
        /// layer.
        /// </summary>
        public override string Id => "JSON-cppagent-mqtt";
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Default <see cref="IMTConnectMqttEntityServerConfiguration"/> implementation used by the
    /// MQTT entity server. The parameterless constructor applies the defaults expected by the
    /// reference broker: <c>MTConnect</c> topic prefix, JSON entity serialisation, and QoS 0.
    /// </summary>
    public class MTConnectMqttEntityServerConfiguration : IMTConnectMqttEntityServerConfiguration
    {
        /// <inheritdoc />
        public string TopicPrefix { get; set; }

        /// <inheritdoc />
        public string DocumentFormat { get; set; }

        /// <inheritdoc />
        public int Qos { get; set; }


        /// <summary>Initialises the configuration with the conventional defaults: <c>MTConnect</c> topic prefix, JSON entity format, and MQTT QoS 0.</summary>
        public MTConnectMqttEntityServerConfiguration()
        {
            TopicPrefix = "MTConnect";
            DocumentFormat = "JSON";
            Qos = 0;
        }
    }
}

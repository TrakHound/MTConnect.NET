// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class MqttRelayAgentApplicationConfiguration : MqttAgentApplicationConfiguration, IMqttRelayAgentApplicationConfiguration
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("qos")]
        public int QoS { get; set; }

        [JsonPropertyName("retryInterval")]
        public int RetryInterval { get; set; }


        public MqttRelayAgentApplicationConfiguration()
        {
            Server = "localhost";
            Port = 1883;
            QoS = 1;
            RetryInterval = 5000;
            RetainMessages = true;
            MqttFormat = MTConnectMqttFormat.Hierarchy;
        }
    }
}
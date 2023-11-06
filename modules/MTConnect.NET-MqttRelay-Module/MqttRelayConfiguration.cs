// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class MqttRelayConfiguration : MTConnectMqttClientConfiguration
    {
        [JsonPropertyName("retainMessages")]
        public bool RetainMessages { get; set; }

        [JsonPropertyName("mqttFormat")]
        public MTConnectMqttFormat MqttFormat { get; set; }

        [JsonPropertyName("observationIntervals")]
        public IEnumerable<int> ObservationIntervals { get; set; }


        public MqttRelayConfiguration()
        {
            RetainMessages = true;
            MqttFormat = MTConnectMqttFormat.Hierarchy;
            ObservationIntervals = new List<int> { 0, 1000 };
        }
    }
}
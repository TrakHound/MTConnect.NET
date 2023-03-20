// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    class MTConnectMqttAgentInformation
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("version")]
        public Version Version { get; set; }

        [JsonPropertyName("deviceModelChangeTime")]
        public DateTime DeviceModelChangeTime { get; set; }

        [JsonPropertyName("heartbeatInterval")]
        public int HeartbeatInterval { get; set; }

        [JsonPropertyName("observationIntervals")]
        public IEnumerable<int> ObservationIntervals { get; set; }

        [JsonPropertyName("devices")]
        public IEnumerable<string> Devices { get; set; }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class MTConnectMqttExpanderConfiguration : MTConnectMqttClientConfiguration, IMTConnectMqttExpanderConfiguration
    {
        [JsonPropertyName("devices")]
        public IEnumerable<string> Devices { get; set; }

        [JsonPropertyName("documentFormat")]
        public string DocumentFormat { get; set; }

        [JsonPropertyName("expandedTopicPrefix")]
        public string ExpandedTopicPrefix { get; set; }


        public MTConnectMqttExpanderConfiguration()
        {
            DocumentFormat = MTConnect.DocumentFormat.XML;
            ExpandedTopicPrefix = "MTConnect-Expanded";
        }
    }
}
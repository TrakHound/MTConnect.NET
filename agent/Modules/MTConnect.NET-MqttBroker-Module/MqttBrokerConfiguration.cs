// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace MTConnect.Configurations
{
    public class MqttBrokerConfiguration
    {
        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("certificateAuthority")]
        public string CertificateAuthority { get; set; }

        [JsonPropertyName("pemCertificate")]
        public string PemCertificate { get; set; }

        [JsonPropertyName("pemPrivateKey")]
        public string PemPrivateKey { get; set; }

        [JsonPropertyName("allowUntrustedCertificates")]
        public bool AllowUntrustedCertificates { get; set; }

        [JsonPropertyName("useTls")]
        public bool UseTls { get; set; }

        [JsonPropertyName("retainMessages")]
        public bool RetainMessages { get; set; }

        [JsonPropertyName("mqttFormat")]
        public MTConnectMqttFormat MqttFormat { get; set; }

        [JsonPropertyName("mqttTopicPrefix")]
        [YamlMember(Alias = "mqttTopicPrefix")]
        public string TopicPrefix { get; set; }

        [JsonPropertyName("observationIntervals")]
        public IEnumerable<int> ObservationIntervals { get; set; }


        public MqttBrokerConfiguration()
        {
            Server = "localhost";
            Port = 1883;
            RetainMessages = true;
            MqttFormat = MTConnectMqttFormat.Hierarchy;
            ObservationIntervals = new List<int> { 0, 1000 };
        }
    }
}
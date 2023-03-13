// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect MQTT Agent Application
    /// </summary>
    public class MqttAgentApplicationConfiguration : AgentApplicationConfiguration, IMqttAgentApplicationConfiguration
    {
        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

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

        [JsonPropertyName("retryInterval")]
        public int RetryInterval { get; set; }

        [JsonPropertyName("retainMessages")]
        public bool RetainMessages { get; set; }

        [JsonPropertyName("mqttFormat")]
        public MTConnectMqttFormat MqttFormat { get; set; }

        [JsonPropertyName("mqttTopicPrefix")]
        [YamlMember(Alias = "mqttTopicPrefix")]
        public string TopicPrefix { get; set; }


        public MqttAgentApplicationConfiguration()
        {
            Server = "localhost";
            Port = 1883;
            RetryInterval = 5000;
            RetainMessages = true;
            MqttFormat = MTConnectMqttFormat.Hierarchy;
        }
    }
}
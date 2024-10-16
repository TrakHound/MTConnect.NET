// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tls;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class MTConnectMqttClientConfiguration : IMTConnectMqttClientConfiguration
    {
        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("qos")]
        public int Qos { get; set; }

        [JsonPropertyName("tls")]
        public TlsConfiguration Tls { get; set; }

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

        [JsonPropertyName("topicPrefix")]
        public string TopicPrefix { get; set; }


        public MTConnectMqttClientConfiguration()
        {
            Server = "localhost";
            Port = 1883;
            Qos = 1;
            RetryInterval = 5000;
            TopicPrefix = "MTConnect";
        }
    }
}
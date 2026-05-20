// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tls;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Default <see cref="IMTConnectMqttClientConfiguration"/> implementation deserialised from
    /// the application JSON/YAML configuration. See the interface for the semantics of each
    /// property; this class only adds the serialiser bindings and the conventional defaults
    /// applied by the parameterless constructor.
    /// </summary>
    public class MTConnectMqttClientConfiguration : IMTConnectMqttClientConfiguration
    {
        /// <inheritdoc />
        [JsonPropertyName("server")]
        public string Server { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("password")]
        public string Password { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("qos")]
        public int Qos { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("tls")]
        public TlsConfiguration Tls { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("certificateAuthority")]
        public string CertificateAuthority { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("pemCertificate")]
        public string PemCertificate { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("pemPrivateKey")]
        public string PemPrivateKey { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("allowUntrustedCertificates")]
        public bool AllowUntrustedCertificates { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("useTls")]
        public bool UseTls { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("retryInterval")]
        public int RetryInterval { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("topicPrefix")]
        public string TopicPrefix { get; set; }


        /// <summary>
        /// Initialises the configuration with the MQTT-defaults expected by an MTConnect MQTT
        /// client: <c>localhost</c> broker on port 1883, QoS 1, five-second retry back-off, and
        /// the <c>MTConnect</c> topic prefix.
        /// </summary>
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

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tls;

namespace MTConnect.Configurations
{
    /// <summary>
    /// The settings an MTConnect MQTT client (<see cref="MTConnect.Clients.MTConnectMqttClient"/>
    /// and the expanded variant) needs to connect to an upstream MQTT broker that fronts an
    /// MTConnect agent. Covers broker address, MQTT credentials, QoS, TLS/PEM options, retry
    /// behaviour, and the topic-tree prefix the agent publishes under.
    /// </summary>
    public interface IMTConnectMqttClientConfiguration
    {
        /// <summary>The MQTT broker hostname or IP address the client connects to.</summary>
        string Server { get; set; }

        /// <summary>The MQTT broker TCP port (typically 1883 plain, 8883 TLS).</summary>
        int Port { get; set; }

        /// <summary>Reconnect/health-check interval in milliseconds; the client retries the connection at this cadence after a drop.</summary>
        int Interval { get; set; }

        /// <summary>UUID of the MTConnect device to subscribe to; null or empty subscribes to all devices under <see cref="TopicPrefix"/>.</summary>
        string DeviceUuid { get; set; }

        /// <summary>MQTT username for credential-based brokers; null or empty connects anonymously.</summary>
        string Username { get; set; }

        /// <summary>MQTT password paired with <see cref="Username"/>.</summary>
        string Password { get; set; }

        /// <summary>The MQTT client identifier; brokers require this to be unique per concurrent connection.</summary>
        string ClientId { get; set; }

        /// <summary>The MQTT Quality of Service level (0, 1, or 2) requested for subscriptions and publishes.</summary>
        int Qos { get; set; }

        /// <summary>Strongly-typed TLS settings (certificate path, password, client certificate); overrides the loose <see cref="CertificateAuthority"/>/<see cref="PemCertificate"/>/<see cref="PemPrivateKey"/> fields when set.</summary>
        TlsConfiguration Tls { get; set; }

        /// <summary>Path to a PEM-encoded certificate authority bundle used to verify the broker certificate when <see cref="UseTls"/> is true.</summary>
        string CertificateAuthority { get; set; }

        /// <summary>Path to a PEM-encoded client certificate presented to the broker.</summary>
        string PemCertificate { get; set; }

        /// <summary>Path to the PEM-encoded private key for <see cref="PemCertificate"/>.</summary>
        string PemPrivateKey { get; set; }

        /// <summary>When true, the connection is upgraded to MQTT-over-TLS; certificate material is taken from <see cref="Tls"/> or the loose PEM fields.</summary>
        bool UseTls { get; set; }

        /// <summary>When true, broker certificates that fail validation (unknown CA, hostname mismatch) are still accepted; intended for self-signed development brokers.</summary>
        bool AllowUntrustedCertificates { get; set; }

        /// <summary>Initial back-off delay in milliseconds between automatic reconnect attempts after a transport failure.</summary>
        int RetryInterval { get; set; }

        /// <summary>The MQTT topic prefix the client expects (e.g. <c>MTConnect</c>); subscriptions are rooted at this prefix.</summary>
        string TopicPrefix { get; set; }
    }
}

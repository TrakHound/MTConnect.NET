// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration shape for the MQTT output adapter module. Bound
    /// from the adapter module's section of <c>adapter.config.yaml</c>.
    /// </summary>
    public class ModuleConfiguration
    {
        /// <summary>
        /// The MQTT broker hostname to publish to.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The MQTT broker port number to publish to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Username for broker authentication. Leave blank for
        /// anonymous brokers.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for broker authentication. Leave blank for
        /// anonymous brokers.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// MQTT client identifier; the broker disambiguates concurrent
        /// connections by this value.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// MQTT Quality-of-Service level (0 = at most once,
        /// 1 = at least once, 2 = exactly once).
        /// </summary>
        public int Qos { get; set; }


        /// <summary>
        /// Filesystem path to the Certificate Authority bundle used
        /// when <see cref="UseTls"/> is on.
        /// </summary>
        public string CertificateAuthority { get; set; }

        /// <summary>
        /// Filesystem path to the PEM-encoded client certificate.
        /// </summary>
        public string PemCertificate { get; set; }

        /// <summary>
        /// Filesystem path to the PEM-encoded private key paired with
        /// <see cref="PemCertificate"/>.
        /// </summary>
        public string PemPrivateKey { get; set; }

        /// <summary>
        /// When <c>true</c>, certificate-chain validation errors are
        /// ignored. Intended for local development; never enable in
        /// production.
        /// </summary>
        public bool AllowUntrustedCertificates { get; set; }

        /// <summary>
        /// Whether the connection should be wrapped in TLS.
        /// </summary>
        public bool UseTls { get; set; }


        /// <summary>
        /// Connect timeout in milliseconds.
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// Reconnect delay in milliseconds after a disconnection.
        /// </summary>
        public int ReconnectInterval { get; set; }


        /// <summary>
        /// MQTT topic the adapter publishes to.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Local device key whose observations should be published on
        /// <see cref="Topic"/>. Matches against device UUID or name.
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// Document-format identifier used to serialise each published
        /// payload (e.g. <c>json</c>, <c>xml</c>).
        /// </summary>
        public string DocumentFormat { get; set; }


        /// <summary>
        /// Initialises a new instance with the bundled defaults
        /// (<c>Server = "localhost"</c>, <c>Port = 7878</c>,
        /// <c>ConnectionTimeout = 5000 ms</c>,
        /// <c>ReconnectInterval = 10000 ms</c>,
        /// <c>DocumentFormat = "json"</c>). YAML / JSON values override
        /// the defaults.
        /// </summary>
        public ModuleConfiguration()
        {
            Server = "localhost";
            Port = 7878;
            ConnectionTimeout = 5000;
            ReconnectInterval = 10000;
            DocumentFormat = "json";
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Tls
{
    /// <summary>
    /// Configures a TLS certificate loaded from PEM-encoded files.
    /// </summary>
    public class PemCertificateConfiguration
    {
        /// <summary>
        /// The path to the PEM file containing the trusted certificate authority chain.
        /// </summary>
        [JsonPropertyName("certificateAuthority")]
        public string CertificateAuthority { get; set; }

        /// <summary>
        /// The path to the PEM file containing the server or client certificate.
        /// </summary>
        [JsonPropertyName("certificatePath")]
        public string CertificatePath { get; set; }

        /// <summary>
        /// The path to the PEM file containing the certificate's private key.
        /// </summary>
        [JsonPropertyName("privateKeyPath")]
        public string PrivateKeyPath { get; set; }

        /// <summary>
        /// The password protecting the private key, when it is encrypted.
        /// </summary>
        [JsonPropertyName("privateKeyPassword")]
        public string PrivateKeyPassword { get; set; }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Tls
{
    /// <summary>
    /// Configures a TLS certificate loaded from a PKCS#12 (PFX) file.
    /// </summary>
    public class PfxCertificateConfiguration
    {
        /// <summary>
        /// The path to the PFX file containing the certificate and its private key.
        /// </summary>
        [JsonPropertyName("certificatePath")]
        public string CertificatePath { get; set; }

        /// <summary>
        /// The password protecting the PFX file, when it is encrypted.
        /// </summary>
        [JsonPropertyName("certificatePassword")]
        public string CertificatePassword { get; set; }
    }
}
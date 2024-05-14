// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Tls
{
    public class PemCertificateConfiguration
    {
        [JsonPropertyName("certificateAuthority")]
        public string CertificateAuthority { get; set; }

        [JsonPropertyName("certificatePath")]
        public string CertificatePath { get; set; }

        [JsonPropertyName("privateKeyPath")]
        public string PrivateKeyPath { get; set; }

        [JsonPropertyName("privateKeyPassword")]
        public string PrivateKeyPassword { get; set; }
    }
}
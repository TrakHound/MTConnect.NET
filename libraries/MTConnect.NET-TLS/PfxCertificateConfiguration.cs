// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Tls
{
    public class PfxCertificateConfiguration
    {
        [JsonPropertyName("certificatePath")]
        public string CertificatePath { get; set; }

        [JsonPropertyName("certificatePassword")]
        public string CertificatePassword { get; set; }
    }
}
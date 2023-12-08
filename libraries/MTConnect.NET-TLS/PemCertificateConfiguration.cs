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
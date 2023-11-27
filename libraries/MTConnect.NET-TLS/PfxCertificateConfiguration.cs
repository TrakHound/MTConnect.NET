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
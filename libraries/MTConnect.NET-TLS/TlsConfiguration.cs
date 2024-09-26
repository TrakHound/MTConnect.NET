// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace MTConnect.Tls
{
    public class TlsConfiguration
    {
        [JsonPropertyName("pfx")]
        public PfxCertificateConfiguration Pfx { get; set; }

        [JsonPropertyName("pem")]
        public PemCertificateConfiguration Pem { get; set; }

        [JsonPropertyName("verifyClientCertificate")]
        public bool VerifyClientCertificate { get; set; }

        [JsonPropertyName("omitCAValidation")]
        public bool OmitCAValidation { get; set; }


        public CertificateLoadResult GetCertificate()
        {
            // Load PFX Certificate
            if (Pfx != null)
            {
                return GetPfxCertificate();
            }

            // Load PEM Certificate
            else if (Pem != null)
            {
                return GetPemCertificate();
            }

            return new CertificateLoadResult();
        }

        public CertificateLoadResult GetCertificateAuthority()
        {
            // Load PEM Certificate
            if (Pem != null)
            {
                return GetPemCertificateAuthority();
            }

            return new CertificateLoadResult();
        }

        private CertificateLoadResult GetPfxCertificate()
        {
            if (!string.IsNullOrEmpty(Pfx.CertificatePath))
            {
                try
                {
                    X509Certificate2 certificate;

                    if (!string.IsNullOrEmpty(Pfx.CertificatePassword)) certificate = new X509Certificate2(Pfx.CertificatePath, Pfx.CertificatePassword);
                    else certificate = new X509Certificate2(Pfx.CertificatePath);
                    
                    return CertificateLoadResult.Ok(certificate);
                }
                catch (Exception ex)
                {
                    return CertificateLoadResult.Error(ex);
                }
            }

            return new CertificateLoadResult();
        }

        private CertificateLoadResult GetPemCertificate()
        {
            if (!string.IsNullOrEmpty(Pem.CertificatePath))
            {
                try
                {
                    X509Certificate2 certificate;

#if NET5_0_OR_GREATER
                    // Read from PEM file(s)
                    if (!string.IsNullOrEmpty(Pem.PrivateKeyPath) && !string.IsNullOrEmpty(Pem.PrivateKeyPassword))
                    {
                        certificate = X509Certificate2.CreateFromEncryptedPemFile(Pem.CertificatePath, Pem.PrivateKeyPassword, Pem.PrivateKeyPath);
                    }
                    else if (!string.IsNullOrEmpty(Pem.PrivateKeyPath))
                    {
                        certificate = X509Certificate2.CreateFromPemFile(Pem.CertificatePath, Pem.PrivateKeyPath);
                    }
                    else
                    {
                        certificate = X509Certificate2.CreateFromPemFile(Pem.CertificatePath);
                    }
#else
                    certificate = null;
#endif

                    // Export to Pkcs12
                    var pfxPassword = Guid.NewGuid().ToString();
                    var pkcsCert = certificate.Export(X509ContentType.Pkcs12, pfxPassword);
                    certificate = new X509Certificate2(pkcsCert, pfxPassword);

                    return CertificateLoadResult.Ok(certificate);
                }
                catch (Exception ex)
                {
                    return CertificateLoadResult.Error(ex);
                }
            }

            return new CertificateLoadResult();
        }

        private CertificateLoadResult GetPemCertificateAuthority()
        {
            if (!string.IsNullOrEmpty(Pem.CertificateAuthority))
            {
                try
                {
                    X509Certificate2 certificate;

#if NET5_0_OR_GREATER
                    certificate = new X509Certificate2(Pem.CertificateAuthority);
#else
                    certificate = null;
#endif

                    // Export to Pkcs12
                    var pfxPassword = Guid.NewGuid().ToString();
                    var pkcsCert = certificate.Export(X509ContentType.Pkcs12, pfxPassword);
                    certificate = new X509Certificate2(pkcsCert, pfxPassword);

                    return CertificateLoadResult.Ok(certificate);
                }
                catch (Exception ex)
                {
                    return CertificateLoadResult.Error(ex);
                }
            }

            return new CertificateLoadResult();
        }
    }
}
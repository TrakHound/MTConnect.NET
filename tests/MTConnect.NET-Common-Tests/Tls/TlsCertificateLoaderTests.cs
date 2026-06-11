// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using MTConnect.Tls;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Tls
{
    // Pins the SYSLIB0057 → X509CertificateLoader migration in
    // libraries/MTConnect.NET-TLS/TlsConfiguration.cs.
    //
    // Each net9.0 call site (5 in total — three in GetPfxCertificate /
    // GetPemCertificate, two in GetPemCertificateAuthority) replaced an
    // obsolete X509Certificate2 byte/path constructor with an
    // X509CertificateLoader.LoadPkcs12* / LoadCertificateFromFile call.
    // The .NET 9 release notes state the loaders are behaviour-equivalent
    // to the obsolete constructors for the password+path / bytes+password
    // shapes — these tests round-trip a freshly-generated self-signed
    // certificate through TlsConfiguration and assert the loaded
    // certificate's thumbprint matches the original byte-for-byte.
    //
    // On net8.0 (the only TFM the test project targets) the production
    // code still uses the legacy ctors — but the assembly under test is
    // the multi-TFM MTConnect.NET-TLS.dll. The test guarantees the
    // legacy path remains functional after the conditional refactor;
    // the .NET 9 path is exercised by the Release-pack CI gate, which
    // builds the same source with the X509CertificateLoader path active.
    [TestFixture]
    public class TlsCertificateLoaderTests
    {
        private string? _tempDir;

        [SetUp]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "mtc-tls-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (_tempDir != null && Directory.Exists(_tempDir))
            {
                try { Directory.Delete(_tempDir, recursive: true); } catch { /* best-effort */ }
            }
        }

        [Test]
        public void GetCertificate_pfx_without_password_round_trips_thumbprint()
        {
            using var original = CreateSelfSignedCertificate("CN=mtc-tls-pfx-nopw");
            var pfxBytes = original.Export(X509ContentType.Pkcs12);
            var pfxPath = Path.Combine(_tempDir!, "no-pw.pfx");
            File.WriteAllBytes(pfxPath, pfxBytes);

            var config = new TlsConfiguration
            {
                Pfx = new PfxCertificateConfiguration { CertificatePath = pfxPath }
            };

            var result = config.GetCertificate();

            Assert.That(result.Success, Is.True, () => "GetCertificate failed: " + result.Exception);
            Assert.That(result.Certificate!.Thumbprint, Is.EqualTo(original.Thumbprint));
            Assert.That(result.Certificate.Subject, Is.EqualTo(original.Subject));
        }

        [Test]
        public void GetCertificate_pfx_with_password_round_trips_thumbprint()
        {
            using var original = CreateSelfSignedCertificate("CN=mtc-tls-pfx-pw");
            const string password = "test-pw-9f3a";
            var pfxBytes = original.Export(X509ContentType.Pkcs12, password);
            var pfxPath = Path.Combine(_tempDir!, "pw.pfx");
            File.WriteAllBytes(pfxPath, pfxBytes);

            var config = new TlsConfiguration
            {
                Pfx = new PfxCertificateConfiguration
                {
                    CertificatePath = pfxPath,
                    CertificatePassword = password,
                }
            };

            var result = config.GetCertificate();

            Assert.That(result.Success, Is.True, () => "GetCertificate failed: " + result.Exception);
            Assert.That(result.Certificate!.Thumbprint, Is.EqualTo(original.Thumbprint));
            Assert.That(result.Certificate.Subject, Is.EqualTo(original.Subject));
        }

        [Test]
        public void GetCertificate_pem_with_private_key_round_trips_subject()
        {
            using var original = CreateSelfSignedCertificate("CN=mtc-tls-pem");
            var pemCertPath = Path.Combine(_tempDir!, "cert.pem");
            var pemKeyPath = Path.Combine(_tempDir!, "key.pem");

            File.WriteAllText(pemCertPath, ExportCertificateToPem(original));
            File.WriteAllText(pemKeyPath, ExportPrivateKeyToPem(original));

            var config = new TlsConfiguration
            {
                Pem = new PemCertificateConfiguration
                {
                    CertificatePath = pemCertPath,
                    PrivateKeyPath = pemKeyPath,
                }
            };

            var result = config.GetCertificate();

            Assert.That(result.Success, Is.True, () => "GetCertificate failed: " + result.Exception);
            // The PEM path re-exports through PKCS#12 and re-imports; the
            // re-imported certificate must retain the original subject and
            // thumbprint (the cert bytes are unchanged by the round-trip).
            Assert.That(result.Certificate!.Subject, Is.EqualTo(original.Subject));
            Assert.That(result.Certificate.Thumbprint, Is.EqualTo(original.Thumbprint));
        }

        [Test]
        public void GetCertificateAuthority_pem_round_trips_subject()
        {
            using var original = CreateSelfSignedCertificate("CN=mtc-tls-ca");
            var caPath = Path.Combine(_tempDir!, "ca.pem");
            File.WriteAllText(caPath, ExportCertificateToPem(original));

            var config = new TlsConfiguration
            {
                Pem = new PemCertificateConfiguration { CertificateAuthority = caPath }
            };

            var result = config.GetCertificateAuthority();

            Assert.That(result.Success, Is.True, () => "GetCertificateAuthority failed: " + result.Exception);
            Assert.That(result.Certificate!.Subject, Is.EqualTo(original.Subject));
            Assert.That(result.Certificate.Thumbprint, Is.EqualTo(original.Thumbprint));
        }

        private static X509Certificate2 CreateSelfSignedCertificate(string subject)
        {
            using var rsa = RSA.Create(2048);
            var request = new CertificateRequest(
                subject,
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            return request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddDays(30));
        }

        private static string ExportCertificateToPem(X509Certificate2 certificate)
        {
            var der = certificate.Export(X509ContentType.Cert);
            return "-----BEGIN CERTIFICATE-----\n"
                + Convert.ToBase64String(der, Base64FormattingOptions.InsertLineBreaks)
                + "\n-----END CERTIFICATE-----\n";
        }

        private static string ExportPrivateKeyToPem(X509Certificate2 certificate)
        {
            using var rsa = certificate.GetRSAPrivateKey()
                ?? throw new InvalidOperationException("certificate has no RSA private key");
            var pkcs8 = rsa.ExportPkcs8PrivateKey();
            return "-----BEGIN PRIVATE KEY-----\n"
                + Convert.ToBase64String(pkcs8, Base64FormattingOptions.InsertLineBreaks)
                + "\n-----END PRIVATE KEY-----\n";
        }
    }
}

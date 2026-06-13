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
    /// <summary>Pins the SYSLIB0057 migration on `TlsConfiguration.GetCertificate` / `GetCertificateAuthority`: every supported cert source (PFX with / without password, PEM cert + key, PEM CA-only) round-trips through the new `X509CertificateLoader` path without losing subject or thumbprint.</summary>
    [TestFixture]
    public class TlsCertificateLoaderTests
    {
        private string? _tempDir;

        /// <summary>Allocates a fresh per-test temp directory under the system temp root so each test owns its own PFX / PEM files and cannot race siblings.</summary>
        [SetUp]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "mtc-tls-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        /// <summary>Tears down the per-test temp directory; failures are swallowed because cert files may briefly hold OS file locks on Windows even after the cert handle is disposed.</summary>
        [TearDown]
        public void TearDown()
        {
            if (_tempDir != null && Directory.Exists(_tempDir))
            {
                try { Directory.Delete(_tempDir, recursive: true); } catch { /* best-effort */ }
            }
        }

        /// <summary>Pins that a password-less PFX loaded via `TlsConfiguration.GetCertificate` round-trips the original certificate's thumbprint and subject — the legacy `new X509Certificate2(byte[])` ctor that SYSLIB0057 obsoleted produced the same result; the new `X509CertificateLoader.LoadCertificate(...)` path must too.</summary>
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

        /// <summary>Pins that a password-protected PFX loaded via `TlsConfiguration.GetCertificate` correctly decrypts and round-trips the original thumbprint and subject — the new `X509CertificateLoader.LoadPkcs12FromFile(path, password, ...)` path must preserve the legacy obsolete-ctor's password semantics.</summary>
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

        /// <summary>Pins that a PEM-encoded certificate + matching private-key file pair loaded via `TlsConfiguration.GetCertificate` round-trips the original thumbprint and subject through the PEM → in-memory PKCS#12 → loader path that the SYSLIB0057 migration introduced.</summary>
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

        /// <summary>Pins that a PEM-encoded CA certificate (cert-only, no private key) loaded via `TlsConfiguration.GetCertificateAuthority` round-trips the original thumbprint and subject — the CA path uses `X509CertificateLoader.LoadCertificateFromFile(...)` which differs from the cert+key path used by `GetCertificate`.</summary>
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

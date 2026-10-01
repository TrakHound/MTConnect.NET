// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MQTTnet.Client;
using MTConnect.Configurations;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect
{
    /// <summary>
    /// Composes <see cref="MqttClientTlsOptions"/> from a
    /// <see cref="MqttRelayModuleConfiguration"/> in a single pass so
    /// the <c>Tls.*</c> flag surface, an optional client certificate,
    /// and the SslProtocols set all land on the same options object.
    ///
    /// <para>Extracted from <c>Module.Worker</c> to keep the TLS
    /// composition rule testable without an MQTT broker. Prior to this
    /// helper the module composed TLS options in two branches (a
    /// client-cert branch and a credentials-branch fallback) that could
    /// each overwrite the other, so <c>Tls.*</c> flags were inert
    /// whenever the user did not supply a client certificate and the
    /// credentials-branch overwrite discarded any TLS composition the
    /// client-cert branch had built.</para>
    /// </summary>
    internal static class MqttRelayTlsOptionsBuilder
    {
        /// <summary>
        /// Builds the <see cref="MqttClientTlsOptions"/> for a relay
        /// connection. Returns <c>null</c> when TLS is not enabled
        /// (i.e. <see cref="MqttRelayModuleConfiguration.UseTls"/> is
        /// <c>false</c> and no <see cref="MqttRelayModuleConfiguration.Tls"/>
        /// object is configured), letting the caller skip the
        /// <c>WithTlsOptions</c> call entirely.
        /// </summary>
        /// <param name="configuration">The bound relay configuration.
        /// The <see cref="MqttRelayModuleConfiguration.Tls"/> subtree,
        /// when present, supplies the client certificate and the
        /// <c>Tls.*</c> flag surface (<c>VerifyClientCertificate</c>,
        /// <c>OmitCAValidation</c>). The <see cref="MqttRelayModuleConfiguration.UseTls"/>
        /// flag alone enables TLS with a server-cert-only handshake and
        /// the resolved SslProtocols set.</param>
        /// <param name="sslProtocols">The resolved SslProtocols
        /// bitmask to apply to the built options; the caller is
        /// responsible for resolving the user-configured value from
        /// <see cref="MqttRelayModuleConfiguration"/>.</param>
        /// <returns>The composed <see cref="MqttClientTlsOptions"/>,
        /// or <c>null</c> when TLS is not enabled.</returns>
        public static MqttClientTlsOptions Build(
            MqttRelayModuleConfiguration configuration,
            SslProtocols sslProtocols)
        {
            if (configuration == null) return null;

            // TLS enables whenever the user opted in via UseTls OR
            // supplied a Tls object; either signal is enough. Gating
            // on the presence of a client cert (as the earlier
            // in-Module code did) locks server-cert-only TLS - the
            // mainstream MQTT-over-TLS shape - out of the module.
            var tlsEnabled = configuration.UseTls || configuration.Tls != null;
            if (!tlsEnabled) return null;

            var tlsOptionsBuilder = new MqttClientTlsOptionsBuilder();
            tlsOptionsBuilder.WithSslProtocols(sslProtocols);

            if (configuration.Tls != null)
            {
                // Layer the Tls.* flag surface on top of the base TLS
                // enablement. VerifyClientCertificate maps to the
                // inverse of AllowUntrustedCertificates so preserving
                // existing semantics: VerifyClientCertificate=true
                // rejects untrusted certificates, matching the flag
                // name.
                tlsOptionsBuilder.WithAllowUntrustedCertificates(!configuration.Tls.VerifyClientCertificate);

                var certificateResults = configuration.Tls.GetCertificate();
                var certificateAuthorityResults = configuration.Tls.GetCertificateAuthority();

                var certificates = new List<X509Certificate2>();
                if (certificateAuthorityResults.Certificate != null
                    && configuration.Tls.OmitCAValidation == false)
                {
                    certificates.Add(certificateAuthorityResults.Certificate);
                }
                if (certificateResults.Success && certificateResults.Certificate != null)
                {
                    certificates.Add(certificateResults.Certificate);
                }
                if (certificates.Count > 0)
                {
                    tlsOptionsBuilder.WithClientCertificates(certificates);
                }

#if NET5_0_OR_GREATER
                // A user-supplied CA cert pins chain validation to
                // that CA. The prior code gated this handler on the
                // presence of a client certificate; the CA validation
                // decision is independent of whether the client
                // presents its own cert, so gate it on the CA cert
                // alone.
                if (certificateAuthorityResults.Certificate != null
                    && configuration.Tls.OmitCAValidation == false)
                {
                    tlsOptionsBuilder.WithCertificateValidationHandler((certContext) =>
                    {
                        var chain = new X509Chain();
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                        chain.ChainPolicy.VerificationTime = System.DateTime.Now;
                        chain.ChainPolicy.UrlRetrievalTimeout = new System.TimeSpan(0, 0, 0);
                        chain.ChainPolicy.CustomTrustStore.Add(certificateAuthorityResults.Certificate);
                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

                        // convert provided X509Certificate to X509Certificate2
                        var x5092 = new X509Certificate2(certContext.Certificate);

                        return chain.Build(x5092);
                    });
                }
#endif
            }

            return tlsOptionsBuilder.Build();
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public interface IMTConnectMqttClientConfiguration
    {
        string Server { get; set; }

        int Port { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        string ClientId { get; set; }

        string CertificateAuthority { get; set; }

        string PemCertificate { get; set; }

        string PemPrivateKey { get; set; }

        bool UseTls { get; set; }

        bool AllowUntrustedCertificates { get; set; }

        int RetryInterval { get; set; }
    }
}
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

        string CertificateAuthority { get; set; }

        string PemClientCertificate { get; set; }

        string PemPrivateKey { get; set; }

        bool UseTls { get; set; }

        int RetryInterval { get; set; }
    }
}
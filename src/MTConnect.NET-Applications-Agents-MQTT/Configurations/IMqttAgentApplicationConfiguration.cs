// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    public interface IMqttAgentApplicationConfiguration : IAgentApplicationConfiguration
    {
        string Server { get; set; }

        int Port { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        string CertificateAuthority { get; set; }

        string PemCertificate { get; set; }

        string PemPrivateKey { get; set; }

        bool UseTls { get; set; }

        bool AllowUntrustedCertificates { get; set; }

        string TopicPrefix { get; set; }

        bool RetainMessages { get; set; }

        MTConnectMqttFormat MqttFormat { get; set; }

        IEnumerable<int> ObservationIntervals { get; set; }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class MqttAdapterModuleConfiguration
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public int QoS { get; set; }

        public string CertificateAuthority { get; set; }

        public string PemCertificate { get; set; }

        public string PemPrivateKey { get; set; }

        public bool AllowUntrustedCertificates { get; set; }

        public bool UseTls { get; set; }

        public int ConnectionTimeout { get; set; }

        public int ReconnectInterval { get; set; }

        public string Topic { get; set; }

        public string DeviceKey { get; set; }


        public MqttAdapterModuleConfiguration()
        {
            Server = "localhost";
            Port = 7878;
            ConnectionTimeout = 5000;
            ReconnectInterval = 10000;
        }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class ModuleConfiguration
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public int Interval { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public bool CleanSession { get; set; }

        public int QoS { get; set; }


        public string CertificateAuthority { get; set; }

        public string PemCertificate { get; set; }

        public string PemPrivateKey { get; set; }

        public bool AllowUntrustedCertificates { get; set; }

        public bool UseTls { get; set; }


        public int RetryInterval { get; set; }

        public string Topic { get; set; }

        public string DeviceKey { get; set; }

        public string DocumentFormat { get; set; }


        public ModuleConfiguration()
        {
            Server = "localhost";
            Port = 1883;
            QoS = 1;
            CleanSession = true;
            RetryInterval = 5000;
            DocumentFormat = "json";
        }
    }
}
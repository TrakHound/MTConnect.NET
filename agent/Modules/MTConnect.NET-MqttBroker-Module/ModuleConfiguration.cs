// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class ModuleConfiguration : IMTConnectMqttServerConfiguration
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string CertificateAuthority { get; set; }

        public string PemCertificate { get; set; }

        public string PemPrivateKey { get; set; }

        public bool AllowUntrustedCertificates { get; set; }

        public bool UseTls { get; set; }

        public int InitialDelay { get; set; }

        public int RestartInterval { get; set; }


        public int CurrentInterval { get; set; }

        public int SampleInterval { get; set; }


        public string DocumentFormat { get; set; }


        public string TopicPrefix { get; set; }

        public string ProbeTopic { get; set; }

        public string CurrentTopic { get; set; }

        public string SampleTopic { get; set; }

        public string AssetTopic { get; set; }


        public ModuleConfiguration()
        {
            Server = "localhost";
            Port = 7878;
            InitialDelay = 500;
            RestartInterval = 5000;

            DocumentFormat = "JSON";

            CurrentInterval = 5000;
            SampleInterval = 500;
            TopicPrefix = "MTConnect";
            ProbeTopic = "Probe";
            CurrentTopic = "Current";
            SampleTopic = "Sample";
            AssetTopic = "Asset";
        }
    }
}
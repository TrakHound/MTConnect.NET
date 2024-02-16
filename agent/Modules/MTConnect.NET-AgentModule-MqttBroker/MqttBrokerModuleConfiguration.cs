// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class MqttBrokerModuleConfiguration : IMTConnectMqttServerConfiguration
    {
        /// <summary>
        /// The MQTT broker hostname to bind to
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The MQTT broker port number to bind to
        /// </summary>
        public int Port { get; set; }


        /// <summary>
        /// The path to the Certificate Authority file
        /// </summary>
        public string CertificateAuthority { get; set; }

        /// <summary>
        /// The path to the PEM Certificate (.pem) file
        /// </summary>
        public string PemCertificate { get; set; }

        /// <summary>
        /// The path to the PEM Private Key file
        /// </summary>
        public string PemPrivateKey { get; set; }

        /// <summary>
        /// Sets whether to validate the certificate chain (true or false)
        /// </summary>
        public bool AllowUntrustedCertificates { get; set; }

        //public bool UseTls { get; set; }

        /// <summary>
        /// The timeout (in milliseconds) to use for connection and read/write
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// The time (in milliseconds) to delay after initial Module start (to allow for TCP binding)
        /// </summary>
        public int InitialDelay { get; set; }

        /// <summary>
        /// The time (in milliseconds) to delay between server start errors
        /// </summary>
        public int RestartInterval { get; set; }

        /// <summary>
        /// Sets the Quality Of Service (QoS) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once
        /// </summary>
        public int QoS { get; set; }


        /// <summary>
        /// The prefix to add to the MQTT topics that are published
        /// </summary>
        public string TopicPrefix { get; set; }

        /// <summary>
        /// The Document Format ID to use to format the payload
        /// </summary>
        public string DocumentFormat { get; set; }

        /// <summary>
        /// Sets whether to indent the output in each payload
        /// </summary>
        public bool IndentOutput { get; set; }


        /// <summary>
        /// Sets the Interval (in milliseconds) to send Current messages at
        /// </summary>
        public int CurrentInterval { get; set; }

        /// <summary>
        /// Sets the Interval (in milliseconds) to send Sample messages at
        /// </summary>
        public int SampleInterval { get; set; }


        public MqttBrokerModuleConfiguration()
        {
            Port = 1883;
            InitialDelay = 500;
            RestartInterval = 5000;
            Timeout = 5000;

            TopicPrefix = "MTConnect";
            DocumentFormat = "JSON";

            CurrentInterval = 5000;
            SampleInterval = 500;
        }
    }
}
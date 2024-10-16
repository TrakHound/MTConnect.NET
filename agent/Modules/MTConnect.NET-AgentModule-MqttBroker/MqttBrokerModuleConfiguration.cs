// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tls;

namespace MTConnect.Configurations
{
    public class MqttBrokerModuleConfiguration : IMTConnectMqttDocumentServerConfiguration
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
        /// Gets or Sets the TLS settings
        /// </summary>
        public TlsConfiguration Tls { get; set; }


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
        /// Sets the Quality Of Service (Qos) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once
        /// </summary>
        public int Qos { get; set; }


        /// <summary>
        /// The prefix to add to the MQTT topics that are published
        /// </summary>
        public string TopicPrefix { get; set; }

        /// <summary>
        /// Sets whether to structure topics and messages around Documents or Entities
        /// </summary>
        public MqttTopicStructure TopicStructure { get; set; }

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
            Server = null;
            Port = 1883;
            InitialDelay = 500;
            RestartInterval = 5000;
            Timeout = 5000;

            TopicPrefix = "MTConnect";
            TopicStructure = MqttTopicStructure.Document;
            DocumentFormat = "json-cppagent";

            CurrentInterval = 5000;
            SampleInterval = 500;
        }
    }
}
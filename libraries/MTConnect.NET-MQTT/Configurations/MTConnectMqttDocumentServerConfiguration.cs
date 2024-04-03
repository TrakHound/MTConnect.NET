// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class MTConnectMqttDocumentServerConfiguration : IMTConnectMqttDocumentServerConfiguration
    {
        public int CurrentInterval { get; set; }

        public int SampleInterval { get; set; }


        public string TopicPrefix { get; set; }

        public string ProbeTopic { get; set; }

        public string CurrentTopic { get; set; }

        public string SampleTopic { get; set; }

        public string AssetTopic { get; set; }


        public MTConnectMqttDocumentServerConfiguration()
        {
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

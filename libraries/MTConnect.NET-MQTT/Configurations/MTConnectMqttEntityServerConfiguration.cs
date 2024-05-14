// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class MTConnectMqttEntityServerConfiguration : IMTConnectMqttEntityServerConfiguration
    {
        public string TopicPrefix { get; set; }

        public string DocumentFormat { get; set; }


        public MTConnectMqttEntityServerConfiguration()
        {
            TopicPrefix = "MTConnect";
            DocumentFormat = "JSON";
        }
    }
}

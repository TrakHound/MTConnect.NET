// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public interface IMTConnectMqttEntityServerConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        string TopicPrefix { get; }

        /// <summary>
        /// 
        /// </summary>
        string DocumentFormat { get; }

        /// <summary>
        /// 
        /// </summary>
        int QoS { get; set; }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Shdr > Http Agent
    /// </summary>
    public interface IMqttAgentApplicationConfiguration : IAgentApplicationConfiguration, IMTConnectMqttClientConfiguration
    {
        bool RetainMessages { get; set; }

        MTConnectMqttFormat MqttFormat { get; set; }
    }
}
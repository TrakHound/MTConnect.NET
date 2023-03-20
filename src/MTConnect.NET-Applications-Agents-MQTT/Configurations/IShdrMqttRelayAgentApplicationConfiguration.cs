// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR > MQTT Agent
    /// </summary>
    public interface IShdrMqttRelayAgentApplicationConfiguration : IShdrAgentApplicationConfiguration, IMqttRelayAgentApplicationConfiguration
    {

    }
}
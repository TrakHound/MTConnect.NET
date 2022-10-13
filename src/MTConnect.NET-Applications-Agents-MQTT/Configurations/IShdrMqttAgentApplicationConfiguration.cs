// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR > MQTT Agent
    /// </summary>
    public interface IShdrMqttAgentApplicationConfiguration : IShdrAgentApplicationConfiguration, IMqttAgentApplicationConfiguration
    {

    }
}

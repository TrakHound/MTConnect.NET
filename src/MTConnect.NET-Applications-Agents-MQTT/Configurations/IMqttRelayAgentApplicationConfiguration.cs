// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public interface IMqttRelayAgentApplicationConfiguration : IMqttAgentApplicationConfiguration
    {
        string ClientId { get; set; }

        int QoS { get; set; }

        int RetryInterval { get; set; }
    }
}
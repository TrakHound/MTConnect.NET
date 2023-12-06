// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    public interface IMTConnectMqttExpanderConfiguration : IMTConnectMqttClientConfiguration
    {
        IEnumerable<string> Devices { get; set; }

        string DocumentFormat { get; set; }

        string ExpandedTopicPrefix { get; set; }
    }
}
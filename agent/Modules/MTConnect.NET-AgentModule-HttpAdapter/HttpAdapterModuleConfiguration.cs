// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    public class HttpAdapterModuleConfiguration : HttpClientConfiguration
    {
        public Dictionary<string, DeviceMappingConfiguration> Devices { get; set; }
    }
}
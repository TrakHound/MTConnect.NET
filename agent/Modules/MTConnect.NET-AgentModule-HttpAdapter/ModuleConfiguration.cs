// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    public class ModuleConfiguration : HttpClientConfiguration
    {
        public Dictionary<string, DeviceMappingConfiguration> Devices { get; set; }
    }
}
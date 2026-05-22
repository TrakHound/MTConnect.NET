// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration shape for the HTTP-adapter agent module. Inherits
    /// every HTTP client setting (URL, timeout, TLS, …) from
    /// <see cref="HttpClientConfiguration"/> and adds the
    /// <see cref="Devices"/> mapping table that routes upstream-agent
    /// devices onto the local agent's device catalogue.
    /// </summary>
    public class HttpAdapterModuleConfiguration : HttpClientConfiguration
    {
        /// <summary>
        /// Per-device mapping table: keyed by the local device key, each
        /// entry pins an upstream device by name / UUID / id so the
        /// adapter knows which source observations belong to which local
        /// device.
        /// </summary>
        public Dictionary<string, DeviceMappingConfiguration> Devices { get; set; }
    }
}
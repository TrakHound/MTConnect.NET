// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent
    /// </summary>
    public interface IHttpAgentConfiguration : IAgentConfiguration, IHttpServerConfiguration
    {
        IEnumerable<HttpServerConfiguration> Http { get; }
    }
}
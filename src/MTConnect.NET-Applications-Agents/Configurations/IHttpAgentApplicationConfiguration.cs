// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Shdr > Http Agent
    /// </summary>
    public interface IHttpAgentApplicationConfiguration : IAgentApplicationConfiguration, IHttpAgentConfiguration
    {
        IEnumerable<NamespaceConfiguration> DevicesNamespaces { get; set; }

        IEnumerable<NamespaceConfiguration> StreamsNamespaces { get; set; }

        IEnumerable<NamespaceConfiguration> AssetsNamespaces { get; set; }

        IEnumerable<NamespaceConfiguration> ErrorNamespaces { get; set; }


        StyleConfiguration DevicesStyle { get; set; }

        StyleConfiguration StreamsStyle { get; set; }

        StyleConfiguration AssetsStyle { get; set; }

        StyleConfiguration ErrorStyle { get; set; }
    }
}

// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    public class HttpServerModuleConfiguration : HttpServerConfiguration
    {
        public IEnumerable<NamespaceConfiguration> DevicesNamespaces { get; set; }

        public IEnumerable<NamespaceConfiguration> StreamsNamespaces { get; set; }

        public IEnumerable<NamespaceConfiguration> AssetsNamespaces { get; set; }

        public IEnumerable<NamespaceConfiguration> ErrorNamespaces { get; set; }


        public StyleConfiguration DevicesStyle { get; set; }

        public StyleConfiguration StreamsStyle { get; set; }

        public StyleConfiguration AssetsStyle { get; set; }

        public StyleConfiguration ErrorStyle { get; set; }
    }
}
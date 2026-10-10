// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Haas
{
    /// <summary>
    /// Reads a Haas control over Ethernet.
    /// </summary>
    public class HaasEthernetDataSource : HaasDataSource
    {
        /// <summary>
        /// Initializes a new data source using the specified Ethernet settings.
        /// </summary>
        /// <param name="configuration">The Ethernet settings.</param>
        public HaasEthernetDataSource(HaasEthernetConfiguration configuration)
            : base(CreateConnection(configuration), configuration) { }


        private static Ethernet CreateConnection(HaasEthernetConfiguration configuration)
        {
            if (configuration == null) configuration = new HaasEthernetConfiguration();

            var ethernet = new Ethernet(configuration.Server, configuration.Port);
            ethernet.ConnectionTimeout = configuration.ConnectionTimeout;
            return ethernet;
        }
    }
}

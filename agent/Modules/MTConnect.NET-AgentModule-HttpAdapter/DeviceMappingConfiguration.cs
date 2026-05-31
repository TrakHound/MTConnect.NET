// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Maps an upstream device (identified by name, UUID, or id) onto
    /// the local agent's device catalog. Bound from the
    /// <c>deviceMappings</c> section of the HTTP-adapter module's
    /// configuration.
    /// </summary>
    public class DeviceMappingConfiguration
    {
        /// <summary>
        /// Upstream device name to match against. When set, observations
        /// whose source device carries this name are routed to the
        /// matching local device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Upstream device UUID to match against. Preferred over
        /// <see cref="Name"/> when both are set because UUIDs are stable
        /// across rename operations.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Upstream device id to match against. Falls back to
        /// <see cref="Name"/> / <see cref="Uuid"/> matching when not set.
        /// </summary>
        public string Id { get; set; }
    }
}

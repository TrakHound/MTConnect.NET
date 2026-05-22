// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration shape for the SHDR-adapter agent module. Extends
    /// the base <see cref="ShdrAdapterClientConfiguration"/> (host,
    /// port, connect-timeout, heartbeat, …) with the
    /// <see cref="AllowShdrDevice"/> opt-in that lets an SHDR adapter
    /// transmit its own device model rather than relying on a pre-
    /// configured one.
    /// </summary>
    public class ShdrAdapterModuleConfiguration : ShdrAdapterClientConfiguration
    {
        /// <summary>
        /// Gets or Sets whether a Device Model can be sent from an SHDR Adapter
        /// </summary>
        public bool AllowShdrDevice { get; set; }


        /// <summary>
        /// Initialises a new instance with <see cref="AllowShdrDevice"/>
        /// set to <c>false</c> (the safe default — a generic adapter
        /// client is not auto-created unless this is opted in).
        /// </summary>
        public ShdrAdapterModuleConfiguration()
        {
            AllowShdrDevice = false;
        }
    }
}
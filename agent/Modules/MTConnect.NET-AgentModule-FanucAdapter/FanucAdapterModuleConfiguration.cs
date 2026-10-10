// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Fanuc;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for the Fanuc Adapter Agent Module (<c>fanuc-adapter</c> in <c>agent.config.yaml</c>).
    /// </summary>
    public class FanucAdapterModuleConfiguration : FanucConfiguration
    {
        /// <summary>
        /// The UUID or Name of the Device that observations are added to.
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// When <c>true</c>, the module adds a Device (named <see cref="DeviceKey"/>) with the DataItems it reports,
        /// updated with the paths, axes and spindles read from the CNC. Set to <c>false</c> to use a Device defined
        /// in a Devices file instead. Default is <c>true</c>.
        /// </summary>
        public bool CreateDevice { get; set; } = true;

        /// <summary>
        /// The UUID of the Device created when <see cref="CreateDevice"/> is <c>true</c>. Defaults to <see cref="DeviceKey"/>.
        /// </summary>
        public string DeviceUuid { get; set; }

        /// <summary>
        /// The interval (in milliseconds) between reads of the CNC. Default is 1000.
        /// </summary>
        public int ReadInterval { get; set; } = 1000;
    }
}

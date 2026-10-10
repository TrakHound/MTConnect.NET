// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Haas
{
    /// <summary>
    /// Reads a Haas control over RS-232.
    /// </summary>
    public class HaasSerialDataSource : HaasDataSource
    {
        /// <summary>
        /// Initializes a new data source using the specified serial port settings.
        /// </summary>
        /// <param name="configuration">The serial port settings.</param>
        public HaasSerialDataSource(HaasSerialConfiguration configuration)
            : base(new Serial(configuration), configuration) { }
    }
}

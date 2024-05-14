// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Data Source
    /// </summary>
    public interface IDataSourceConfiguration
    {
        /// <summary>
        /// The interval (in milliseconds) at which new data is read from the Data Source
        /// </summary>
        int ReadInterval { get; set; }
    }
}
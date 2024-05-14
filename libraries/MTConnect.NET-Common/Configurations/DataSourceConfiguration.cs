// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Data Source
    /// </summary>
    public class DataSourceConfiguration : IDataSourceConfiguration
    {
        /// <summary>
        /// The interval (in milliseconds) at which new data is read from the Data Source
        /// </summary>
        [JsonPropertyName("readInterval")]
        public int ReadInterval { get; set; }


        public DataSourceConfiguration()
        {
            ReadInterval = 1000;
        }
    }
}
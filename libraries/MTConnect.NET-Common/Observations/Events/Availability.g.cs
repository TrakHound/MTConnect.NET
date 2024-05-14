// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Agent's ability to communicate with the data source.
    /// </summary>
    public enum Availability
    {
        /// <summary>
        /// Data source is active and capable of providing data.
        /// </summary>
        AVAILABLE,
        
        /// <summary>
        /// Data source is either inactive or not capable of providing data.
        /// </summary>
        UNAVAILABLE
    }
}
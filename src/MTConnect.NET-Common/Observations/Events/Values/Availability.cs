// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Represents the Agent’s ability to communicate with the data source.
    /// </summary>
    public enum Availability
    {
        /// <summary>
        /// The Structural Element is either inactive or not capable of providing data.
        /// </summary>
        UNAVAILABLE,

        /// <summary>
        /// The Structural Element is active and capable of providing data.
        /// </summary>
        AVAILABLE
    }
}
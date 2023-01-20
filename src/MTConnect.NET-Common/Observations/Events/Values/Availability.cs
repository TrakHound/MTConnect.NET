// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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

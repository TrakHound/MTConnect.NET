// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Represents the Agent�s ability to communicate with the data source.
    /// </summary>
    public static class AvailabilityDescriptions
    {
        /// <summary>
        /// The Structural Element is either inactive or not capable of providing data.
        /// </summary>
        public const string UNAVAILABLE = "The Structural Element is either inactive or not capable of providing data.";

        /// <summary>
        /// The Structural Element is active and capable of providing data.
        /// </summary>
        public const string AVAILABLE = "The Structural Element is active and capable of providing data.";


        public static string Get(Availability value)
        {
            switch (value)
            {
                case Availability.UNAVAILABLE: return UNAVAILABLE;
                case Availability.AVAILABLE: return AVAILABLE;
            }

            return null;
        }
    }
}

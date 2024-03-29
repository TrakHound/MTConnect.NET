// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
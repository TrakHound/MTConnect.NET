// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="Availability"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class AvailabilityDescriptions
    {
        /// <summary>
        /// Data source is active and capable of providing data.
        /// </summary>
        public const string AVAILABLE = "Data source is active and capable of providing data.";
        
        /// <summary>
        /// Data source is either inactive or not capable of providing data.
        /// </summary>
        public const string UNAVAILABLE = "Data source is either inactive or not capable of providing data.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="Availability"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(Availability value)
        {
            switch (value)
            {
                case Availability.AVAILABLE: return "Data source is active and capable of providing data.";
                case Availability.UNAVAILABLE: return "Data source is either inactive or not capable of providing data.";
            }

            return null;
        }
    }
}
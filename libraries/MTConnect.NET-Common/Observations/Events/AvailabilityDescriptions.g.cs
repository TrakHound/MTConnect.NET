// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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
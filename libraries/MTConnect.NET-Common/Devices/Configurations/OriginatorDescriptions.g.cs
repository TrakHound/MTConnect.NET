// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Description text for each <see cref="Originator"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class OriginatorDescriptions
    {
        /// <summary>
        /// Manufacturer of a piece of equipment or Component.
        /// </summary>
        public const string MANUFACTURER = "Manufacturer of a piece of equipment or Component.";
        
        /// <summary>
        /// Owner or implementer of a piece of equipment or Component.
        /// </summary>
        public const string USER = "Owner or implementer of a piece of equipment or Component.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="Originator"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(Originator value)
        {
            switch (value)
            {
                case Originator.MANUFACTURER: return "Manufacturer of a piece of equipment or Component.";
                case Originator.USER: return "Owner or implementer of a piece of equipment or Component.";
            }

            return null;
        }
    }
}
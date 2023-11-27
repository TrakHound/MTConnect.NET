// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
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
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class OriginatorDescriptions
    {
        /// <summary>
        /// The manufacturer of a piece of equipment or Component.
        /// </summary>
        public const string MANUFACTURER = "The manufacturer of a piece of equipment or Component.";

        /// <summary>
        /// The owner or implementer of a piece of equipment or Component.
        /// </summary>
        public const string USER = "The owner or implementer of a piece of equipment or Component.";


        public static string Get(Originator originator)
        {
            switch (originator)
            {
                case Originator.MANUFACTURER: return MANUFACTURER;
                case Originator.USER: return USER;
            }

            return "";
        }
    }
}
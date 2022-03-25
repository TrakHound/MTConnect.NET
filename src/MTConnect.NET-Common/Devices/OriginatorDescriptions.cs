// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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

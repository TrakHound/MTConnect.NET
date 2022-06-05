// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class CriticalityDescriptions
    {
        /// <summary>
        /// The services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        public const string CRITICAL = "The services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.";

        /// <summary>
        /// The services or functions provided by the associated piece of equipment is not required for the operation of this piece of equipment.
        /// </summary>
        public const string NONCRITICAL = "The services or functions provided by the associated piece of equipment is not required for the operation of this piece of equipment.";


        public static string Get(Criticality criticality)
        {
            switch (criticality)
            {
                case Criticality.CRITICAL: return CRITICAL;
                case Criticality.NONCRITICAL: return NONCRITICAL;
            }

            return "";
        }
    }
}

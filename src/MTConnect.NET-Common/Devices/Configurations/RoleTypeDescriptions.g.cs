// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class RoleTypeDescriptions
    {
        /// <summary>
        /// Associated element performs the functions as an `Auxiliary` for this element.
        /// </summary>
        public const string AUXILIARY = "Associated element performs the functions as an `Auxiliary` for this element.";
        
        /// <summary>
        /// Associated element performs the functions of a System for this element.
        /// </summary>
        public const string SYSTEM = "Associated element performs the functions of a System for this element.";


        public static string Get(RoleType value)
        {
            switch (value)
            {
                case RoleType.AUXILIARY: return "Associated element performs the functions as an `Auxiliary` for this element.";
                case RoleType.SYSTEM: return "Associated element performs the functions of a System for this element.";
            }

            return null;
        }
    }
}
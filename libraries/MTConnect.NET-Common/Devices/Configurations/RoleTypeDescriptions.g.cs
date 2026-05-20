// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Description text for each <see cref="RoleType"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="RoleType"/> value, or <c>null</c> when none is defined.
        /// </summary>
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
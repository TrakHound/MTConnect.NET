// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Description text for each <see cref="CriticalityType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class CriticalityTypeDescriptions
    {
        /// <summary>
        /// Services or functions provided by the associated element is required for the operation of this element.
        /// </summary>
        public const string CRITICAL = "Services or functions provided by the associated element is required for the operation of this element.";
        
        /// <summary>
        /// Services or functions provided by the associated element is not required for the operation of this element.
        /// </summary>
        public const string NONCRITICAL = "Services or functions provided by the associated element is not required for the operation of this element.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="CriticalityType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(CriticalityType value)
        {
            switch (value)
            {
                case CriticalityType.CRITICAL: return "Services or functions provided by the associated element is required for the operation of this element.";
                case CriticalityType.NONCRITICAL: return "Services or functions provided by the associated element is not required for the operation of this element.";
            }

            return null;
        }
    }
}
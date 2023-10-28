// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
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
        /// 
        /// </summary>
        public const string NOT_SPECIFIED = "";


        public static string Get(CriticalityType value)
        {
            switch (value)
            {
                case CriticalityType.CRITICAL: return "Services or functions provided by the associated element is required for the operation of this element.";
                case CriticalityType.NONCRITICAL: return "Services or functions provided by the associated element is not required for the operation of this element.";
                case CriticalityType.NOT_SPECIFIED: return "";
            }

            return null;
        }
    }
}
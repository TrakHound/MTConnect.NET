// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class AlarmLimitsDescriptions
    {
        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public const string LowerLimit = "Lower conformance boundary for a variable.> Note: immediate concern or action may be required.";
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public const string LowerWarning = "Lower boundary indicating increased concern and supervision may be required.";
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public const string UpperLimit = "Upper conformance boundary for a variable.> Note: immediate concern or action may be required.";
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public const string UpperWarning = "Upper boundary indicating increased concern and supervision may be required.";
    }
}
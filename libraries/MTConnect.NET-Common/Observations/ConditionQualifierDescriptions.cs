// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// Provides additional information regarding a Fault State associated with the measured value of a process variable.
    /// </summary>
    public static class ConditionQualifierDescriptions
    {
        /// <summary>
        /// A measured value that is less than the expected value for the process variable
        /// </summary>
        public const string LOW = "A measured value that is less than the expected value for the process variable";

        /// <summary>
        /// A measured value that is greater than the expected value for the process variable
        /// </summary>
        public const string HIGH = "A measured value that is greater than the expected value for the process variable";


        public static string Get(ConditionQualifier qualifier)
        {
            switch (qualifier)
            {
                case ConditionQualifier.LOW: return LOW;
                case ConditionQualifier.HIGH: return HIGH;
            }

            return "";
        }
    }
}
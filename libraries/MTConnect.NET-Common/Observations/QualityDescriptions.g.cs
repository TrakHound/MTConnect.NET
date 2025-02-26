// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    public static class QualityDescriptions
    {
        /// <summary>
        /// Observation is not valid against the MTConnect Standard according to the validation capabilities of the MTConnect Agent.
        /// </summary>
        public const string INVALID = "Observation is not valid against the MTConnect Standard according to the validation capabilities of the MTConnect Agent.";
        
        /// <summary>
        /// Observation cannot be validated.
        /// </summary>
        public const string UNVERIFIABLE = "Observation cannot be validated.";
        
        /// <summary>
        /// Observation is valid against the MTConnect Standard.
        /// </summary>
        public const string VALID = "Observation is valid against the MTConnect Standard.";


        public static string Get(Quality value)
        {
            switch (value)
            {
                case Quality.INVALID: return "Observation is not valid against the MTConnect Standard according to the validation capabilities of the MTConnect Agent.";
                case Quality.UNVERIFIABLE: return "Observation cannot be validated.";
                case Quality.VALID: return "Observation is valid against the MTConnect Standard.";
            }

            return null;
        }
    }
}
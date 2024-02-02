// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class CharacteristicStatusDescriptions
    {
        /// <summary>
        /// Measurement is within acceptable tolerances.
        /// </summary>
        public const string PASS = "Measurement is within acceptable tolerances.";
        
        /// <summary>
        /// Measurement is not within acceptable tolerances.
        /// </summary>
        public const string FAIL = "Measurement is not within acceptable tolerances.";
        
        /// <summary>
        /// Failed, but acceptable constraints achievable by utilizing additional manufacturing processes.
        /// </summary>
        public const string REWORK = "Failed, but acceptable constraints achievable by utilizing additional manufacturing processes.";
        
        /// <summary>
        /// Measurement is indeterminate due to an equipment failure.
        /// </summary>
        public const string SYSTEM_ERROR = "Measurement is indeterminate due to an equipment failure.";
        
        /// <summary>
        /// Measurement cannot be determined.
        /// </summary>
        public const string INDETERMINATE = "Measurement cannot be determined.";
        
        /// <summary>
        /// Measurement cannot be evaluated.
        /// </summary>
        public const string NOT_ANALYZED = "Measurement cannot be evaluated.";
        
        /// <summary>
        /// Nominal provided without tolerance limits. QIF 3:2018 5.10.2.6
        /// </summary>
        public const string BASIC_OR_THEORETIC_EXACT_DIMENSION = "Nominal provided without tolerance limits. QIF 3:2018 5.10.2.6";
        
        /// <summary>
        /// Status of measurement cannot be determined.
        /// </summary>
        public const string UNDEFINED = "Status of measurement cannot be determined.";


        public static string Get(CharacteristicStatus value)
        {
            switch (value)
            {
                case CharacteristicStatus.PASS: return "Measurement is within acceptable tolerances.";
                case CharacteristicStatus.FAIL: return "Measurement is not within acceptable tolerances.";
                case CharacteristicStatus.REWORK: return "Failed, but acceptable constraints achievable by utilizing additional manufacturing processes.";
                case CharacteristicStatus.SYSTEM_ERROR: return "Measurement is indeterminate due to an equipment failure.";
                case CharacteristicStatus.INDETERMINATE: return "Measurement cannot be determined.";
                case CharacteristicStatus.NOT_ANALYZED: return "Measurement cannot be evaluated.";
                case CharacteristicStatus.BASIC_OR_THEORETIC_EXACT_DIMENSION: return "Nominal provided without tolerance limits. QIF 3:2018 5.10.2.6";
                case CharacteristicStatus.UNDEFINED: return "Status of measurement cannot be determined.";
            }

            return null;
        }
    }
}
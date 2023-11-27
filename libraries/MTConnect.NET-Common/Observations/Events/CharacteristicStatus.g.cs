// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Pass/fail result of the measurement.
    /// </summary>
    public enum CharacteristicStatus
    {
        /// <summary>
        /// Measurement is within acceptable tolerances.
        /// </summary>
        PASS,
        
        /// <summary>
        /// Measurement is not within acceptable tolerances.
        /// </summary>
        FAIL,
        
        /// <summary>
        /// Failed, but acceptable constraints achievable by utilizing additional manufacturing processes.
        /// </summary>
        REWORK,
        
        /// <summary>
        /// Measurement is indeterminate due to an equipment failure.
        /// </summary>
        SYSTEM_ERROR,
        
        /// <summary>
        /// Measurement cannot be determined.
        /// </summary>
        INDETERMINATE,
        
        /// <summary>
        /// Measurement cannot be evaluated.
        /// </summary>
        NOT_ANALYZED,
        
        /// <summary>
        /// Nominal provided without tolerance limits. QIF 3:2018 5.10.2.6
        /// </summary>
        BASIC_OR_THEORETIC_EXACT_DIMENSION,
        
        /// <summary>
        /// Status of measurement cannot be determined.
        /// </summary>
        UNDEFINED
    }
}
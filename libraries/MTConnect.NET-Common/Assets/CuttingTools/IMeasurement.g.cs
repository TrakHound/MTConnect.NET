// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained scalar value associated with a cutting tool.
    /// </summary>
    public partial interface IMeasurement
    {
        /// <summary>
        /// Shop specific code for the measurement. ISO 13399 codes **MAY** be used for these codes as well. See Cutting Tool Measurement Subtypes and Cutting Item Measurement Subtypes for details on Measurement types and their respective code values.
        /// </summary>
        string Code { get; }
        
        /// <summary>
        /// Maximum value for the measurement.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Minimum value for the measurement.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// Units the measurement was originally recorded in. See Device Information Model for the complete list of nativeUnits.
        /// </summary>
        string NativeUnits { get; }
        
        /// <summary>
        /// As advertised value for the measurement.
        /// </summary>
        double? Nominal { get; }
        
        /// <summary>
        /// Number of significant digits in the reported value.
        /// </summary>
        int? SignificantDigits { get; }
        
        /// <summary>
        /// Units for the measurements. See Device Information Model for the complete list of units.
        /// </summary>
        string Units { get; }
        
        /// <summary>
        /// 
        /// </summary>
        double? Value { get; }
    }
}
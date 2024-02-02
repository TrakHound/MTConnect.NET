// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_C09F377D_8946_421b_B746_E23C01D97EAC

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained scalar value associated with a cutting tool.
    /// </summary>
    public partial class Measurement : IMeasurement
    {
        public const string DescriptionText = "Constrained scalar value associated with a cutting tool.";


        /// <summary>
        /// Shop specific code for the measurement. ISO 13399 codes **MAY** be used for these codes as well. See Cutting Tool Measurement Subtypes and Cutting Item Measurement Subtypes for details on Measurement types and their respective code values.
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Maximum value for the measurement.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Minimum value for the measurement.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// Units the measurement was originally recorded in. See Device Information Model for the complete list of nativeUnits.
        /// </summary>
        public string NativeUnits { get; set; }
        
        /// <summary>
        /// As advertised value for the measurement.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// Number of significant digits in the reported value.
        /// </summary>
        public int? SignificantDigits { get; set; }
        
        /// <summary>
        /// Units for the measurements. See Device Information Model for the complete list of units.
        /// </summary>
        public string Units { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double? Value { get; set; }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727793846441_986747_23754

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Constrained scalar value associated with an Asset.
    /// </summary>
    public abstract class Measurement : IMeasurement
    {
        public const string DescriptionText = "Constrained scalar value associated with an Asset.";


        /// <summary>
        /// Maximum value for the measurement.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Minimum value for the measurement.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// NativeUnits.
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
        /// Units.
        /// </summary>
        public string Units { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double? Value { get; set; }
    }
}
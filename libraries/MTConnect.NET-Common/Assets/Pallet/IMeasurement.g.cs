// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// Constrained scalar value associated with an Asset.
    /// </summary>
    public interface IMeasurement
    {
        /// <summary>
        /// Maximum value for the measurement.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Minimum value for the measurement.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// NativeUnits.
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
        /// Units.
        /// </summary>
        string Units { get; }
        
        /// <summary>
        /// 
        /// </summary>
        double? Value { get; }
    }
}
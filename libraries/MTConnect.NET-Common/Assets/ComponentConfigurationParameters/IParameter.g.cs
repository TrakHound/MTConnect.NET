// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Property that determines the characteristic or behavior of an entity.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Internal identifier, register, or address.
        /// </summary>
        string Identifier { get; }
        
        /// <summary>
        /// Maximum allowed value.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Minimal allowed value.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// Descriptive name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Nominal value.
        /// </summary>
        double? Nominal { get; }
        
        /// <summary>
        /// Engineering units.units **SHOULD** be SI or MTConnect Units.
        /// </summary>
        string Units { get; }
        
        /// <summary>
        /// Configured value.
        /// </summary>
        string Value { get; }
    }
}
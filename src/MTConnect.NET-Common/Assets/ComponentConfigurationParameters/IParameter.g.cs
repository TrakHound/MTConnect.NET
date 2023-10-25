// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Property defining a configuration of a Component.
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
        string Maximum { get; }
        
        /// <summary>
        /// Minimal allowed value.
        /// </summary>
        string Minimum { get; }
        
        /// <summary>
        /// Descriptive name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Nominal value.
        /// </summary>
        string Nominal { get; }
        
        /// <summary>
        /// Engineering units.units **SHOULD** be SI or MTConnect Units (See UnitEnum).
        /// </summary>
        string Units { get; }
        
        /// <summary>
        /// Configured value.
        /// </summary>
        string Value { get; }
    }
}
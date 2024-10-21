// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1678197371649_500788_17992

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Property that determines the characteristic or behavior of an entity.
    /// </summary>
    public class Parameter : IParameter
    {
        public const string DescriptionText = "Property that determines the characteristic or behavior of an entity.";


        /// <summary>
        /// Internal identifier, register, or address.
        /// </summary>
        public string Identifier { get; set; }
        
        /// <summary>
        /// Maximum allowed value.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Minimal allowed value.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// Descriptive name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Nominal value.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// Engineering units.units **SHOULD** be SI or MTConnect Units.
        /// </summary>
        public string Units { get; set; }
        
        /// <summary>
        /// Configured value.
        /// </summary>
        public string Value { get; set; }
    }
}
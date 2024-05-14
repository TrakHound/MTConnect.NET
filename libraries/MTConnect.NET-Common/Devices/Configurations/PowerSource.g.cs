// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1696870815347_556704_3251

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Potential energy sources for the Component.
    /// </summary>
    public class PowerSource : IPowerSource
    {
        public const string DescriptionText = "Potential energy sources for the Component.";


        /// <summary>
        /// Reference to the Component providing observations about the power source.
        /// </summary>
        public string ComponentIdRef { get; set; }
        
        /// <summary>
        /// Unique identifier for the power source.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Optional precedence for a given power source.
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Type of the power source.
        /// </summary>
        public MTConnect.Devices.Configurations.PowerSourceType Type { get; set; }
        
        /// <summary>
        /// Name of the power source.
        /// </summary>
        public string Value { get; set; }
    }
}
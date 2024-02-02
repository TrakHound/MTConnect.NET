// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_A2837EAA_7D27_45c0_B365_BC308844F978

namespace MTConnect.Devices
{
    /// <summary>
    /// Identifies the Component, DataItem, or Composition from which a measured value originates.
    /// </summary>
    public class Source : ISource
    {
        public const string DescriptionText = "Identifies the Component, DataItem, or Composition from which a measured value originates.";


        /// <summary>
        /// Identifier of the Component that represents the physical part of a piece of equipment where the data represented by the DataItem originated.
        /// </summary>
        public string ComponentId { get; set; }
        
        /// <summary>
        /// Identifier of the Composition that represents the physical part of a piece of equipment where the data represented by the DataItem originated.
        /// </summary>
        public string CompositionId { get; set; }
        
        /// <summary>
        /// Identifier of the DataItem that represents the originally measured value of the data referenced by this DataItem.
        /// </summary>
        public string DataItemId { get; set; }
        
        /// <summary>
        /// Identifier of the source entity.
        /// </summary>
        public string Value { get; set; }
    }
}
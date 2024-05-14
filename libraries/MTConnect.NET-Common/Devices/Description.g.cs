// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_64352755_7251_46af_846D_937E5A1E3949

namespace MTConnect.Devices
{
    /// <summary>
    /// Descriptive content.
    /// </summary>
    public class Description : IDescription
    {
        public const string DescriptionText = "Descriptive content.";


        /// <summary>
        /// Name of the manufacturer of the physical or logical part of a piece of equipment represented by this element.
        /// </summary>
        public string Manufacturer { get; set; }
        
        /// <summary>
        /// Model description of the physical part or logical function of a piece of equipment represented by this element.
        /// </summary>
        public string Model { get; set; }
        
        /// <summary>
        /// Serial number associated with a piece of equipment.
        /// </summary>
        public string SerialNumber { get; set; }
        
        /// <summary>
        /// Identifier where a manufacturing function takes place.
        /// </summary>
        public string Station { get; set; }
        
        /// <summary>
        /// Description of the element.
        /// </summary>
        public string Value { get; set; }
    }
}
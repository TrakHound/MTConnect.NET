// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Descriptive content.
    /// </summary>
    public interface IDescription
    {
        /// <summary>
        /// Name of the manufacturer of the physical or logical part of a piece of equipment represented by this element.
        /// </summary>
        string Manufacturer { get; }
        
        /// <summary>
        /// Model description of the physical part or logical function of a piece of equipment represented by this element.
        /// </summary>
        string Model { get; }
        
        /// <summary>
        /// Serial number associated with a piece of equipment.
        /// </summary>
        string SerialNumber { get; }
        
        /// <summary>
        /// Station where the physical part or logical function of a piece of equipment is located when it is part of a manufacturing unit or cell with multiple stations.
        /// </summary>
        string Station { get; }
        
        /// <summary>
        /// Description of the element.
        /// </summary>
        string Value { get; }
    }
}
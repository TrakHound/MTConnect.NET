// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Identifies the Component, DataItem, or Composition from which a measured value originates.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Identifier of the Component that represents the physical part of a piece of equipment where the data represented by the DataItem originated.
        /// </summary>
        string ComponentId { get; }
        
        /// <summary>
        /// Identifier of the Composition that represents the physical part of a piece of equipment where the data represented by the DataItem originated.
        /// </summary>
        string CompositionId { get; }
        
        /// <summary>
        /// Identifier of the DataItem that represents the originally measured value of the data referenced by this DataItem.
        /// </summary>
        string DataItemId { get; }
        
        /// <summary>
        /// Identifier of the source entity.
        /// </summary>
        string Value { get; }
    }
}
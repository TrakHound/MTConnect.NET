// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Functional part of a piece of equipment contained within a Component.
    /// </summary>
    public partial interface IComposition
    {
        /// <summary>
        /// Type of Composition.
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Logical or physical entity that provides a capability.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IComponent> Components { get; }
        
        /// <summary>
        /// Functional part of a piece of equipment contained within a Component.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IComposition> Compositions { get; }
        
    }
}
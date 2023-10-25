// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Reference that is a pointer to all of the information associated with another entity defined for a piece of equipment.
    /// </summary>
    public interface IComponentRef : IReference
    {
        /// <summary>
        /// Pointer to the id attribute of the Component that contains the information to be associated with this element.
        /// </summary>
        MTConnect.Devices.IComponent IdRef { get; }
    }
}
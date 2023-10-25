// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1581212139957_418083_223

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Reference that is a pointer to all of the information associated with another entity defined for a piece of equipment.
    /// </summary>
    public class ComponentRef : Reference, IComponentRef
    {
        public new const string DescriptionText = "Reference that is a pointer to all of the information associated with another entity defined for a piece of equipment.";


        /// <summary>
        /// Pointer to the id attribute of the Component that contains the information to be associated with this element.
        /// </summary>
        public MTConnect.Devices.IComponent IdRef { get; set; }
    }
}
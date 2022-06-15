// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;

namespace MTConnect.Devices
{
    /// <summary>
    /// ComponentRef XML element is a pointer to all of the information associated with another Structural Element defined elsewhere in the XML document for a piece of equipment.
    /// ComponentRef allows all of the information (Lower Level Components and all Data Entities) that is associated with the other Structural Element to be directly associated with this XML element.
    /// </summary>
    public class XmlComponentReference : XmlReference
    {
        public XmlComponentReference() { }

        public XmlComponentReference(ComponentReference reference)
        {
            if (reference != null)
            {
                IdRef = reference.IdRef;
                Name = reference.Name;
            }
        }

        public override Reference ToReference()
        {
            var reference = new ComponentReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}

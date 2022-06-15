// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;

namespace MTConnect.Devices
{
    /// <summary>
    /// DataItemRef XML element is a pointer to a Data Entity associated with another Structural Element defined elsewhere in the XML document for a piece of equipment.
    /// DataItemRef allows the data associated with a data item defined in another Structural Element to be directly associated with this XML element.
    /// </summary>
    public class XmlDataItemReference : XmlReference
    {
        public XmlDataItemReference() { }

        public XmlDataItemReference(DataItemReference reference)
        {
            if (reference != null)
            {
                IdRef = reference.IdRef;
                Name = reference.Name;
            }
        }

        public override Reference ToReference()
        {
            var reference = new DataItemReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}

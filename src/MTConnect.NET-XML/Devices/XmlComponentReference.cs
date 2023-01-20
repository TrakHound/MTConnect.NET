// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;

namespace MTConnect.Devices.Xml
{
    public class XmlComponentReference : XmlReference
    {
        public override IReference ToReference()
        {
            var reference = new ComponentReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;

namespace MTConnect.Devices.Xml
{
    public class XmlDataItemReference : XmlReference
    {
        public override IReference ToReference()
        {
            var reference = new DataItemReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}
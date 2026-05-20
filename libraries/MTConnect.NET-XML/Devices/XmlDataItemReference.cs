// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.References;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>DataItemRef</c>. Resolves
    /// the inherited <c>idRef</c>/<c>name</c> attributes to a reference to a
    /// <c>DataItem</c> defined elsewhere in the device model.
    /// </summary>
    public class XmlDataItemReference : XmlReference
    {
        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="DataItemReference"/>,
        /// copying the referenced data item id and optional display name.
        /// </summary>
        public override IReference ToReference()
        {
            var reference = new DataItemReference();
            reference.IdRef = IdRef;
            reference.Name = Name;
            return reference;
        }
    }
}
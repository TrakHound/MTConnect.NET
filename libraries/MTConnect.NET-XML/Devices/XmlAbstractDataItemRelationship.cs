// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public abstract class XmlAbstractDataItemRelationship
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("idRef")]
        public string IdRef { get; set; }


        public virtual IAbstractDataItemRelationship ToRelationship() { return null; }

        public static void WriteCommonXml(XmlWriter writer, IAbstractDataItemRelationship relationship)
        {
            if (!string.IsNullOrEmpty(relationship.IdRef)) writer.WriteAttributeString("idRef", relationship.IdRef);
            if (!string.IsNullOrEmpty(relationship.Name)) writer.WriteAttributeString("name", relationship.Name);
        }
    }
}
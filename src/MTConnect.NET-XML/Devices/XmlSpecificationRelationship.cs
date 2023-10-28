// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("SpecificationRelationship")]
    public class XmlSpecificationRelationship : XmlAbstractDataItemRelationship
    {
        [XmlAttribute("type")]
        public SpecificationRelationshipType Type { get; set; }


        public override IAbstractDataItemRelationship ToRelationship()
        {
            var relationship = new SpecificationRelationship();
            relationship.Name = Name;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }

        public static void WriteXml(XmlWriter writer, ISpecificationRelationship relationship)
        {
            if (relationship != null)
            {
                writer.WriteStartElement(relationship.GetType().Name);
                WriteCommonXml(writer, relationship);
                writer.WriteAttributeString("type", relationship.Type.ToString());
                writer.WriteEndElement();
            }
        }
    }
}
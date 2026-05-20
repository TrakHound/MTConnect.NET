// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>SpecificationRelationship</c>,
    /// associating a Specification with another specification it depends on,
    /// limits, or otherwise relates to.
    /// </summary>
    [XmlRoot("SpecificationRelationship")]
    public class XmlSpecificationRelationship : XmlAbstractDataItemRelationship
    {
        /// <summary>
        /// The kind of relationship to the referenced specification, carried by
        /// the <c>type</c> attribute (for example <c>LIMIT</c>).
        /// </summary>
        [XmlAttribute("type")]
        public SpecificationRelationshipType Type { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="SpecificationRelationship"/>, copying the relationship
        /// name, referenced id, and relationship type.
        /// </summary>
        public override IAbstractDataItemRelationship ToRelationship()
        {
            var relationship = new SpecificationRelationship();
            relationship.Name = Name;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }

        /// <summary>
        /// Writes the relationship element, emitting the shared relationship
        /// attributes followed by the <c>type</c> attribute.
        /// </summary>
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
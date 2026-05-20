// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>DataItemRelationship</c>,
    /// associating a DataItem with another data item it depends on, limits, or
    /// otherwise relates to.
    /// </summary>
    [XmlRoot("DataItemRelationship")]
    public class XmlDataItemRelationship : XmlAbstractDataItemRelationship
    {
        /// <summary>
        /// The kind of relationship to the referenced data item, carried by the
        /// <c>type</c> attribute (for example <c>LIMIT</c> or <c>OBSERVATION</c>).
        /// </summary>
        [XmlAttribute("type")]
        public DataItemRelationshipType Type { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="DataItemRelationship"/>, copying the relationship name,
        /// referenced id, and relationship type.
        /// </summary>
        public override IAbstractDataItemRelationship ToRelationship()
        {
            var relationship = new DataItemRelationship();
            relationship.Name = Name;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }

        /// <summary>
        /// Writes the relationship element, emitting the shared relationship
        /// attributes followed by the <c>type</c> attribute.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IDataItemRelationship relationship)
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
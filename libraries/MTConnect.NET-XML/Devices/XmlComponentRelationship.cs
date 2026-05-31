// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>ComponentRelationship</c>,
    /// associating a Component with another component it depends on, is a
    /// parent or child of, or otherwise relates to.
    /// </summary>
    [XmlRoot("ComponentRelationship")]
    public class XmlComponentRelationship : XmlConfigurationRelationship
    {
        /// <summary>
        /// The <c>id</c> of the related component, carried by the <c>idRef</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("idRef")]
        public string IdRef { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ComponentRelationship"/>, copying the shared relationship
        /// fields, mapping the optional criticality to its enumeration, and
        /// copying the referenced component id.
        /// </summary>
        public override IConfigurationRelationship ToRelationship()
        {
            var relationship = new ComponentRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type;
            if (!string.IsNullOrEmpty(Criticality)) relationship.Criticality = Criticality.ConvertEnum<CriticalityType>();
            relationship.IdRef = IdRef;
            return relationship;
        }

        /// <summary>
        /// Writes the relationship element, emitting the shared relationship
        /// attributes followed by the optional <c>idRef</c> attribute.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IComponentRelationship relationship)
        {
            if (relationship != null)
            {
                writer.WriteStartElement(relationship.GetType().Name);
                WriteCommonXml(writer, relationship);
                if (!string.IsNullOrEmpty(relationship.IdRef)) writer.WriteAttributeString("idRef", relationship.IdRef);
                writer.WriteEndElement();
            }
        }
    }
}
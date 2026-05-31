// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Base XML serialization surrogate for the configuration relationships of
    /// a component. Carries the identification attributes shared by every
    /// relationship kind and the common attribute-writing helper.
    /// </summary>
    public abstract class XmlConfigurationRelationship
    {
        /// <summary>
        /// The unique identifier of the relationship within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The optional human-readable name of the relationship.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The kind of relationship, such as <c>PARENT</c> or <c>CHILD</c>.
        /// </summary>
        [XmlAttribute("type")]
        public RelationshipType Type { get; set; }

        /// <summary>
        /// How critical the relationship is, as the raw attribute text.
        /// </summary>
        [XmlAttribute("criticality")]
        public string Criticality { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed relationship model.
        /// Overridden by each concrete relationship surrogate; the base
        /// implementation returns <c>null</c>.
        /// </summary>
        public virtual IConfigurationRelationship ToRelationship() { return null; }

        /// <summary>
        /// Writes the identification attributes shared by every relationship
        /// kind, omitting the optional attributes that are not set.
        /// </summary>
        public static void WriteCommonXml(XmlWriter writer, IConfigurationRelationship relationship)
        {
            if (!string.IsNullOrEmpty(relationship.Id)) writer.WriteAttributeString("id", relationship.Id);
            if (!string.IsNullOrEmpty(relationship.Name)) writer.WriteAttributeString("name", relationship.Name);
            writer.WriteAttributeString("type", relationship.Type.ToString());
            if (relationship.Criticality != null) writer.WriteAttributeString("criticality", relationship.Criticality.ToString());
        }
    }
}
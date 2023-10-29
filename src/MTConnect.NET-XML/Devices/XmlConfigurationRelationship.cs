// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public abstract class XmlConfigurationRelationship
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public RelationshipType Type { get; set; }

        [XmlAttribute("criticality")]
        public string Criticality { get; set; }


        public virtual IConfigurationRelationship ToRelationship() { return null; }

        public static void WriteCommonXml(XmlWriter writer, IConfigurationRelationship relationship)
        {
            if (!string.IsNullOrEmpty(relationship.Id)) writer.WriteAttributeString("id", relationship.Id);
            if (!string.IsNullOrEmpty(relationship.Name)) writer.WriteAttributeString("name", relationship.Name);
            writer.WriteAttributeString("type", relationship.Type.ToString());
            if (relationship.Criticality != null) writer.WriteAttributeString("criticality", relationship.Criticality.ToString());
        }
    }
}
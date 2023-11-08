// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("ComponentRelationship")]
    public class XmlComponentRelationship : XmlConfigurationRelationship
    {
        [XmlAttribute("idRef")]
        public string IdRef { get; set; }


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
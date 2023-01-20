// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlRelationship
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("criticality")]
        public Criticality Criticality { get; set; }

        [XmlAttribute("idRef")]
        public string IdRef { get; set; }


        public virtual IRelationship ToRelationship()
        {
            var relationship = new Relationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            return relationship;
        }

        public static void WriteXml(XmlWriter writer, IRelationship relationship)
        {
            if (relationship != null)
            {
                writer.WriteStartElement(relationship.GetType().Name);
                WriteCommonXml(writer, relationship);

                switch (relationship.GetType().Name)
                {
                    case "ComponentRelationship": WriteXml(writer, relationship as IComponentRelationship); break;
                    case "DataItemRelationship": WriteXml(writer, relationship as IDataItemRelationship); break;
                    case "DeviceRelationship": WriteXml(writer, relationship as IDeviceRelationship); break;
                    case "SpecificationRelationship": WriteXml(writer, relationship as ISpecificationRelationship); break;
                }

                writer.WriteEndElement();
            }
        }

        public static void WriteCommonXml(XmlWriter writer, IRelationship relationship)
        {
            // Write Properties
            if (!string.IsNullOrEmpty(relationship.Id)) writer.WriteAttributeString("id", relationship.Id);
            if (!string.IsNullOrEmpty(relationship.Name)) writer.WriteAttributeString("name", relationship.Name);
            if (relationship.Criticality != Criticality.NOT_SPECIFIED) writer.WriteAttributeString("criticality", relationship.Criticality.ToString());
            if (!string.IsNullOrEmpty(relationship.IdRef)) writer.WriteAttributeString("idRef", relationship.IdRef);
        }


        public static void WriteXml(XmlWriter writer, IComponentRelationship relationship)
        {
            writer.WriteAttributeString("type", relationship.Type.ToString());
        }

        public static void WriteXml(XmlWriter writer, IDataItemRelationship relationship)
        {
            writer.WriteAttributeString("type", relationship.Type.ToString());
        }

        public static void WriteXml(XmlWriter writer, IDeviceRelationship relationship)
        {
            writer.WriteAttributeString("type", relationship.Type.ToString());
            if (!string.IsNullOrEmpty(relationship.DeviceUuidRef)) writer.WriteAttributeString("deviceUuidRef", relationship.DeviceUuidRef);
            writer.WriteAttributeString("role", relationship.Role.ToString());
            if (!string.IsNullOrEmpty(relationship.Href)) writer.WriteAttributeString("href", relationship.Href);
            if (!string.IsNullOrEmpty(relationship.XLinkType)) writer.WriteAttributeString("xLinkType", relationship.XLinkType);
        }

        public static void WriteXml(XmlWriter writer, ISpecificationRelationship relationship)
        {
            writer.WriteAttributeString("type", relationship.Type.ToString());
        }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("DeviceRelationship")]
    public class XmlDeviceRelationship : XmlConfigurationRelationship
    {
        [XmlAttribute("deviceUuidRef")]
        public string DeviceUuidRef { get; set; }

        [XmlAttribute("role")]
        public string Role { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("type", Namespace = "http://www.w3.org/1999/xlink")]
        public string XLinkType { get; set; }


        public override IDeviceRelationship ToRelationship()
        {
            var relationship = new DeviceRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type;
            if (!string.IsNullOrEmpty(Criticality)) relationship.Criticality = Criticality.ConvertEnum<CriticalityType>();
            relationship.DeviceUuidRef = DeviceUuidRef;
            if (!string.IsNullOrEmpty(Role)) relationship.Role = Role.ConvertEnum<RoleType>();
            relationship.Href = Href;
            relationship.XLinkType = XLinkType;
            return relationship;
        }

        public static void WriteXml(XmlWriter writer, IDeviceRelationship relationship)
        {
            if (relationship != null)
            {
                writer.WriteStartElement(relationship.GetType().Name);
                WriteCommonXml(writer, relationship);
                if (!string.IsNullOrEmpty(relationship.DeviceUuidRef)) writer.WriteAttributeString("deviceUuidRef", relationship.DeviceUuidRef);
                if (relationship.Role != null) writer.WriteAttributeString("role", relationship.Role.ToString());
                if (!string.IsNullOrEmpty(relationship.Href)) writer.WriteAttributeString("href", relationship.Href);
                if (!string.IsNullOrEmpty(relationship.XLinkType)) writer.WriteAttributeString("type", "http://www.w3.org/1999/xlink", relationship.XLinkType);
                writer.WriteEndElement();
            }
        }
    }
}
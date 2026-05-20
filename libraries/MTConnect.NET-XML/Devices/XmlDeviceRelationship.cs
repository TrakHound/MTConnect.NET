// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>DeviceRelationship</c>, which links
    /// a component to another device. Mirrors the on-the-wire element and
    /// converts to the strongly-typed <see cref="DeviceRelationship"/> model.
    /// </summary>
    [XmlRoot("DeviceRelationship")]
    public class XmlDeviceRelationship : XmlConfigurationRelationship
    {
        /// <summary>
        /// The UUID of the related device.
        /// </summary>
        [XmlAttribute("deviceUuidRef")]
        public string DeviceUuidRef { get; set; }

        /// <summary>
        /// The role the related device plays, such as <c>SYSTEM</c> or
        /// <c>AUXILIARY</c>, as the raw attribute text.
        /// </summary>
        [XmlAttribute("role")]
        public string Role { get; set; }

        /// <summary>
        /// The URL the related device's information can be retrieved from.
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }

        /// <summary>
        /// The XLink type of the <see cref="Href"/> reference.
        /// </summary>
        [XmlAttribute("type", Namespace = "http://www.w3.org/1999/xlink")]
        public string XLinkType { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="DeviceRelationship"/>, including the inherited
        /// identification attributes and parsing the criticality and role.
        /// </summary>
        public override IConfigurationRelationship ToRelationship()
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

        /// <summary>
        /// Writes the given <see cref="IDeviceRelationship"/> to
        /// <paramref name="writer"/>, naming the element after the concrete
        /// relationship type and omitting optional attributes that are not set.
        /// </summary>
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
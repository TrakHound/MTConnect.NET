// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Rotation> of type ThreeSpaceValueType (simple content).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Rotation (UML ID `_2024x_3_3870182_1764951373391_145162_327`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Rotation")]
    public class XmlRotation
    {
        [XmlText]
        public string Value { get; set; }


        public IRotation ToRotation()
        {
            var rotation = new Rotation();
            rotation.Value = Value;
            return rotation;
        }

        public static void WriteXml(XmlWriter writer, IRotation rotation)
        {
            if (rotation != null)
            {
                writer.WriteStartElement("Rotation");
                if (!string.IsNullOrEmpty(rotation.Value)) writer.WriteString(rotation.Value);
                writer.WriteEndElement();
            }
        }
    }
}

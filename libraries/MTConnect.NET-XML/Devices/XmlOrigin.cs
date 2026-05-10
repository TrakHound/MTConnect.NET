// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Origin> of type OriginType (extends ThreeSpaceValueType, simple content).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Origin (UML ID `_19_0_3_45f01b9_1579107743274_159386_163610`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Origin")]
    public class XmlOrigin
    {
        [XmlText]
        public string Value { get; set; }


        public IOrigin ToOrigin()
        {
            var origin = new Origin();
            origin.Value = Value;
            return origin;
        }

        public static void WriteXml(XmlWriter writer, IOrigin origin)
        {
            if (origin != null)
            {
                writer.WriteStartElement("Origin");
                if (!string.IsNullOrEmpty(origin.Value)) writer.WriteString(origin.Value);
                writer.WriteEndElement();
            }
        }
    }
}

// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Axis> of type MotionAxisType (extends ThreeSpaceValueType, simple content).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Axis (UML ID `_2024x_3_3870182_1764951682685_285104_645`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Axis")]
    public class XmlAxis
    {
        [XmlText]
        public string Value { get; set; }


        public IAxis ToAxis()
        {
            var axis = new Axis();
            axis.Value = Value;
            return axis;
        }

        public static void WriteXml(XmlWriter writer, IAxis axis)
        {
            if (axis != null)
            {
                writer.WriteStartElement("Axis");
                if (!string.IsNullOrEmpty(axis.Value)) writer.WriteString(axis.Value);
                writer.WriteEndElement();
            }
        }
    }
}

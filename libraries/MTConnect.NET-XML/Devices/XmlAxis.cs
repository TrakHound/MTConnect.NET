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
    /// <summary>
    /// XML serialization surrogate for the <c>Axis</c> element of a Component
    /// <c>Configuration</c> <c>Motion</c>, carrying the direction vector of the
    /// motion axis as simple element content.
    /// </summary>
    [XmlRoot("Axis")]
    public class XmlAxis
    {
        /// <summary>
        /// The axis direction values as the raw, space-delimited element text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Axis"/>.
        /// </summary>
        public IAxis ToAxis()
        {
            var axis = new Axis();
            axis.Value = Value;
            return axis;
        }

        /// <summary>
        /// Writes the <c>Axis</c> element for the supplied model, emitting the
        /// value as element text and omitting an empty value.
        /// </summary>
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

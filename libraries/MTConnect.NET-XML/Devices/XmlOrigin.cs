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
    /// <summary>
    /// XML serialization surrogate for the <c>Origin</c> element of a Component
    /// <c>Configuration</c> <c>CoordinateSystem</c>, carrying the position of
    /// the coordinate-system origin as simple element content.
    /// </summary>
    [XmlRoot("Origin")]
    public class XmlOrigin
    {
        /// <summary>
        /// The origin values as the raw, space-delimited element text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Origin"/>.
        /// </summary>
        public IOrigin ToOrigin()
        {
            var origin = new Origin();
            origin.Value = Value;
            return origin;
        }

        /// <summary>
        /// Writes the <c>Origin</c> element for the supplied model, emitting the
        /// value as element text and omitting an empty value.
        /// </summary>
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

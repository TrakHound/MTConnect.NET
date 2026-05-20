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
    /// <summary>
    /// XML serialization surrogate for the <c>Rotation</c> element of a
    /// Component <c>Configuration</c> <c>CoordinateSystem</c>, carrying the
    /// rotation about each axis as simple element content.
    /// </summary>
    [XmlRoot("Rotation")]
    public class XmlRotation
    {
        /// <summary>
        /// The rotation values as the raw, space-delimited element text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Rotation"/>.
        /// </summary>
        public IRotation ToRotation()
        {
            var rotation = new Rotation();
            rotation.Value = Value;
            return rotation;
        }

        /// <summary>
        /// Writes the <c>Rotation</c> element for the supplied model, emitting
        /// the value as element text and omitting an empty value.
        /// </summary>
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

// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Scale> of type SolidModelScaleType (extends ScaleValueType, simple content
//   with 1 to 3 floats).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Scale (UML ID `_2024x_3_3870182_1764951924679_898861_876`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the solid-model <c>Scale</c> element of a
    /// Component <c>Configuration</c>, carrying the scale factor(s) applied to a
    /// referenced solid model as simple element content.
    /// </summary>
    [XmlRoot("Scale")]
    public class XmlScale
    {
        /// <summary>
        /// The scale value(s) as the raw, space-delimited element text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Scale"/>.
        /// </summary>
        public IScale ToScale()
        {
            var scale = new Scale();
            scale.Value = Value;
            return scale;
        }

        /// <summary>
        /// Writes the <c>Scale</c> element for the supplied model, emitting the
        /// value as element text and omitting an empty value.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IScale scale)
        {
            if (scale != null)
            {
                writer.WriteStartElement("Scale");
                if (!string.IsNullOrEmpty(scale.Value)) writer.WriteString(scale.Value);
                writer.WriteEndElement();
            }
        }
    }
}

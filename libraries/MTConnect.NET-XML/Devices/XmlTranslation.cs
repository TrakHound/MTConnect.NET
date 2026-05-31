// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <Translation> of type ThreeSpaceValueType (simple content).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Translation (UML ID `_2024x_3_3870182_1764951167326_754957_161`).

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>Translation</c> element of a
    /// Component <c>Configuration</c> <c>CoordinateSystem</c>, carrying the
    /// translation vector relative to the parent coordinate system as simple
    /// element content.
    /// </summary>
    [XmlRoot("Translation")]
    public class XmlTranslation
    {
        /// <summary>
        /// The translation values as the raw, space-delimited element text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Translation"/>.
        /// </summary>
        public ITranslation ToTranslation()
        {
            var translation = new Translation();
            translation.Value = Value;
            return translation;
        }

        /// <summary>
        /// Writes the <c>Translation</c> element for the supplied model,
        /// emitting the value as element text and omitting an empty value.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ITranslation translation)
        {
            if (translation != null)
            {
                writer.WriteStartElement("Translation");
                if (!string.IsNullOrEmpty(translation.Value)) writer.WriteString(translation.Value);
                writer.WriteEndElement();
            }
        }
    }
}

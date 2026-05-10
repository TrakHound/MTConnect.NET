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
    [XmlRoot("Translation")]
    public class XmlTranslation
    {
        [XmlText]
        public string Value { get; set; }


        public ITranslation ToTranslation()
        {
            var translation = new Translation();
            translation.Value = Value;
            return translation;
        }

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

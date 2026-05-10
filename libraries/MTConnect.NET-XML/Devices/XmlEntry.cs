// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   complexType ThreeDimensionalEntryType: simple content with required `key` attribute
//   (KeyType -> X|Y|Z|A|B|C) and decimal value content. Used by XYZDataSetType and
//   ABCDataSetType for Configuration polymorphic DataSet variants.

using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlEntry
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}

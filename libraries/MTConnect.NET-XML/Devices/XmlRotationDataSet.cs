// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <RotationDataSet> of type ABCDataSetType. Sequence of
//   <Entry key="A|B|C">value</Entry> (0..3 occurrences per ThreeDimensionalEntryType).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class RotationDataSet (UML ID `_2024x_68e0225_1727807509860_595526_23700`).

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("RotationDataSet")]
    public class XmlRotationDataSet
    {
        [XmlElement("Entry")]
        public List<XmlEntry> Entries { get; set; }


        public IRotationDataSet ToRotationDataSet()
        {
            var dataSet = new RotationDataSet();
            if (Entries != null)
            {
                foreach (var entry in Entries)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.Key)) continue;
                    switch (entry.Key)
                    {
                        case "A": dataSet.A = entry.Value; break;
                        case "B": dataSet.B = entry.Value; break;
                        case "C": dataSet.C = entry.Value; break;
                    }
                }
            }
            return dataSet;
        }

        public static void WriteXml(XmlWriter writer, IRotationDataSet dataSet)
        {
            if (dataSet != null)
            {
                writer.WriteStartElement("RotationDataSet");
                WriteEntry(writer, "A", dataSet.A);
                WriteEntry(writer, "B", dataSet.B);
                WriteEntry(writer, "C", dataSet.C);
                writer.WriteEndElement();
            }
        }

        private static void WriteEntry(XmlWriter writer, string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            writer.WriteStartElement("Entry");
            writer.WriteAttributeString("key", key);
            writer.WriteString(value);
            writer.WriteEndElement();
        }
    }
}

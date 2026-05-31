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
    /// <summary>
    /// XML serialization surrogate for the <c>RotationDataSet</c> element, the
    /// keyed-entry representation of a coordinate-system rotation where each
    /// rotation component is carried as an <c>Entry</c> keyed <c>A</c>,
    /// <c>B</c>, or <c>C</c>.
    /// </summary>
    [XmlRoot("RotationDataSet")]
    public class XmlRotationDataSet
    {
        /// <summary>
        /// The per-axis rotation entries as serialized <c>Entry</c> elements.
        /// </summary>
        [XmlElement("Entry")]
        public List<XmlEntry> Entries { get; set; }


        /// <summary>
        /// Converts the keyed entries to a strongly-typed
        /// <see cref="RotationDataSet"/>, mapping each <c>A</c>/<c>B</c>/<c>C</c>
        /// entry to its component property and ignoring unkeyed entries.
        /// </summary>
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

        /// <summary>
        /// Writes the <c>RotationDataSet</c> element, emitting one keyed
        /// <c>Entry</c> per component and omitting components with no value.
        /// </summary>
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

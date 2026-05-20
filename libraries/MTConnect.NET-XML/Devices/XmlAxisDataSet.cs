// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <AxisDataSet> of type XYZDataSetType. Sequence of <Entry key="X|Y|Z">value</Entry>
//   (0..3 occurrences per ThreeDimensionalEntryType).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class AxisDataSet (UML ID `_2024x_68e0225_1727807971743_962437_23839`).

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>AxisDataSet</c> element, the
    /// keyed-entry representation of a motion axis direction where each
    /// component is carried as an <c>Entry</c> keyed <c>X</c>, <c>Y</c>, or
    /// <c>Z</c>.
    /// </summary>
    [XmlRoot("AxisDataSet")]
    public class XmlAxisDataSet
    {
        /// <summary>
        /// The per-axis direction entries as serialized <c>Entry</c> elements.
        /// </summary>
        [XmlElement("Entry")]
        public List<XmlEntry> Entries { get; set; }


        /// <summary>
        /// Converts the keyed entries to a strongly-typed
        /// <see cref="AxisDataSet"/>, mapping each <c>X</c>/<c>Y</c>/<c>Z</c>
        /// entry to its component property and ignoring unkeyed entries.
        /// </summary>
        public IAxisDataSet ToAxisDataSet()
        {
            var dataSet = new AxisDataSet();
            if (Entries != null)
            {
                foreach (var entry in Entries)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.Key)) continue;
                    var value = ParseDouble(entry.Value);
                    switch (entry.Key)
                    {
                        case "X": dataSet.X = value; break;
                        case "Y": dataSet.Y = value; break;
                        case "Z": dataSet.Z = value; break;
                    }
                }
            }
            return dataSet;
        }

        /// <summary>
        /// Writes the <c>AxisDataSet</c> element, emitting one keyed
        /// <c>Entry</c> per component using invariant-culture number formatting.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IAxisDataSet dataSet)
        {
            if (dataSet != null)
            {
                writer.WriteStartElement("AxisDataSet");
                WriteEntry(writer, "X", dataSet.X);
                WriteEntry(writer, "Y", dataSet.Y);
                WriteEntry(writer, "Z", dataSet.Z);
                writer.WriteEndElement();
            }
        }

        private static void WriteEntry(XmlWriter writer, string key, double value)
        {
            writer.WriteStartElement("Entry");
            writer.WriteAttributeString("key", key);
            writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        private static double ParseDouble(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0;
        }
    }
}

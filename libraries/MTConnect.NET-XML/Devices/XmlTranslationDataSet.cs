// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <TranslationDataSet> of type XYZDataSetType. Sequence of
//   <Entry key="X|Y|Z">value</Entry> (0..3 occurrences per ThreeDimensionalEntryType).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class TranslationDataSet (UML ID `_2024x_68e0225_1727807350445_154414_23573`).

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>TranslationDataSet</c> element,
    /// the keyed-entry representation of a coordinate-system translation where
    /// each component is carried as an <c>Entry</c> keyed <c>X</c>, <c>Y</c>,
    /// or <c>Z</c>.
    /// </summary>
    [XmlRoot("TranslationDataSet")]
    public class XmlTranslationDataSet
    {
        /// <summary>
        /// The per-axis translation entries as serialized <c>Entry</c> elements.
        /// </summary>
        [XmlElement("Entry")]
        public List<XmlEntry> Entries { get; set; }


        /// <summary>
        /// Converts the keyed entries to a strongly-typed
        /// <see cref="TranslationDataSet"/>, mapping each
        /// <c>X</c>/<c>Y</c>/<c>Z</c> entry to its component property and
        /// ignoring unkeyed entries.
        /// </summary>
        public ITranslationDataSet ToTranslationDataSet()
        {
            var dataSet = new TranslationDataSet();
            if (Entries != null)
            {
                foreach (var entry in Entries)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.Key)) continue;
                    switch (entry.Key)
                    {
                        case "X": dataSet.X = entry.Value; break;
                        case "Y": dataSet.Y = entry.Value; break;
                        case "Z": dataSet.Z = entry.Value; break;
                    }
                }
            }
            return dataSet;
        }

        /// <summary>
        /// Writes the <c>TranslationDataSet</c> element, emitting one keyed
        /// <c>Entry</c> per component and omitting components with no value.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ITranslationDataSet dataSet)
        {
            if (dataSet != null)
            {
                writer.WriteStartElement("TranslationDataSet");
                WriteEntry(writer, "X", dataSet.X);
                WriteEntry(writer, "Y", dataSet.Y);
                WriteEntry(writer, "Z", dataSet.Z);
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

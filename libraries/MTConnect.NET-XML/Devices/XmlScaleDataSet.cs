// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// XSD reference: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
//   Element <ScaleDataSet> of type XYZDataSetType. Sequence of
//   <Entry key="X|Y|Z">value</Entry> (0..3 occurrences per ThreeDimensionalEntryType).
// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class ScaleDataSet (UML ID `_2024x_68e0225_1727807698383_716499_23819`).

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("ScaleDataSet")]
    public class XmlScaleDataSet
    {
        [XmlElement("Entry")]
        public List<XmlEntry> Entries { get; set; }


        public IScaleDataSet ToScaleDataSet()
        {
            var dataSet = new ScaleDataSet();
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

        public static void WriteXml(XmlWriter writer, IScaleDataSet dataSet)
        {
            if (dataSet != null)
            {
                writer.WriteStartElement("ScaleDataSet");
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

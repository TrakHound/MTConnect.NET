// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlMeasurement
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("significantDigits")]
        public string SignificantDigits { get; set; }

        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlAttribute("nativeUnits")]
        public string NativeUnits { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IMeasurement ToMeasurement()
        {
            var measurement = new Measurement();
            measurement.Type = Type;
            if (!string.IsNullOrEmpty(Value)) measurement.Value = Value.ToDouble();
            if (!string.IsNullOrEmpty(SignificantDigits)) measurement.SignificantDigits = SignificantDigits.ToInt();
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Code = Code;
            if (!string.IsNullOrEmpty(Maximum)) measurement.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Minimum)) measurement.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) measurement.Nominal = Nominal.ToDouble();
            return Measurement.Create(Type, measurement);
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IMeasurement> measurements)
        {
            if (!measurements.IsNullOrEmpty())
            {
                writer.WriteStartElement("Measurements");

                foreach (var measurement in measurements)
                {
                    writer.WriteStartElement(measurement.Type);
                    writer.WriteAttributeString("code", measurement.Code);
                    if (measurement.Units != null) writer.WriteAttributeString("units", measurement.Units);
                    if (measurement.NativeUnits != null) writer.WriteAttributeString("nativeUnits", measurement.NativeUnits);
                    if (measurement.SignificantDigits != null) writer.WriteAttributeString("significantDigits", measurement.SignificantDigits.ToString());
                    if (measurement.Maximum != null) writer.WriteAttributeString("maximum", measurement.Maximum.ToString());
                    if (measurement.Minimum != null) writer.WriteAttributeString("minimum", measurement.Minimum.ToString());
                    if (measurement.Nominal != null) writer.WriteAttributeString("nominal", measurement.Nominal.ToString());
                    if (measurement.Value != null) writer.WriteString(measurement.Value.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
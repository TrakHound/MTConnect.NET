// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for a cutting-tool <c>Measurement</c>.
    /// Mirrors the on-the-wire element, where the dimension is carried as the
    /// element text and its metadata as attributes, and converts to the
    /// strongly-typed <see cref="IToolingMeasurement"/> model.
    /// </summary>
    public class XmlMeasurement
    {
        /// <summary>
        /// The kind of measurement, such as <c>OverallToolLength</c> or
        /// <c>BodyDiameterMax</c>.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value, as the raw
        /// attribute text.
        /// </summary>
        [XmlAttribute("significantDigits")]
        public string SignificantDigits { get; set; }

        /// <summary>
        /// The engineering units the value is reported in.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The units the value was originally measured in before conversion.
        /// </summary>
        [XmlAttribute("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// The short code identifying the measurement on tooling data sheets.
        /// </summary>
        [XmlAttribute("code")]
        public string Code { get; set; }

        /// <summary>
        /// The maximum tolerated value, as the raw attribute text.
        /// </summary>
        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        /// <summary>
        /// The minimum tolerated value, as the raw attribute text.
        /// </summary>
        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        /// <summary>
        /// The nominal (target) value, as the raw attribute text.
        /// </summary>
        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        /// <summary>
        /// The measured value, carried as the element text.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="IToolingMeasurement"/>, parsing the numeric text values
        /// and instantiating the concrete measurement class that matches
        /// <see cref="Type"/>.
        /// </summary>
        public IToolingMeasurement ToMeasurement()
        {
            var measurement = new ToolingMeasurement();
            measurement.Type = Type;
            if (!string.IsNullOrEmpty(Value)) measurement.Value = Value.ToDouble();
            if (!string.IsNullOrEmpty(SignificantDigits)) measurement.SignificantDigits = SignificantDigits.ToInt();
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Code = Code;
            if (!string.IsNullOrEmpty(Maximum)) measurement.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Minimum)) measurement.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) measurement.Nominal = Nominal.ToDouble();
            return ToolingMeasurement.Create(Type, measurement);
        }

        /// <summary>
        /// Writes the given measurements to <paramref name="writer"/> wrapped in
        /// a <c>Measurements</c> element, one element per measurement named
        /// after its type, omitting optional attributes that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IEnumerable<IToolingMeasurement> measurements)
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
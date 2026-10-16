// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Pallet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// Base XML serialization surrogate for the physical assets (<c>Pallet</c> and <c>Fixture</c>,
    /// MTConnect 2.5+): the inspection dates and the <c>Measurements</c> they share.
    /// </summary>
    public abstract class XmlPhysicalAsset : XmlAsset
    {
        /// <summary>
        /// The date the asset was made, as the raw element text.
        /// </summary>
        [XmlElement("ManufactureDate")]
        public string ManufactureDate { get; set; }

        /// <summary>
        /// The date the asset was last calibrated, as the raw element text.
        /// </summary>
        [XmlElement("CalibrationDate")]
        public string CalibrationDate { get; set; }

        /// <summary>
        /// The date the asset was last inspected, as the raw element text.
        /// </summary>
        [XmlElement("InspectionDate")]
        public string InspectionDate { get; set; }

        /// <summary>
        /// The date the asset is next due for inspection, as the raw element text.
        /// </summary>
        [XmlElement("NextInspectionDate")]
        public string NextInspectionDate { get; set; }

        /// <summary>
        /// The measurements of the asset (weight, height, width, length, swing and their loaded counterparts).
        /// </summary>
        [XmlElement("Measurements")]
        public XmlPhysicalAssetMeasurements Measurements { get; set; }


        /// <summary>
        /// True when the document being written has the full 2.6+ content; 2.5 defines only the
        /// <c>Measurements</c> of a physical asset.
        /// </summary>
        protected static bool WritesFullContent => WriteVersion == null || WriteVersion >= MTConnectVersions.Version26;


        /// <summary>
        /// Copies the common and physical-asset properties onto <paramref name="asset"/>.
        /// </summary>
        protected void CopyPhysicalProperties(PhysicalAsset asset)
        {
            if (asset != null)
            {
                CopyCommonProperties(asset);

                asset.ManufactureDate = ParseDate(ManufactureDate);
                asset.CalibrationDate = ParseDate(CalibrationDate);
                asset.InspectionDate = ParseDate(InspectionDate);
                asset.NextInspectionDate = ParseDate(NextInspectionDate);
                asset.Measurements = Measurements?.ToMeasurements();
            }
        }

        /// <summary>
        /// Writes the inspection dates (2.6+, only those that are set) and the <c>Measurements</c>.
        /// </summary>
        protected static void WritePhysicalElementsXml(XmlWriter writer, IPhysicalAsset asset)
        {
            if (asset != null)
            {
                if (WritesFullContent)
                {
                    WriteDate(writer, "ManufactureDate", asset.ManufactureDate);
                    WriteDate(writer, "CalibrationDate", asset.CalibrationDate);
                    WriteDate(writer, "InspectionDate", asset.InspectionDate);
                    WriteDate(writer, "NextInspectionDate", asset.NextInspectionDate);
                }

                XmlPhysicalAssetMeasurements.WriteXml(writer, asset.Measurements);
            }
        }

        /// <summary>
        /// Writes <paramref name="value"/> as an element when it is set.
        /// </summary>
        protected static void WriteElement(XmlWriter writer, string name, string value)
        {
            if (!string.IsNullOrEmpty(value)) writer.WriteElementString(name, value);
        }

        private static void WriteDate(XmlWriter writer, string name, DateTime value)
        {
            if (value > DateTime.MinValue) writer.WriteElementString(name, FormatTimestamp(value));
        }

        private static DateTime ParseDate(string value)
        {
            if (!string.IsNullOrEmpty(value) &&
                DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var date))
            {
                return date;
            }

            return default;
        }

        /// <summary>
        /// Parses an integer element value, returning 0 when it is missing or invalid.
        /// </summary>
        protected static int ParseInteger(string value)
        {
            return !string.IsNullOrEmpty(value) && int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0;
        }
    }

    /// <summary>
    /// XML serialization surrogate for the <c>Measurements</c> of a physical asset, whose child
    /// elements are named by measurement type (<c>Weight</c>, <c>LoadedHeight</c>, ...).
    /// </summary>
    public class XmlPhysicalAssetMeasurements
    {
        // The order the schema lists them in
        private static readonly string[] _typeOrder = new[]
        {
            WeightMeasurement.TypeId, HeightMeasurement.TypeId, WidthMeasurement.TypeId, LengthMeasurement.TypeId, SwingMeasurement.TypeId,
            LoadedWeightMeasurement.TypeId, LoadedHeightMeasurement.TypeId, LoadedWidthMeasurement.TypeId, LoadedLengthMeasurement.TypeId, LoadedSwingMeasurement.TypeId
        };

        /// <summary>
        /// The measurement elements as read.
        /// </summary>
        [XmlAnyElement]
        public XmlElement[] Elements { get; set; }


        /// <summary>
        /// Converts the elements into measurements, using the concrete measurement class for each known
        /// type and a plain <see cref="Measurement"/> carrying the type name otherwise.
        /// </summary>
        public IEnumerable<IMeasurement> ToMeasurements()
        {
            if (Elements == null || Elements.Length < 1) return null;

            var measurements = new List<IMeasurement>();
            foreach (var element in Elements)
            {
                var measurement = Measurement.Create(element.LocalName);
                measurement.Value = ParseDouble(element.InnerText);
                measurement.Units = GetAttribute(element, "units");
                measurement.NativeUnits = GetAttribute(element, "nativeUnits");
                measurement.Maximum = ParseDouble(GetAttribute(element, "maximum"));
                measurement.Minimum = ParseDouble(GetAttribute(element, "minimum"));
                measurement.Nominal = ParseDouble(GetAttribute(element, "nominal"));

                var significantDigits = GetAttribute(element, "significantDigits");
                if (significantDigits != null && int.TryParse(significantDigits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var digits))
                {
                    measurement.SignificantDigits = digits;
                }

                measurements.Add(measurement);
            }

            return measurements;
        }

        /// <summary>
        /// Writes the <c>Measurements</c> element in the schema's type order; writes nothing when there
        /// are no measurements. <c>nominal</c> is written for 2.6+ documents only.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IEnumerable<IMeasurement> measurements)
        {
            var items = measurements?
                .Where(o => o != null && !string.IsNullOrEmpty(Measurement.GetTypeName(o)))
                .OrderBy(o => { var i = Array.IndexOf(_typeOrder, Measurement.GetTypeName(o)); return i < 0 ? int.MaxValue : i; })
                .ToList();

            if (items == null || items.Count < 1) return;

            var writeNominal = XmlAsset.WriteVersion == null || XmlAsset.WriteVersion >= MTConnectVersions.Version26;

            writer.WriteStartElement("Measurements");

            foreach (var measurement in items)
            {
                writer.WriteStartElement(Measurement.GetTypeName(measurement));

                if (measurement.SignificantDigits != null) writer.WriteAttributeString("significantDigits", XmlConvert.ToString(measurement.SignificantDigits.Value));
                if (!string.IsNullOrEmpty(measurement.Units)) writer.WriteAttributeString("units", measurement.Units);
                if (!string.IsNullOrEmpty(measurement.NativeUnits)) writer.WriteAttributeString("nativeUnits", measurement.NativeUnits);
                if (measurement.Maximum != null) writer.WriteAttributeString("maximum", XmlConvert.ToString(measurement.Maximum.Value));
                if (measurement.Minimum != null) writer.WriteAttributeString("minimum", XmlConvert.ToString(measurement.Minimum.Value));
                if (writeNominal && measurement.Nominal != null) writer.WriteAttributeString("nominal", XmlConvert.ToString(measurement.Nominal.Value));
                if (measurement.Value != null) writer.WriteString(XmlConvert.ToString(measurement.Value.Value));

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }


        private static string GetAttribute(XmlElement element, string name)
        {
            var attribute = element.GetAttributeNode(name);
            return attribute?.Value;
        }

        private static double? ParseDouble(string value)
        {
            if (!string.IsNullOrEmpty(value) && double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) return d;
            return null;
        }
    }
}

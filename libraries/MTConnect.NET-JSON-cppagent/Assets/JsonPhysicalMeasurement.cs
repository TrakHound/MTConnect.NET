// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Pallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    /// <summary>
    /// cppagent JSON surrogate for one measurement of a physical asset (<c>Pallet</c> or <c>Fixture</c>).
    /// The measurement type is not a property: measurements are grouped under their type name, as
    /// <c>"Measurements": { "Weight": [ { "value": 12.5, "units": "KILOGRAM" } ] }</c>.
    /// </summary>
    public class JsonPhysicalMeasurement
    {
        // The order the MTConnect schema lists the measurement types in
        private static readonly string[] _typeOrder = new[]
        {
            WeightMeasurement.TypeId, HeightMeasurement.TypeId, WidthMeasurement.TypeId, LengthMeasurement.TypeId, SwingMeasurement.TypeId,
            LoadedWeightMeasurement.TypeId, LoadedHeightMeasurement.TypeId, LoadedWidthMeasurement.TypeId, LoadedLengthMeasurement.TypeId, LoadedSwingMeasurement.TypeId
        };


        /// <summary>
        /// The measured value.
        /// </summary>
        [JsonPropertyName("value")]
        public double? Value { get; set; }

        /// <summary>
        /// The number of significant digits in the value.
        /// </summary>
        [JsonPropertyName("significantDigits")]
        public int? SignificantDigits { get; set; }

        /// <summary>
        /// The units of the value.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The units the value was originally measured in.
        /// </summary>
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// The maximum allowed value.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The minimum allowed value.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// The nominal value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }


        /// <summary>
        /// Creates an empty surrogate for deserialization.
        /// </summary>
        public JsonPhysicalMeasurement() { }

        /// <summary>
        /// Creates the surrogate from <paramref name="measurement"/>.
        /// </summary>
        public JsonPhysicalMeasurement(IMeasurement measurement)
        {
            if (measurement != null)
            {
                Value = measurement.Value;
                SignificantDigits = measurement.SignificantDigits;
                Units = measurement.Units;
                NativeUnits = measurement.NativeUnits;
                Maximum = measurement.Maximum;
                Minimum = measurement.Minimum;
                Nominal = measurement.Nominal;
            }
        }


        /// <summary>
        /// Converts this surrogate into the measurement class for <paramref name="type"/>.
        /// </summary>
        public IMeasurement ToMeasurement(string type)
        {
            var measurement = Measurement.Create(type);
            measurement.Value = Value;
            measurement.SignificantDigits = SignificantDigits;
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Maximum = Maximum;
            measurement.Minimum = Minimum;
            measurement.Nominal = Nominal;
            return measurement;
        }


        // Grouped by type name, in schema order (then any other types in the order they appear)
        internal static Dictionary<string, List<JsonPhysicalMeasurement>> Group(IEnumerable<IMeasurement> measurements)
        {
            var items = measurements?
                .Where(o => o != null && !string.IsNullOrEmpty(Measurement.GetTypeName(o)))
                .GroupBy(o => Measurement.GetTypeName(o))
                .OrderBy(o => { var i = Array.IndexOf(_typeOrder, o.Key); return i < 0 ? int.MaxValue : i; })
                .ToList();

            if (items == null || items.Count < 1) return null;

            var groups = new Dictionary<string, List<JsonPhysicalMeasurement>>();
            foreach (var group in items)
            {
                groups.Add(group.Key, group.Select(o => new JsonPhysicalMeasurement(o)).ToList());
            }
            return groups;
        }

        internal static IEnumerable<IMeasurement> Ungroup(Dictionary<string, List<JsonPhysicalMeasurement>> groups)
        {
            var measurements = groups?
                .Where(o => !string.IsNullOrEmpty(o.Key) && o.Value != null)
                .SelectMany(o => o.Value.Where(m => m != null).Select(m => m.ToMeasurement(o.Key)))
                .ToList();

            return measurements != null && measurements.Count > 0 ? measurements : null;
        }

        // Unset dates (DateTime.MinValue in the model) are left out of the JSON
        internal static DateTime? ToNullable(DateTime value) => value > DateTime.MinValue ? value : (DateTime?)null;
    }
}

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
    /// JSON surrogate for a measurement of a physical asset (<c>Pallet</c> or <c>Fixture</c>), such as
    /// <c>Weight</c> or <c>LoadedHeight</c>, named by its <c>type</c>.
    /// </summary>
    public class JsonPhysicalMeasurement
    {
        /// <summary>
        /// The measurement type, such as <c>Weight</c> or <c>LoadedHeight</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

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
                Type = Measurement.GetTypeName(measurement);
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
        /// Converts this surrogate into the strongly-typed model.
        /// </summary>
        public IMeasurement ToMeasurement()
        {
            var measurement = Measurement.Create(Type);
            measurement.Value = Value;
            measurement.SignificantDigits = SignificantDigits;
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Maximum = Maximum;
            measurement.Minimum = Minimum;
            measurement.Nominal = Nominal;
            return measurement;
        }


        internal static List<JsonPhysicalMeasurement> Create(IEnumerable<IMeasurement> measurements)
        {
            var items = measurements?.Where(o => o != null).Select(o => new JsonPhysicalMeasurement(o)).ToList();
            return items != null && items.Count > 0 ? items : null;
        }

        internal static IEnumerable<IMeasurement> ToMeasurements(IEnumerable<JsonPhysicalMeasurement> measurements)
        {
            var items = measurements?.Where(o => o != null && !string.IsNullOrEmpty(o.Type)).Select(o => o.ToMeasurement()).ToList();
            return items != null && items.Count > 0 ? items : null;
        }

        // Unset dates (DateTime.MinValue in the model) are left out of the JSON
        internal static DateTime? ToNullable(DateTime value) => value > DateTime.MinValue ? value : (DateTime?)null;
    }
}

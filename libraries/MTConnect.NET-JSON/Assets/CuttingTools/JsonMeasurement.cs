// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools.Measurements
{
    /// <summary>
    /// JSON serialization surrogate for a cutting tool <c>Measurement</c>.
    /// Mirrors the on-the-wire shape so the JSON serializer can read and write
    /// it, then converts to and from the strongly-typed
    /// <see cref="ToolingMeasurement"/> model.
    /// </summary>
    public class JsonMeasurement
    {
        /// <summary>
        /// The measurement type (for example CuttingDiameter or
        /// OverallToolLength).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The measured value.
        /// </summary>
        [JsonPropertyName("value")]
        public double? Value { get; set; }

        /// <summary>
        /// The number of significant digits in the measured value.
        /// </summary>
        [JsonPropertyName("significantDigits")]
        public int? SignificantDigits { get; set; }

        /// <summary>
        /// The engineering units the measurement is expressed in.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The units the measurement is natively reported in, prior to
        /// conversion to <see cref="Units"/>.
        /// </summary>
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// The standardized code identifying the measurement.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// The maximum value within tolerance.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The minimum value within tolerance.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// The nominal (target) value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonMeasurement() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IToolingMeasurement"/>.
        /// </summary>
        public JsonMeasurement(IToolingMeasurement measurement)
        {
            if (measurement != null)
            {
                Type = measurement.Type;
                Value = measurement.Value;
                SignificantDigits = measurement.SignificantDigits;
                Units = measurement.Units;
                NativeUnits = measurement.NativeUnits;
                Code = measurement.Code;
                Maximum = measurement.Maximum;
                Minimum = measurement.Minimum;
                Nominal = measurement.Nominal;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IToolingMeasurement"/>, instantiating the concrete
        /// measurement subtype for <see cref="Type"/>.
        /// </summary>
        public IToolingMeasurement ToMeasurement()
        {
            var measurement = new ToolingMeasurement();
            measurement.Type = Type;
            measurement.Value = Value;
            measurement.SignificantDigits = SignificantDigits;
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Code = Code;
            measurement.Maximum = Maximum;
            measurement.Minimum = Minimum;
            measurement.Nominal = Nominal;
            return ToolingMeasurement.Create(Type, measurement);
        }
    }
}
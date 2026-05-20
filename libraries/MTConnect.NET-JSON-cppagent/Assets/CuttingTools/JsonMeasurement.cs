// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool measurement value
    /// in the cppagent-compatible shape. Carries the measured value with
    /// its significant digits, units, native units, optional code, and
    /// tolerance band (minimum, maximum, nominal). The measurement type
    /// is stored on the dictionary key in the parent
    /// <c>Measurements</c> container, so it is supplied to
    /// <see cref="ToMeasurement"/> rather than embedded in the surrogate.
    /// </summary>
    public class JsonMeasurement
    {
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
        /// Engineering units of the measured value.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// Native engineering units used by the equipment for this
        /// measurement.
        /// </summary>
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// Optional measurement code (for example an ISO 13399 code).
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Upper tolerance bound of the measurement.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// Lower tolerance bound of the measurement.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// Nominal (designed) value of the measurement.
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
        /// <see cref="IToolingMeasurement"/>, restoring the measurement
        /// type from the supplied dictionary key and delegating to
        /// <see cref="ToolingMeasurement.Create"/> to materialize the
        /// concrete measurement subtype.
        /// </summary>
        public IToolingMeasurement ToMeasurement(string type)
        {
            var measurement = new ToolingMeasurement();
            measurement.Type = type;
            measurement.Value = Value;
            measurement.SignificantDigits = SignificantDigits;
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Code = Code;
            measurement.Maximum = Maximum;
            measurement.Minimum = Minimum;
            measurement.Nominal = Nominal;
            return ToolingMeasurement.Create(type, measurement);
        }
    }
}
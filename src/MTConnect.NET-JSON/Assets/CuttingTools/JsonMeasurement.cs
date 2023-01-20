// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools.Measurements
{
    public class JsonMeasurement
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("significantDigits")]
        public int SignificantDigits { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("maximum")]
        public double Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public double Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public double Nominal { get; set; }


        public JsonMeasurement() { }

        public JsonMeasurement(Measurement measurement)
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


        public Measurement ToMeasurement()
        {
            var measurement = new Measurement();
            measurement.Type = Type;
            measurement.Value = Value;
            measurement.SignificantDigits = SignificantDigits;
            measurement.Units = Units;
            measurement.NativeUnits = NativeUnits;
            measurement.Code = Code;
            measurement.Maximum = Maximum;
            measurement.Minimum = Minimum;
            measurement.Nominal = Nominal;
            return Measurement.Create(Type, measurement);
        }
    }
}
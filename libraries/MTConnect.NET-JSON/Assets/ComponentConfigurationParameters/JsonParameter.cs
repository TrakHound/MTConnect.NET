// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.ComponentConfigurationParameters
{
    public class JsonParameter
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonParameter() { }

        public JsonParameter(IParameter parameter)
        {
            if (parameter != null)
            {
                Identifier = parameter.Identifier;
                Name = parameter.Name;
                Maximum = parameter.Maximum;
                Minimum = parameter.Minimum;
                Nominal = parameter.Nominal;
                Units = parameter.Units;
                Value = parameter.Value;
            }
        }


        public IParameter ToParameter()
        {
            var parameter = new Parameter();
            parameter.Identifier = Identifier;
            parameter.Name = Name;
            parameter.Maximum = Maximum;
            parameter.Minimum = Minimum;
            parameter.Nominal = Nominal;
            parameter.Units = Units;
            parameter.Value = Value;
            return parameter;
        }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.ComponentConfigurationParameters
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Parameter</c> within a
    /// ComponentConfigurationParameters asset. Mirrors the on-the-wire shape so
    /// the JSON serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="Parameter"/> model.
    /// </summary>
    public class JsonParameter
    {
        /// <summary>
        /// The identifier of the parameter within its set.
        /// </summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// The human-readable name of the parameter.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The maximum permitted value.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The minimum permitted value.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// The nominal (target) value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The engineering units the parameter is expressed in.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The current value of the parameter.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonParameter() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IParameter"/>.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IParameter"/>.
        /// </summary>
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
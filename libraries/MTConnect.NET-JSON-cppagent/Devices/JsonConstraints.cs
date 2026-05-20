// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a data-item
    /// <c>Constraints</c> in the cppagent-compatible shape. Restricts
    /// the allowed value space of a data item via numeric bounds, a
    /// nominal value, an enumeration of permitted literal values, and an
    /// optional change filter. Converts to and from the strongly-typed
    /// <see cref="Constraints"/> model.
    /// </summary>
    public class JsonConstraints
    {
        /// <summary>
        /// Upper numeric bound that values must not exceed.
        /// </summary>
        [JsonPropertyName("Maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// Lower numeric bound that values must not fall below.
        /// </summary>
        [JsonPropertyName("Minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// Nominal (target) value of the data item.
        /// </summary>
        [JsonPropertyName("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// Enumeration of permitted literal values, when the data item
        /// is restricted to a discrete set.
        /// </summary>
        [JsonPropertyName("Value")]
        public List<JsonConstraintsValue> Values { get; set; }

        /// <summary>
        /// Optional change filter applied to the data item.
        /// </summary>
        [JsonPropertyName("Filter")]
        public JsonFilter Filter { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonConstraints() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IConstraints"/>, projecting each permitted value
        /// into a <see cref="JsonConstraintsValue"/>.
        /// </summary>
        public JsonConstraints(IConstraints constraints)
        {
            if (constraints != null)
            {
                Maximum = constraints.Maximum;
                Minimum = constraints.Minimum;
                Nominal = constraints.Nominal;
                if (!constraints.Values.IsNullOrEmpty())
                {
                    var values = new List<JsonConstraintsValue>();
                    foreach (var value in constraints.Values)
                    {
                        values.Add(new JsonConstraintsValue(value));
                    }
                    Values = values;
                }
                if (constraints.Filter != null) Filter = new JsonFilter(constraints.Filter);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IConstraints"/>, flattening the permitted-value
        /// projections back into plain strings.
        /// </summary>
        public IConstraints ToConstraints()
        {
            var constraints = new Constraints();
            constraints.Maximum = Maximum;
            constraints.Minimum = Minimum;
            constraints.Nominal = Nominal;

            if (!Values.IsNullOrEmpty())
            {
                var values = new List<string>();
                foreach (var value in Values)
                {
                    values.Add(value.ToValue());
                }
                constraints.Values = values;
            }

            if (Filter != null) constraints.Filter = Filter.ToFilter();

            return constraints;
        }
    }
}
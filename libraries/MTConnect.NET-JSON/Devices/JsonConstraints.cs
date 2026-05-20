// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a DataItem <c>Constraints</c>, bounding
    /// the values a data item may report through numeric limits, an enumerated
    /// value list, or a filter.
    /// </summary>
    public class JsonConstraints
    {
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
        /// The nominal (expected) value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The set of permitted enumerated values.
        /// </summary>
        [JsonPropertyName("values")]
        public List<string> Values { get; set; }

        /// <summary>
        /// An optional filter further constraining reported values.
        /// </summary>
        [JsonPropertyName("filter")]
        public JsonFilter Filter { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonConstraints() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IConstraints"/>.
        /// </summary>
        public JsonConstraints(IConstraints constraints)
        {
            if (constraints != null)
            {
                Maximum = constraints.Maximum;
                Minimum = constraints.Minimum;
                Nominal = constraints.Nominal;
                if (constraints.Values != null) Values = constraints.Values.ToList();
                if (constraints.Filter != null) Filter = new JsonFilter(constraints.Filter);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Constraints"/>,
        /// copying the numeric bounds and the enumerated value list.
        /// </summary>
        public IConstraints ToConstraints()
        {
            var constraints = new Constraints();
            constraints.Maximum = Maximum;
            constraints.Minimum = Minimum;
            constraints.Nominal = Nominal;
            constraints.Values = Values;
            return constraints;
        }
    }
}
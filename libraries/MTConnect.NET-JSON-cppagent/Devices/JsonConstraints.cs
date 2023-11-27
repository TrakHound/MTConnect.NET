// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonConstraints
    {
        [JsonPropertyName("Maximum")]
        public double? Maximum { get; set; }

        [JsonPropertyName("Minimum")]
        public double? Minimum { get; set; }

        [JsonPropertyName("Nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("Value")]
        public List<JsonConstraintsValue> Values { get; set; }

        [JsonPropertyName("Filter")]
        public JsonFilter Filter { get; set; }


        public JsonConstraints() { }

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
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonConstraints
    {
        [JsonPropertyName("maximum")]
        public string Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public string Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public string Nominal { get; set; }

        [JsonPropertyName("values")]
        public List<string> Values { get; set; }

        [JsonPropertyName("filter")]
        public JsonFilter Filter { get; set; }


        public JsonConstraints() { }

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
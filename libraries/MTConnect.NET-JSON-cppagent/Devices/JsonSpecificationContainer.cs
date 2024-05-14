// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSpecificationContainer
    {
        [JsonPropertyName("Specification")]
        public List<JsonSpecification> Specifications { get; set; }

        [JsonPropertyName("ProcessSpecification")]
        public List<JsonProcessSpecification> ProcessSpecifications { get; set; }
    }
}
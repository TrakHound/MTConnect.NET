// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate that partitions a component's
    /// configuration specifications by kind into typed sibling lists,
    /// keyed by element name (<c>Specification</c>,
    /// <c>ProcessSpecification</c>) per the cppagent shape. Either list
    /// is suppressed when its partition is empty.
    /// </summary>
    public class JsonSpecificationContainer
    {
        /// <summary>
        /// Scalar specifications in the container.
        /// </summary>
        [JsonPropertyName("Specification")]
        public List<JsonSpecification> Specifications { get; set; }

        /// <summary>
        /// Process specifications in the container.
        /// </summary>
        [JsonPropertyName("ProcessSpecification")]
        public List<JsonProcessSpecification> ProcessSpecifications { get; set; }
    }
}
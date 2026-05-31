// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a single permitted-value entry
    /// inside a data-item <c>Constraints.Value</c> list. Wraps a plain
    /// string in an object so the cppagent shape can carry each value
    /// as a discrete object rather than a bare string.
    /// </summary>
    public class JsonConstraintsValue
    {
        /// <summary>
        /// The permitted literal value.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonConstraintsValue() { }

        /// <summary>
        /// Initializes the surrogate from a plain string value.
        /// </summary>
        public JsonConstraintsValue(string value)
        {
            Value = value;
        }


        /// <summary>
        /// Returns the underlying string value.
        /// </summary>
        public string ToValue()
        {
            return Value;
        }
    }
}
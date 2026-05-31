// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Scale (UML ID `_2024x_3_3870182_1764951924679_898861_876`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Scale</c>, the inline variant
    /// carrying its components as a single string (space-separated triple).
    /// Converts to and from the strongly-typed <see cref="Scale"/> model.
    /// </summary>
    public class JsonScale
    {
        /// <summary>
        /// The scale factors as a space-separated triple.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonScale() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed <see cref="IScale"/>.
        /// </summary>
        public JsonScale(IScale scale)
        {
            if (scale != null)
            {
                Value = scale.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IScale"/>.
        /// </summary>
        public IScale ToScale()
        {
            var scale = new Scale();
            scale.Value = Value;
            return scale;
        }
    }
}

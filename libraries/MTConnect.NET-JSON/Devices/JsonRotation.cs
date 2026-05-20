// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Rotation (UML ID `_2024x_3_3870182_1764951373391_145162_327`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Rotation</c>, the inline variant
    /// carrying its angles as a single string (space-separated triple).
    /// Converts to and from the strongly-typed <see cref="Rotation"/> model.
    /// </summary>
    public class JsonRotation
    {
        /// <summary>
        /// The rotation angles as a space-separated triple.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonRotation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IRotation"/>.
        /// </summary>
        public JsonRotation(IRotation rotation)
        {
            if (rotation != null)
            {
                Value = rotation.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IRotation"/>.
        /// </summary>
        public IRotation ToRotation()
        {
            var rotation = new Rotation();
            rotation.Value = Value;
            return rotation;
        }
    }
}

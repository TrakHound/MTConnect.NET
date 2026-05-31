// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Origin (UML ID `_19_0_3_45f01b9_1579107743274_159386_163610`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an <c>Origin</c>, the inline variant
    /// carrying its coordinates as a single string (space-separated triple).
    /// Converts to and from the strongly-typed <see cref="Origin"/> model.
    /// </summary>
    public class JsonOrigin
    {
        /// <summary>
        /// The origin coordinates as a space-separated triple.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonOrigin() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IOrigin"/>.
        /// </summary>
        public JsonOrigin(IOrigin origin)
        {
            if (origin != null)
            {
                Value = origin.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IOrigin"/>.
        /// </summary>
        public IOrigin ToOrigin()
        {
            var origin = new Origin();
            origin.Value = Value;
            return origin;
        }
    }
}

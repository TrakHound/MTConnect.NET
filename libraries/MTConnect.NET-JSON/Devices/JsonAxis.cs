// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Axis (UML ID `_2024x_3_3870182_1764951682685_285104_645`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an <c>Axis</c>, the inline variant
    /// carrying its direction vector as a single string (space-separated
    /// triple). Converts to and from the strongly-typed <see cref="Axis"/>
    /// model.
    /// </summary>
    public class JsonAxis
    {
        /// <summary>
        /// The axis direction vector as a space-separated triple.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAxis() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed <see cref="IAxis"/>.
        /// </summary>
        public JsonAxis(IAxis axis)
        {
            if (axis != null)
            {
                Value = axis.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IAxis"/>.
        /// </summary>
        public IAxis ToAxis()
        {
            var axis = new Axis();
            axis.Value = Value;
            return axis;
        }
    }
}

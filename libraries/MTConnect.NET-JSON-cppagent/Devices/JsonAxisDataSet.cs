// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class AxisDataSet (UML ID `_2024x_68e0225_1727807971743_962437_23839`).

// JSON shape: flat object with PascalCase keys ({"X": v, "Y": v, "Z": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Axis</c>
    /// expressed as a data set in the cppagent-compatible shape. The
    /// flat <c>{"X": v, "Y": v, "Z": v}</c> PascalCase object encoding
    /// matches the cppagent v2 DataSet convention. Converts to and from
    /// the strongly-typed <see cref="AxisDataSet"/> model.
    /// </summary>
    public class JsonAxisDataSet
    {
        /// <summary>
        /// Component of the axis along the X direction.
        /// </summary>
        [JsonPropertyName("X")]
        public double X { get; set; }

        /// <summary>
        /// Component of the axis along the Y direction.
        /// </summary>
        [JsonPropertyName("Y")]
        public double Y { get; set; }

        /// <summary>
        /// Component of the axis along the Z direction.
        /// </summary>
        [JsonPropertyName("Z")]
        public double Z { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAxisDataSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IAxisDataSet"/>.
        /// </summary>
        public JsonAxisDataSet(IAxisDataSet dataSet)
        {
            if (dataSet != null)
            {
                X = dataSet.X;
                Y = dataSet.Y;
                Z = dataSet.Z;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IAxisDataSet"/>.
        /// </summary>
        public IAxisDataSet ToAxisDataSet()
        {
            var dataSet = new AxisDataSet();
            dataSet.X = X;
            dataSet.Y = Y;
            dataSet.Z = Z;
            return dataSet;
        }
    }
}

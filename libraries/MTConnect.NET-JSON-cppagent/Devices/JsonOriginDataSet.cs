// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class OriginDataSet (UML ID `_2024x_68e0225_1727808122960_76782_23993`).

// JSON shape: flat object with PascalCase keys ({"X": v, "Y": v, "Z": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Origin</c>
    /// expressed as a data set in the cppagent-compatible shape. The
    /// flat <c>{"X": v, "Y": v, "Z": v}</c> PascalCase object encoding
    /// matches the cppagent v2 DataSet convention; the values are
    /// serialized as strings to round-trip equipment-native formatting.
    /// Converts to and from the strongly-typed
    /// <see cref="OriginDataSet"/> model.
    /// </summary>
    public class JsonOriginDataSet
    {
        /// <summary>
        /// Component of the origin along the X direction.
        /// </summary>
        [JsonPropertyName("X")]
        public string X { get; set; }

        /// <summary>
        /// Component of the origin along the Y direction.
        /// </summary>
        [JsonPropertyName("Y")]
        public string Y { get; set; }

        /// <summary>
        /// Component of the origin along the Z direction.
        /// </summary>
        [JsonPropertyName("Z")]
        public string Z { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonOriginDataSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IOriginDataSet"/>.
        /// </summary>
        public JsonOriginDataSet(IOriginDataSet dataSet)
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
        /// <see cref="IOriginDataSet"/>.
        /// </summary>
        public IOriginDataSet ToOriginDataSet()
        {
            var dataSet = new OriginDataSet();
            dataSet.X = X;
            dataSet.Y = Y;
            dataSet.Z = Z;
            return dataSet;
        }
    }
}

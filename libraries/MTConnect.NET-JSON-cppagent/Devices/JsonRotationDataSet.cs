// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class RotationDataSet (UML ID `_2024x_68e0225_1727807509860_595526_23700`).

// JSON shape: flat object with PascalCase keys ({"A": v, "B": v, "C": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Rotation</c>
    /// expressed as a data set in the cppagent-compatible shape. The
    /// flat <c>{"A": v, "B": v, "C": v}</c> PascalCase object encoding
    /// matches the cppagent v2 DataSet convention; the angular values
    /// are serialized as strings to round-trip equipment-native
    /// formatting. Converts to and from the strongly-typed
    /// <see cref="RotationDataSet"/> model.
    /// </summary>
    public class JsonRotationDataSet
    {
        /// <summary>
        /// Rotation around the A axis.
        /// </summary>
        [JsonPropertyName("A")]
        public string A { get; set; }

        /// <summary>
        /// Rotation around the B axis.
        /// </summary>
        [JsonPropertyName("B")]
        public string B { get; set; }

        /// <summary>
        /// Rotation around the C axis.
        /// </summary>
        [JsonPropertyName("C")]
        public string C { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonRotationDataSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IRotationDataSet"/>.
        /// </summary>
        public JsonRotationDataSet(IRotationDataSet dataSet)
        {
            if (dataSet != null)
            {
                A = dataSet.A;
                B = dataSet.B;
                C = dataSet.C;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IRotationDataSet"/>.
        /// </summary>
        public IRotationDataSet ToRotationDataSet()
        {
            var dataSet = new RotationDataSet();
            dataSet.A = A;
            dataSet.B = B;
            dataSet.C = C;
            return dataSet;
        }
    }
}

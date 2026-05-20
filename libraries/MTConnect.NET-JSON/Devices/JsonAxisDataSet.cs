// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class AxisDataSet (UML ID `_2024x_68e0225_1727807971743_962437_23839`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an <c>AxisDataSet</c>, the variant of
    /// a motion axis whose components are reported per axis. Converts to and
    /// from the strongly-typed <see cref="AxisDataSet"/> model.
    /// </summary>
    public class JsonAxisDataSet
    {
        /// <summary>
        /// The X component of the axis direction vector.
        /// </summary>
        [JsonPropertyName("x")]
        public double X { get; set; }

        /// <summary>
        /// The Y component of the axis direction vector.
        /// </summary>
        [JsonPropertyName("y")]
        public double Y { get; set; }

        /// <summary>
        /// The Z component of the axis direction vector.
        /// </summary>
        [JsonPropertyName("z")]
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

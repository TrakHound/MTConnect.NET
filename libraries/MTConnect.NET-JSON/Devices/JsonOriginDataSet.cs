// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class OriginDataSet (UML ID `_2024x_68e0225_1727808122960_76782_23993`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an <c>OriginDataSet</c>, the variant
    /// of an origin whose coordinates are references to data items rather than
    /// inline numbers. Converts to and from the strongly-typed
    /// <see cref="OriginDataSet"/> model.
    /// </summary>
    public class JsonOriginDataSet
    {
        /// <summary>
        /// Reference to the data item or value supplying the X coordinate.
        /// </summary>
        [JsonPropertyName("x")]
        public string X { get; set; }

        /// <summary>
        /// Reference to the data item or value supplying the Y coordinate.
        /// </summary>
        [JsonPropertyName("y")]
        public string Y { get; set; }

        /// <summary>
        /// Reference to the data item or value supplying the Z coordinate.
        /// </summary>
        [JsonPropertyName("z")]
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

// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class RotationDataSet (UML ID `_2024x_68e0225_1727807509860_595526_23700`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>RotationDataSet</c>, the variant
    /// of a rotation whose angles are references to data items rather than
    /// inline numbers. Converts to and from the strongly-typed
    /// <see cref="RotationDataSet"/> model.
    /// </summary>
    public class JsonRotationDataSet
    {
        /// <summary>
        /// Reference to the data item or value supplying the A rotation angle.
        /// </summary>
        [JsonPropertyName("a")]
        public string A { get; set; }

        /// <summary>
        /// Reference to the data item or value supplying the B rotation angle.
        /// </summary>
        [JsonPropertyName("b")]
        public string B { get; set; }

        /// <summary>
        /// Reference to the data item or value supplying the C rotation angle.
        /// </summary>
        [JsonPropertyName("c")]
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

// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class ScaleDataSet (UML ID `_2024x_68e0225_1727807698383_716499_23819`).

// JSON shape: flat object with PascalCase keys ({"X": v, "Y": v, "Z": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration
    /// <c>SolidModel.Scale</c> expressed as a data set in the
    /// cppagent-compatible shape. The flat
    /// <c>{"X": v, "Y": v, "Z": v}</c> PascalCase object encoding
    /// matches the cppagent v2 DataSet convention. Converts to and from
    /// the strongly-typed <see cref="ScaleDataSet"/> model.
    /// </summary>
    public class JsonScaleDataSet
    {
        /// <summary>
        /// Scale factor along the X axis.
        /// </summary>
        [JsonPropertyName("X")]
        public double X { get; set; }

        /// <summary>
        /// Scale factor along the Y axis.
        /// </summary>
        [JsonPropertyName("Y")]
        public double Y { get; set; }

        /// <summary>
        /// Scale factor along the Z axis.
        /// </summary>
        [JsonPropertyName("Z")]
        public double Z { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonScaleDataSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IScaleDataSet"/>.
        /// </summary>
        public JsonScaleDataSet(IScaleDataSet dataSet)
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
        /// <see cref="IScaleDataSet"/>.
        /// </summary>
        public IScaleDataSet ToScaleDataSet()
        {
            var dataSet = new ScaleDataSet();
            dataSet.X = X;
            dataSet.Y = Y;
            dataSet.Z = Z;
            return dataSet;
        }
    }
}

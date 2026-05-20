// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class TranslationDataSet (UML ID `_2024x_68e0225_1727807350445_154414_23573`).

// JSON shape: flat object with PascalCase keys ({"X": v, "Y": v, "Z": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration
    /// <c>Translation</c> expressed as a data set in the
    /// cppagent-compatible shape. The flat
    /// <c>{"X": v, "Y": v, "Z": v}</c> PascalCase object encoding
    /// matches the cppagent v2 DataSet convention; the values are
    /// serialized as strings to round-trip equipment-native formatting.
    /// Converts to and from the strongly-typed
    /// <see cref="TranslationDataSet"/> model.
    /// </summary>
    public class JsonTranslationDataSet
    {
        /// <summary>
        /// Translation along the X direction.
        /// </summary>
        [JsonPropertyName("X")]
        public string X { get; set; }

        /// <summary>
        /// Translation along the Y direction.
        /// </summary>
        [JsonPropertyName("Y")]
        public string Y { get; set; }

        /// <summary>
        /// Translation along the Z direction.
        /// </summary>
        [JsonPropertyName("Z")]
        public string Z { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonTranslationDataSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ITranslationDataSet"/>.
        /// </summary>
        public JsonTranslationDataSet(ITranslationDataSet dataSet)
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
        /// <see cref="ITranslationDataSet"/>.
        /// </summary>
        public ITranslationDataSet ToTranslationDataSet()
        {
            var dataSet = new TranslationDataSet();
            dataSet.X = X;
            dataSet.Y = Y;
            dataSet.Z = Z;
            return dataSet;
        }
    }
}

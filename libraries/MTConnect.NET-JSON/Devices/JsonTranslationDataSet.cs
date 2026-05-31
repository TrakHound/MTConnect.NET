// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class TranslationDataSet (UML ID `_2024x_68e0225_1727807350445_154414_23573`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>TranslationDataSet</c>, the
    /// variant of a translation whose offsets are references to data items
    /// rather than inline numbers. Converts to and from the strongly-typed
    /// <see cref="TranslationDataSet"/> model.
    /// </summary>
    public class JsonTranslationDataSet
    {
        /// <summary>
        /// Reference to the data item or value supplying the X offset.
        /// </summary>
        [JsonPropertyName("x")]
        public string X { get; set; }

        /// <summary>
        /// Reference to the data item or value supplying the Y offset.
        /// </summary>
        [JsonPropertyName("y")]
        public string Y { get; set; }

        /// <summary>
        /// Reference to the data item or value supplying the Z offset.
        /// </summary>
        [JsonPropertyName("z")]
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

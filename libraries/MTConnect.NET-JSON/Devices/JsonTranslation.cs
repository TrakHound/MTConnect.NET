// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Translation (UML ID `_2024x_3_3870182_1764951167326_754957_161`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Translation</c>, the inline
    /// variant carrying its offsets as a single string (space-separated
    /// triple). Converts to and from the strongly-typed
    /// <see cref="Translation"/> model.
    /// </summary>
    public class JsonTranslation
    {
        /// <summary>
        /// The translation offsets as a space-separated triple.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonTranslation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ITranslation"/>.
        /// </summary>
        public JsonTranslation(ITranslation translation)
        {
            if (translation != null)
            {
                Value = translation.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="ITranslation"/>.
        /// </summary>
        public ITranslation ToTranslation()
        {
            var translation = new Translation();
            translation.Value = Value;
            return translation;
        }
    }
}

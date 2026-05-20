// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a single name/value
    /// <c>FileProperty</c> attached to a File asset.
    /// </summary>
    public class JsonFileProperty
    {
        /// <summary>
        /// The property name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The property value.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonFileProperty() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IFileProperty"/>.
        /// </summary>
        public JsonFileProperty(IFileProperty fileProperty)
        {
            if (fileProperty != null)
            {
                Name = fileProperty.Name;
                Value = fileProperty.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IFileProperty"/>.
        /// </summary>
        public IFileProperty ToFileProperty()
        {
            var fileProperty = new FileProperty();
            fileProperty.Name = Name;
            fileProperty.Value = Value;
            return fileProperty;
        }
    }
}
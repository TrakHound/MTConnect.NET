// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a <c>FileProperty</c>, a single
    /// name/value pair describing a file asset. Converts to and from the
    /// strongly-typed <see cref="FileProperty"/> model.
    /// </summary>
    public class JsonFileProperty
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the property.
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
        /// Converts this surrogate to a strongly-typed <see cref="IFileProperty"/>.
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
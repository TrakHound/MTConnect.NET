// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// Common JSON serialization shape for the File and FileArchetype assets,
    /// carrying the file's metadata and properties.
    /// </summary>
    public abstract class JsonAbstractFileAsset
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The MIME media type of the file's contents.
        /// </summary>
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The category of application the file is associated with, serialized
        /// as the enumeration name.
        /// </summary>
        [JsonPropertyName("applicationCategory")]
        public string ApplicationCategory { get; set; }

        /// <summary>
        /// The type of application the file is associated with, serialized as
        /// the enumeration name.
        /// </summary>
        [JsonPropertyName("applicationType")]
        public string ApplicationType { get; set; }

        /// <summary>
        /// The key/value properties describing the file.
        /// </summary>
        [JsonPropertyName("fileProperties")]
        public IEnumerable<JsonFileProperty> FileProperties { get; set; }

        /// <summary>
        /// The comments associated with the file.
        /// </summary>
        [JsonPropertyName("fileComments")]
        public IEnumerable<JsonFileComment> FileComments { get; set; }
    }
}
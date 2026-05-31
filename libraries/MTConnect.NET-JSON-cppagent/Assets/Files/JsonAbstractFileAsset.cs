// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// Shared JSON serialization surrogate for the attributes common to
    /// both the abstract and concrete <c>File</c> assets in the
    /// cppagent-compatible shape. Extends <see cref="JsonAsset"/> with
    /// file-specific identification (name, media type, application
    /// category and type) and the optional file properties and comments
    /// collections.
    /// </summary>
    public abstract class JsonAbstractFileAsset : JsonAsset
    {
        /// <summary>
        /// The display name of the file.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// IANA media type of the file content (for example
        /// <c>application/pdf</c>).
        /// </summary>
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The high-level application category the file belongs to (for
        /// example DESIGN, MAINTENANCE).
        /// </summary>
        [JsonPropertyName("applicationCategory")]
        public string ApplicationCategory { get; set; }

        /// <summary>
        /// The specific application type the file serves within its
        /// category.
        /// </summary>
        [JsonPropertyName("applicationType")]
        public string ApplicationType { get; set; }

        /// <summary>
        /// Custom name/value properties attached to the file.
        /// </summary>
        [JsonPropertyName("FileProperties")]
        public IEnumerable<JsonFileProperty> FileProperties { get; set; }

        /// <summary>
        /// Human-authored comments attached to the file.
        /// </summary>
        [JsonPropertyName("FileComments")]
        public IEnumerable<JsonFileComment> FileComments { get; set; }
    }
}
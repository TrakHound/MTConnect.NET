// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a timestamped
    /// <c>FileComment</c> attached to a File asset.
    /// </summary>
    public class JsonFileComment
    {
        /// <summary>
        /// The timestamp when the comment was authored.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The free-form comment text.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonFileComment() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IFileComment"/>.
        /// </summary>
        public JsonFileComment(IFileComment fileComment)
        {
            if (fileComment != null)
            {
                Timestamp = fileComment.Timestamp;
                Value = fileComment.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IFileComment"/>.
        /// </summary>
        public IFileComment ToFileComment()
        {
            var fileComment = new FileComment();
            fileComment.Timestamp = Timestamp;
            fileComment.Value = Value;
            return fileComment;
        }
    }
}
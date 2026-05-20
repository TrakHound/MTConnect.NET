// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a <c>FileComment</c>, a timestamped
    /// remark on a file asset. Converts to and from the strongly-typed
    /// <see cref="FileComment"/> model.
    /// </summary>
    public class JsonFileComment
    {
        /// <summary>
        /// The time the comment was made.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The comment text.
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
        /// Converts this surrogate to a strongly-typed <see cref="IFileComment"/>.
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
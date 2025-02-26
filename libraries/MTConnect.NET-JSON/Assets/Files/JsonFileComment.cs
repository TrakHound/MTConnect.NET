// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    public class JsonFileComment
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonFileComment() { }

        public JsonFileComment(IFileComment fileComment)
        {
            if (fileComment != null)
            {
                Timestamp = fileComment.Timestamp;
                Value = fileComment.Value;
            }
        }


        public IFileComment ToFileComment()
        {
            var fileComment = new FileComment();
            fileComment.Timestamp = Timestamp;
            fileComment.Value = Value;
            return fileComment;
        }
    }
}
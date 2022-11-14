// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Files
{
    public class JsonFileComment
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonFileComment() { }

        public JsonFileComment(FileComment fileComment)
        {
            if (fileComment != null)
            {
                Timestamp = fileComment.Timestamp;
                Value = fileComment.Value;
            }
        }


        public FileComment ToFileComment()
        {
            var fileComment = new FileComment();
            fileComment.Timestamp = Timestamp;
            fileComment.Value = Value;
            return fileComment;
        }
    }
}

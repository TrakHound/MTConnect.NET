// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Formatters
{
    public struct FormattedDocumentResult
    {
        public string Content { get; set; }

        public string ContentType { get; set; }

        public bool Success { get; set; }

        public long ResponseDuration { get; set; }


        public FormattedDocumentResult(string content, string contentType, bool success = true)
        {
            Content = content;
            ContentType = contentType;
            Success = success;
            ResponseDuration = 0;
        }

        public static FormattedDocumentResult Error()
        {
            return new FormattedDocumentResult(null, null, false);
        }
    }
}

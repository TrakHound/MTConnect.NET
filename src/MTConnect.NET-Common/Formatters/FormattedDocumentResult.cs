// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public struct FormattedDocumentResult
    {
        public string Content { get; set; }

        public string ContentType { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public long ResponseDuration { get; set; }


        public FormattedDocumentResult(string content, string contentType, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Content = content;
            ContentType = contentType;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        public static FormattedDocumentResult Successful(string content, string contentType, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormattedDocumentResult(content, contentType, true, messages);
        }

        public static FormattedDocumentResult Successful(string content, string contentType, IEnumerable<string> messages)
        {
            return new FormattedDocumentResult(content, contentType, true, messages);
        }


        public static FormattedDocumentResult Warning(string content, string contentType, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormattedDocumentResult(content, contentType, true, null, warnings);
        }

        public static FormattedDocumentResult Warning(string content, string contentType, IEnumerable<string> warnings)
        {
            return new FormattedDocumentResult(content, contentType, true, null, warnings);
        }


        public static FormattedDocumentResult Error(string content, string contentType, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormattedDocumentResult(content, contentType, false, null, null, errors);
        }

        public static FormattedDocumentResult Error(string content, string contentType, IEnumerable<string> errors)
        {
            return new FormattedDocumentResult(content, contentType, false, null, null, errors);
        }

        public static FormattedDocumentResult Error(IEnumerable<string> errors = null)
        {
            return new FormattedDocumentResult(null, null, false, null, null, errors);
        }
    }
}

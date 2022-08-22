// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public struct FormattedDocumentWriteResult
    {
        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public double ResponseDuration { get; set; }


        public FormattedDocumentWriteResult(byte[] content, string contentType, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Content = content;
            ContentType = contentType;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        public static FormattedDocumentWriteResult Successful(byte[] content, string contentType, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormattedDocumentWriteResult(content, contentType, true, messages);
        }

        public static FormattedDocumentWriteResult Successful(byte[] content, string contentType, IEnumerable<string> messages)
        {
            return new FormattedDocumentWriteResult(content, contentType, true, messages);
        }


        public static FormattedDocumentWriteResult Warning(byte[] content, string contentType, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormattedDocumentWriteResult(content, contentType, true, null, warnings);
        }

        public static FormattedDocumentWriteResult Warning(byte[] content, string contentType, IEnumerable<string> warnings)
        {
            return new FormattedDocumentWriteResult(content, contentType, true, null, warnings);
        }


        public static FormattedDocumentWriteResult Error(byte[] content, string contentType, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormattedDocumentWriteResult(content, contentType, false, null, null, errors);
        }

        public static FormattedDocumentWriteResult Error(byte[] content, string contentType, IEnumerable<string> errors)
        {
            return new FormattedDocumentWriteResult(content, contentType, false, null, null, errors);
        }

        public static FormattedDocumentWriteResult Error(IEnumerable<string> errors = null)
        {
            return new FormattedDocumentWriteResult(null, null, false, null, null, errors);
        }
    }
}

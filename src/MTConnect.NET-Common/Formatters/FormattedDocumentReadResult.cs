// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public struct FormattedDocumentReadResult<T>
    {
        public T Document { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public double ResponseDuration { get; set; }


        public FormattedDocumentReadResult(T document, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Document = document;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        public static FormattedDocumentReadResult<T> Successful(T document, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormattedDocumentReadResult<T>(document, true, messages);
        }

        public static FormattedDocumentReadResult<T> Successful(T document, IEnumerable<string> messages)
        {
            return new FormattedDocumentReadResult<T>(document, true, messages);
        }


        public static FormattedDocumentReadResult<T> Warning(T document, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormattedDocumentReadResult<T>(document, true, null, warnings);
        }

        public static FormattedDocumentReadResult<T> Warning(T document, IEnumerable<string> warnings)
        {
            return new FormattedDocumentReadResult<T>(document, true, null, warnings);
        }


        public static FormattedDocumentReadResult<T> Error(T document, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormattedDocumentReadResult<T>(document, false, null, null, errors);
        }

        public static FormattedDocumentReadResult<T> Error(T document, IEnumerable<string> errors)
        {
            return new FormattedDocumentReadResult<T>(document, false, null, null, errors);
        }

        public static FormattedDocumentReadResult<T> Error(IEnumerable<string> errors = null)
        {
            return new FormattedDocumentReadResult<T>(default, false, null, null, errors);
        }
    }
}

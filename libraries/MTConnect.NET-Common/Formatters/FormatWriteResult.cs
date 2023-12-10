// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public struct FormatWriteResult
    {
        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public double ResponseDuration { get; set; }


        public FormatWriteResult(byte[] content, string contentType, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Content = content;
            ContentType = contentType;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        public static FormatWriteResult Successful(byte[] content, string contentType, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormatWriteResult(content, contentType, true, messages);
        }

        public static FormatWriteResult Successful(byte[] content, string contentType, IEnumerable<string> messages)
        {
            return new FormatWriteResult(content, contentType, true, messages);
        }


        public static FormatWriteResult Warning(byte[] content, string contentType, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormatWriteResult(content, contentType, true, null, warnings);
        }

        public static FormatWriteResult Warning(byte[] content, string contentType, IEnumerable<string> warnings)
        {
            return new FormatWriteResult(content, contentType, true, null, warnings);
        }


        public static FormatWriteResult Error(byte[] content, string contentType, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormatWriteResult(content, contentType, false, null, null, errors);
        }

        public static FormatWriteResult Error(byte[] content, string contentType, IEnumerable<string> errors)
        {
            return new FormatWriteResult(content, contentType, false, null, null, errors);
        }

        public static FormatWriteResult Error(IEnumerable<string> errors = null)
        {
            return new FormatWriteResult(null, null, false, null, null, errors);
        }
    }
}
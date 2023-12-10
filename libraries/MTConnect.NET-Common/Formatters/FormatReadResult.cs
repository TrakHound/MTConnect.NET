// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public struct FormatReadResult<T>
    {
        public T Content { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public double ResponseDuration { get; set; }


        public FormatReadResult(T content, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Content = content;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        public static FormatReadResult<T> Successful(T content, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormatReadResult<T>(content, true, messages);
        }

        public static FormatReadResult<T> Successful(T content, IEnumerable<string> messages)
        {
            return new FormatReadResult<T>(content, true, messages);
        }


        public static FormatReadResult<T> Warning(T content, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormatReadResult<T>(content, true, null, warnings);
        }

        public static FormatReadResult<T> Warning(T content, IEnumerable<string> warnings)
        {
            return new FormatReadResult<T>(content, true, null, warnings);
        }


        public static FormatReadResult<T> Error(T content, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormatReadResult<T>(content, false, null, null, errors);
        }

        public static FormatReadResult<T> Error(T content, IEnumerable<string> errors)
        {
            return new FormatReadResult<T>(content, false, null, null, errors);
        }

        public static FormatReadResult<T> Error(IEnumerable<string> errors = null)
        {
            return new FormatReadResult<T>(default, false, null, null, errors);
        }
    }
}
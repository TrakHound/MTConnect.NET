// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    /// <summary>
    /// The outcome of deserializing a document into an entity of type <typeparamref name="T"/>, carrying the parsed content, a success flag, and any diagnostic messages.
    /// </summary>
    /// <typeparam name="T">The deserialized entity type.</typeparam>
    public struct FormatReadResult<T> : IFormatReadResult
    {
        /// <summary>
        /// The deserialized entity, or the type default when deserialization failed.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// The runtime type of <see cref="Content"/>.
        /// </summary>
        public Type ContentType => typeof(T);

        /// <summary>
        /// Indicates whether deserialization succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Informational messages produced during deserialization.
        /// </summary>
        public IEnumerable<string> Messages { get; set; }

        /// <summary>
        /// Non-fatal warnings produced during deserialization.
        /// </summary>
        public IEnumerable<string> Warnings { get; set; }

        /// <summary>
        /// Errors that caused or accompanied a failed deserialization.
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        /// <summary>
        /// The time taken to produce the result, in milliseconds.
        /// </summary>
        public double ResponseDuration { get; set; }


        /// <summary>
        /// Initializes a read result from its constituent parts; the response duration starts at zero.
        /// </summary>
        /// <param name="content">The deserialized entity.</param>
        /// <param name="success">Whether deserialization succeeded.</param>
        /// <param name="messages">Optional informational messages.</param>
        /// <param name="warnings">Optional non-fatal warnings.</param>
        /// <param name="errors">Optional errors.</param>
        public FormatReadResult(T content, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Content = content;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        /// <summary>
        /// Creates a successful result with an optional single informational message.
        /// </summary>
        /// <param name="content">The deserialized entity.</param>
        /// <param name="message">An optional informational message.</param>
        public static FormatReadResult<T> Successful(T content, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormatReadResult<T>(content, true, messages);
        }

        /// <summary>
        /// Creates a successful result carrying a collection of informational messages.
        /// </summary>
        /// <param name="content">The deserialized entity.</param>
        /// <param name="messages">The informational messages.</param>
        public static FormatReadResult<T> Successful(T content, IEnumerable<string> messages)
        {
            return new FormatReadResult<T>(content, true, messages);
        }


        /// <summary>
        /// Creates a result that succeeded but carries an optional single warning.
        /// </summary>
        /// <param name="content">The deserialized entity.</param>
        /// <param name="warning">An optional warning message.</param>
        public static FormatReadResult<T> Warning(T content, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormatReadResult<T>(content, true, null, warnings);
        }

        /// <summary>
        /// Creates a result that succeeded but carries a collection of warnings.
        /// </summary>
        /// <param name="content">The deserialized entity.</param>
        /// <param name="warnings">The warning messages.</param>
        public static FormatReadResult<T> Warning(T content, IEnumerable<string> warnings)
        {
            return new FormatReadResult<T>(content, true, null, warnings);
        }


        /// <summary>
        /// Creates a failed result that still carries the partially parsed content and an optional single error.
        /// </summary>
        /// <param name="content">The partially parsed entity, if any.</param>
        /// <param name="error">An optional error message.</param>
        public static FormatReadResult<T> Error(T content, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormatReadResult<T>(content, false, null, null, errors);
        }

        /// <summary>
        /// Creates a failed result that still carries the partially parsed content and a collection of errors.
        /// </summary>
        /// <param name="content">The partially parsed entity, if any.</param>
        /// <param name="errors">The error messages.</param>
        public static FormatReadResult<T> Error(T content, IEnumerable<string> errors)
        {
            return new FormatReadResult<T>(content, false, null, null, errors);
        }

        /// <summary>
        /// Creates a failed result with no content, carrying only the supplied errors.
        /// </summary>
        /// <param name="errors">The error messages.</param>
        public static FormatReadResult<T> Error(IEnumerable<string> errors = null)
        {
            return new FormatReadResult<T>(default, false, null, null, errors);
        }
    }
}
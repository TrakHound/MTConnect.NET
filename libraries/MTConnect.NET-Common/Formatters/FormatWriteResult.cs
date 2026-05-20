// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;

namespace MTConnect.Formatters
{
    /// <summary>
    /// The outcome of serializing an entity to a document, carrying the produced content stream, its content type, a success flag, and any diagnostic messages.
    /// </summary>
    public struct FormatWriteResult
    {
        /// <summary>
        /// The serialized output stream, or null when serialization failed.
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// The MIME content type of <see cref="Content"/>.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Indicates whether serialization succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Informational messages produced during serialization.
        /// </summary>
        public IEnumerable<string> Messages { get; set; }

        /// <summary>
        /// Non-fatal warnings produced during serialization.
        /// </summary>
        public IEnumerable<string> Warnings { get; set; }

        /// <summary>
        /// Errors that caused or accompanied a failed serialization.
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        /// <summary>
        /// The time taken to produce the result, in milliseconds.
        /// </summary>
        public double ResponseDuration { get; set; }


        /// <summary>
        /// Initializes a write result from its constituent parts; the response duration starts at zero.
        /// </summary>
        /// <param name="content">The serialized output stream.</param>
        /// <param name="contentType">The MIME content type of the output.</param>
        /// <param name="success">Whether serialization succeeded.</param>
        /// <param name="messages">Optional informational messages.</param>
        /// <param name="warnings">Optional non-fatal warnings.</param>
        /// <param name="errors">Optional errors.</param>
        public FormatWriteResult(Stream content, string contentType, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Content = content;
            ContentType = contentType;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        /// <summary>
        /// Creates a successful result with optional single informational message.
        /// </summary>
        /// <param name="content">The serialized output stream.</param>
        /// <param name="contentType">The MIME content type.</param>
        /// <param name="message">An optional informational message.</param>
        public static FormatWriteResult Successful(Stream content, string contentType, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormatWriteResult(content, contentType, true, messages);
        }

        /// <summary>
        /// Creates a successful result carrying a collection of informational messages.
        /// </summary>
        /// <param name="content">The serialized output stream.</param>
        /// <param name="contentType">The MIME content type.</param>
        /// <param name="messages">The informational messages.</param>
        public static FormatWriteResult Successful(Stream content, string contentType, IEnumerable<string> messages)
        {
            return new FormatWriteResult(content, contentType, true, messages);
        }


        /// <summary>
        /// Creates a result that succeeded but carries an optional single warning.
        /// </summary>
        /// <param name="content">The serialized output stream.</param>
        /// <param name="contentType">The MIME content type.</param>
        /// <param name="warning">An optional warning message.</param>
        public static FormatWriteResult Warning(Stream content, string contentType, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormatWriteResult(content, contentType, true, null, warnings);
        }

        /// <summary>
        /// Creates a result that succeeded but carries a collection of warnings.
        /// </summary>
        /// <param name="content">The serialized output stream.</param>
        /// <param name="contentType">The MIME content type.</param>
        /// <param name="warnings">The warning messages.</param>
        public static FormatWriteResult Warning(Stream content, string contentType, IEnumerable<string> warnings)
        {
            return new FormatWriteResult(content, contentType, true, null, warnings);
        }


        /// <summary>
        /// Creates a failed result with the produced content and an optional single error.
        /// </summary>
        /// <param name="content">The partial output stream, if any.</param>
        /// <param name="contentType">The MIME content type.</param>
        /// <param name="error">An optional error message.</param>
        public static FormatWriteResult Error(Stream content, string contentType, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormatWriteResult(content, contentType, false, null, null, errors);
        }

        /// <summary>
        /// Creates a failed result with the produced content and a collection of errors.
        /// </summary>
        /// <param name="content">The partial output stream, if any.</param>
        /// <param name="contentType">The MIME content type.</param>
        /// <param name="errors">The error messages.</param>
        public static FormatWriteResult Error(Stream content, string contentType, IEnumerable<string> errors)
        {
            return new FormatWriteResult(content, contentType, false, null, null, errors);
        }

        /// <summary>
        /// Creates a failed result with no content, carrying only the supplied errors.
        /// </summary>
        /// <param name="errors">The error messages.</param>
        public static FormatWriteResult Error(IEnumerable<string> errors = null)
        {
            return new FormatWriteResult(null, null, false, null, null, errors);
        }
    }
}
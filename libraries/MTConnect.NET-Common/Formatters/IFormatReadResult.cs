// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    /// <summary>
    /// The non-generic view of a deserialization outcome, exposing the parsed content type, success flag, and diagnostic messages independently of the parsed entity type.
    /// </summary>
    public interface IFormatReadResult
    {
        /// <summary>
        /// The runtime type of the deserialized content.
        /// </summary>
        Type ContentType { get; }

        /// <summary>
        /// Indicates whether deserialization succeeded.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Informational messages produced during deserialization.
        /// </summary>
        IEnumerable<string> Messages { get; }

        /// <summary>
        /// Non-fatal warnings produced during deserialization.
        /// </summary>
        IEnumerable<string> Warnings { get; }

        /// <summary>
        /// Errors that caused or accompanied a failed deserialization.
        /// </summary>
        IEnumerable<string> Errors { get; }

        /// <summary>
        /// The time taken to produce the result, in milliseconds.
        /// </summary>
        double ResponseDuration { get; }
    }
}
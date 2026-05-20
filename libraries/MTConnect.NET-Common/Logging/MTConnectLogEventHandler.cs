// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Logging
{
    /// <summary>
    /// Callback raised when an MTConnect component emits a log entry, carrying the severity, message text, and an optional log channel identifier.
    /// </summary>
    /// <param name="sender">The component that produced the log entry.</param>
    /// <param name="logLevel">The severity of the entry.</param>
    /// <param name="message">The log message text.</param>
    /// <param name="logId">An optional identifier for the originating log channel or category.</param>
    public delegate void MTConnectLogEventHandler(object sender, MTConnectLogLevel logLevel, string message, string logId = null);
}

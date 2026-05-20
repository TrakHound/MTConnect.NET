// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Logging
{
    /// <summary>
    /// Severity of a log entry emitted by an MTConnect component, ordered from most to least severe.
    /// </summary>
    public enum MTConnectLogLevel
    {
        /// <summary>
        /// An unrecoverable error that typically forces the component to stop.
        /// </summary>
        Fatal,

        /// <summary>
        /// A failure that prevents an operation from completing but does not stop the component.
        /// </summary>
        Error,

        /// <summary>
        /// An abnormal condition that does not prevent operation but warrants attention.
        /// </summary>
        Warning,

        /// <summary>
        /// A normal operational message describing significant activity.
        /// </summary>
        Information,

        /// <summary>
        /// Diagnostic detail useful while investigating behavior.
        /// </summary>
        Debug,

        /// <summary>
        /// The most verbose detail, tracing fine-grained execution flow.
        /// </summary>
        Trace
    }
}

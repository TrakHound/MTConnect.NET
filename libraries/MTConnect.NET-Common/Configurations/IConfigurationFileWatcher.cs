// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration File Watcher that notifies when the specified Configuration is updated
    /// </summary>
    public interface IConfigurationFileWatcher<T> : IDisposable
    {
        /// <summary>
        /// Raised when the watched file changes and is successfully re-read, supplying the freshly deserialized configuration of type <typeparamref name="T"/>.
        /// </summary>
        event EventHandler<T> ConfigurationUpdated;

        /// <summary>
        /// Raised when a change is detected but the file cannot be read or deserialized; the event argument carries the error message.
        /// </summary>
        event EventHandler<string> ErrorReceived;
    }
}
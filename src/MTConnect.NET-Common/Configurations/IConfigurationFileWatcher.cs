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
        EventHandler<T> ConfigurationUpdated { get; set; }

        EventHandler<string> ErrorReceived { get; set; }
    }
}
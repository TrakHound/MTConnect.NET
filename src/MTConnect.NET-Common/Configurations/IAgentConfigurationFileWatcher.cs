// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Agent Configuration File Watcher that notifies when the specified Configuration is updated
    /// </summary>
    public interface IAgentConfigurationFileWatcher : IDisposable
    {
        EventHandler<AgentConfiguration> ConfigurationUpdated { get; set; }

        EventHandler<string> ErrorReceived { get; set; }
    }
}
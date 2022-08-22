// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Agent Configuration File Watcher that notifies when the specified Configuration is updated
    /// </summary>
    public interface IAgentConfigurationFileWatcher : IDisposable
    {
        public EventHandler<AgentConfiguration> ConfigurationUpdated { get; set; }

        public EventHandler<string> ErrorReceived { get; set; }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Agent Configuration File Watcher that notifies when the specified Configuration is updated
    /// </summary>
    public interface IAgentConfigurationFileWatcher : IConfigurationFileWatcher<AgentConfiguration> { }
}
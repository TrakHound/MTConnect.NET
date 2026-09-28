// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration shape for the Web UI agent module. Controls the
    /// address the UI listens on and whether the Agent configuration
    /// file may be edited from the browser.
    /// </summary>
    public class WebUiModuleConfiguration
    {
        /// <summary>
        /// The hostname the UI binds to. Defaults to <c>localhost</c> so the
        /// UI (and its configuration editor) is only reachable from the
        /// machine the Agent runs on. Use <c>*</c> or <c>+</c> to listen on
        /// every interface (may require elevated permissions on Windows).
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The port the UI listens on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or Sets whether the Agent configuration file can be edited
        /// and saved from the UI. When <c>false</c> the configuration is
        /// shown read-only.
        /// </summary>
        public bool AllowConfigurationEdit { get; set; }


        /// <summary>
        /// Initialises a new instance bound to <c>localhost:5080</c> with
        /// configuration editing enabled.
        /// </summary>
        public WebUiModuleConfiguration()
        {
            Server = "localhost";
            Port = 5080;
            AllowConfigurationEdit = true;
        }
    }
}

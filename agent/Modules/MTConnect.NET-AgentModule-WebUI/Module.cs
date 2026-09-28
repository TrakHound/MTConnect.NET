// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Logging;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// Agent module that hosts a browser user interface for the Agent.
    /// The UI lets a user browse Devices, current / sampled Observations
    /// and Assets, and view or edit the Agent configuration file.
    /// </summary>
    public class Module : MTConnectAgentModule
    {
        /// <summary>
        /// Token used in <c>agent.config.yaml</c> to bind this module
        /// (<c>- web-ui:</c>).
        /// </summary>
        public const string ConfigurationTypeId = "web-ui";
        private const string ModuleId = "Web UI";

        private readonly WebUiModuleConfiguration _configuration;
        private WebUiServer _server;


        /// <summary>
        /// Initialises the module and binds the supplied controller-
        /// configuration payload to <see cref="WebUiModuleConfiguration"/>.
        /// </summary>
        /// <param name="mtconnectAgent">Agent broker the UI reads from.</param>
        /// <param name="controllerConfiguration">Raw configuration payload
        /// bound to <see cref="WebUiModuleConfiguration"/>.</param>
        public Module(IMTConnectAgentBroker mtconnectAgent, object controllerConfiguration) : base(mtconnectAgent)
        {
            Id = ModuleId;

            _configuration = AgentApplicationConfiguration.GetConfiguration<WebUiModuleConfiguration>(controllerConfiguration);
            if (_configuration == null) _configuration = new WebUiModuleConfiguration();
        }


        /// <summary>
        /// Module lifecycle hook: creates the UI web server before the Agent
        /// loads its devices so the per-Device Observation counts include
        /// the Observations added while the devices initialize.
        /// </summary>
        /// <param name="initializeDataItems">Inherited flag; unused by
        /// this module.</param>
        protected override void OnStartBeforeLoad(bool initializeDataItems)
        {
            _server = new WebUiServer(_configuration, Agent);
        }

        /// <summary>
        /// Module lifecycle hook: starts the UI web server once the Agent
        /// has loaded its devices.
        /// </summary>
        /// <param name="initializeDataItems">Inherited flag; unused by
        /// this module.</param>
        protected override void OnStartAfterLoad(bool initializeDataItems)
        {
            if (_server == null) _server = new WebUiServer(_configuration, Agent);
            _server.LogReceived += (s, message) => Log(MTConnectLogLevel.Information, message);
            _server.ErrorReceived += (s, message) => Log(MTConnectLogLevel.Warning, message);
            _server.Start();
        }

        /// <summary>
        /// Module lifecycle hook: stops the UI web server.
        /// </summary>
        protected override void OnStop()
        {
            if (_server != null) _server.Stop();
        }
    }
}

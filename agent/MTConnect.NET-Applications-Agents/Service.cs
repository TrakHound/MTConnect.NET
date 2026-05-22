// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Services;
using NLog;
using System.Runtime.Versioning;

namespace MTConnect.Applications
{
    /// <summary>
    /// Class used to implement a Windows Service for an MTConnect Agent Application
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class Service : MTConnectAgentService
    {
        private static readonly Logger _serviceLogger = LogManager.GetLogger("service-logger");
        private readonly IMTConnectAgentApplication _application;


        /// <summary>
        /// Initialises a new Windows-service wrapper that delegates
        /// every <c>Start</c> / <c>Stop</c> action to the supplied
        /// agent application.
        /// </summary>
        /// <param name="application">Agent application the service
        /// hosts.</param>
        /// <param name="name">Windows-service identifier.</param>
        /// <param name="displayName">Human-readable display name.</param>
        /// <param name="description">Service description.</param>
        /// <param name="autoStart">Whether the service should be set
        /// to start automatically on boot.</param>
        public Service(IMTConnectAgentApplication application, string name = null, string displayName = null, string description = null, bool autoStart = true)
            : base(name, displayName, description, autoStart)
        {
            _application = application;
        }


        /// <summary>
        /// Service-control hook: delegates to
        /// <see cref="IMTConnectAgentApplication.StartAgent"/> with the
        /// supplied configuration path.
        /// </summary>
        /// <param name="configurationPath">Path to the agent's
        /// configuration file.</param>
        protected override void StartAgent(string configurationPath)
        {
            _application.StartAgent(configurationPath);
        }

        /// <summary>
        /// Service-control hook: delegates to
        /// <see cref="IMTConnectAgentApplication.StopAgent"/>.
        /// </summary>
        protected override void StopAgent()
        {
            _application.StopAgent();
        }


        /// <summary>
        /// Routes informational service messages to the
        /// <c>service-logger</c> NLog target.
        /// </summary>
        /// <param name="message">Message to log.</param>
        protected override void OnLogInformation(string message)
        {
            _serviceLogger.Info(message);
        }

        /// <summary>
        /// Routes warning service messages to the
        /// <c>service-logger</c> NLog target.
        /// </summary>
        /// <param name="message">Message to log.</param>
        protected override void OnLogWarning(string message)
        {
            _serviceLogger.Warn(message);
        }

        /// <summary>
        /// Routes error service messages to the <c>service-logger</c>
        /// NLog target.
        /// </summary>
        /// <param name="message">Message to log.</param>
        protected override void OnLogError(string message)
        {
            _serviceLogger.Error(message);
        }
    }
}
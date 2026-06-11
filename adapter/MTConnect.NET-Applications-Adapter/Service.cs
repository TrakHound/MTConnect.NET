// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Services;
using NLog;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

namespace MTConnect.Applications
{
    /// <summary>
    /// Class used to implement a Windows Service for an MTConnect Agent Application
    /// </summary>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public class Service : MTConnectAdapterService
    {
        private static readonly Logger _serviceLogger = LogManager.GetLogger("service-logger");
        private readonly IMTConnectAdapterApplication _application;


        /// <summary>
        /// Initialises a new Windows-service wrapper that delegates
        /// every <c>Start</c> / <c>Stop</c> action to the supplied
        /// adapter application.
        /// </summary>
        /// <param name="application">Adapter application the service
        /// hosts.</param>
        /// <param name="label">Human-readable label.</param>
        /// <param name="name">Windows-service identifier.</param>
        /// <param name="displayName">Service display name.</param>
        /// <param name="description">Service description.</param>
        /// <param name="autoStart">Whether the service should be set
        /// to start automatically on boot.</param>
        public Service(IMTConnectAdapterApplication application, string label = null, string name = null, string displayName = null, string description = null, bool autoStart = true)
            : base(label, name, displayName, description, autoStart)
        {
            _application = application;
        }


        /// <summary>
        /// Service-control hook: delegates to
        /// <see cref="IMTConnectAdapterApplication.StartAdapter"/>.
        /// </summary>
        /// <param name="configurationPath">Path to the adapter's
        /// configuration file.</param>
        protected override void StartAdapter(string configurationPath)
        {
            _application.StartAdapter(configurationPath);
        }

        /// <summary>
        /// Service-control hook: delegates to
        /// <see cref="IMTConnectAdapterApplication.StopAdapter"/>.
        /// </summary>
        protected override void StopAdapter()
        {
            _application.StopAdapter();
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
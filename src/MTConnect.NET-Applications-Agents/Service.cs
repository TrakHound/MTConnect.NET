// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Services;
using NLog;

namespace MTConnect.Applications
{
    /// <summary>
    /// Class used to implement a Windows Service for an MTConnect Agent Application
    /// </summary>
    public class Service : MTConnectAgentService
    {
        private static readonly Logger _serviceLogger = LogManager.GetLogger("service-logger");
        private readonly IMTConnectAgentApplication _application;


        public Service(IMTConnectAgentApplication application, string name = null, string displayName = null, string description = null, bool autoStart = true)
            : base(name, displayName, description, autoStart) 
        {
            _application = application;
        }


        protected override void StartAgent(string configurationPath)
        {
            _application.StartAgent(configurationPath);
        }

        protected override void StopAgent()
        {
            _application.StopAgent();
        }


        protected override void OnLogInformation(string message)
        {
            _serviceLogger.Info(message);
        }

        protected override void OnLogWarning(string message)
        {
            _serviceLogger.Warn(message);
        }

        protected override void OnLogError(string message)
        {
            _serviceLogger.Error(message);
        }
    }
}
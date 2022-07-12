// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Applications.Agents;
using MTConnect.Services;
using NLog;

namespace MTConnect.Applications
{
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

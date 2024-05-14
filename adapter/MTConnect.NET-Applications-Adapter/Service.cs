// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Services;
using NLog;

namespace MTConnect.Applications
{
    /// <summary>
    /// Class used to implement a Windows Service for an MTConnect Agent Application
    /// </summary>
    public class Service : MTConnectAdapterService
    {
        private static readonly Logger _serviceLogger = LogManager.GetLogger("service-logger");
        private readonly IMTConnectAdapterApplication _application;


        public Service(IMTConnectAdapterApplication application, string label = null, string name = null, string displayName = null, string description = null, bool autoStart = true)
            : base(label, name, displayName, description, autoStart) 
        {
            _application = application;
        }


        protected override void StartAdapter(string configurationPath)
        {
            _application.StartAdapter(configurationPath);
        }

        protected override void StopAdapter()
        {
            _application.StopAdapter();
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
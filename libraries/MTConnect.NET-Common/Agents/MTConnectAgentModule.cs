// Copyright(c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;

namespace MTConnect.Agents
{
    public abstract class MTConnectAgentModule : IMTConnectAgentModule
    {
        private readonly IMTConnectAgentBroker _agent;


        public string Id { get; set; }

        public string Description { get; set; }

        public IMTConnectAgentBroker Agent => _agent;


        public event MTConnectLogEventHandler LogReceived;


        public MTConnectAgentModule(IMTConnectAgentBroker agent)
        {
            _agent = agent;
        }


        public void StartBeforeLoad(bool initializeDataItems)
        {
            OnStartBeforeLoad(initializeDataItems);
        }

        public void StartAfterLoad(bool initializeDataItems)
        {
            OnStartAfterLoad(initializeDataItems);
        }

        public void Stop()
        {
            OnStop();
        }


        protected virtual void OnStartBeforeLoad(bool initializeDataItems) { }

        protected virtual void OnStartAfterLoad(bool initializeDataItems) { }

        protected virtual void OnStop() { }


        protected void Log(MTConnectLogLevel logLevel, string message, string logId = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message, logId);
        }
    }
}

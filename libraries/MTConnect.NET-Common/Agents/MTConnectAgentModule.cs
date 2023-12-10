// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
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


        public void StartBeforeLoad()
        {
            OnStartBeforeLoad();
        }

        public void StartAfterLoad()
        {
            OnStartAfterLoad();
        }

        public void Stop()
        {
            OnStop();
        }


        protected virtual void OnStartBeforeLoad() { }

        protected virtual void OnStartAfterLoad() { }

        protected virtual void OnStop() { }


        protected void Log(MTConnectLogLevel logLevel, string message, string logId = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message, logId);
        }
    }
}

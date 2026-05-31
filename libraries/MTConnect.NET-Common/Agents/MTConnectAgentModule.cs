// Copyright(c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;

namespace MTConnect.Agents
{
    /// <summary>
    /// Base class for <see cref="IMTConnectAgentModule"/> implementations that provides access to the host Agent broker and turns the lifecycle calls into overridable hook methods.
    /// </summary>
    public abstract class MTConnectAgentModule : IMTConnectAgentModule
    {
        private readonly IMTConnectAgentBroker _agent;


        /// <summary>
        /// A unique identifier that distinguishes this module from other modules loaded by the Agent.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A human-readable description of the purpose this module serves within the Agent.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Agent broker that hosts this module, available to derived classes for querying and publishing data.
        /// </summary>
        public IMTConnectAgentBroker Agent => _agent;


        /// <summary>
        /// Raised when the module emits a log entry, allowing the host Agent to surface module diagnostics.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes a new instance bound to the given host Agent broker.
        /// </summary>
        /// <param name="agent">The Agent broker that hosts this module.</param>
        public MTConnectAgentModule(IMTConnectAgentBroker agent)
        {
            _agent = agent;
        }


        /// <summary>
        /// Invoked during Agent startup before Devices are loaded; dispatches to <see cref="OnStartBeforeLoad(bool)"/>.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        public void StartBeforeLoad(bool initializeDataItems)
        {
            OnStartBeforeLoad(initializeDataItems);
        }

        /// <summary>
        /// Invoked during Agent startup after Devices are loaded; dispatches to <see cref="OnStartAfterLoad(bool)"/>.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        public void StartAfterLoad(bool initializeDataItems)
        {
            OnStartAfterLoad(initializeDataItems);
        }

        /// <summary>
        /// Invoked during Agent shutdown; dispatches to <see cref="OnStop"/>.
        /// </summary>
        public void Stop()
        {
            OnStop();
        }


        /// <summary>
        /// Override to perform work before Devices are loaded into the Agent. The default implementation does nothing.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        protected virtual void OnStartBeforeLoad(bool initializeDataItems) { }

        /// <summary>
        /// Override to perform work after Devices are loaded into the Agent. The default implementation does nothing.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        protected virtual void OnStartAfterLoad(bool initializeDataItems) { }

        /// <summary>
        /// Override to release resources during Agent shutdown. The default implementation does nothing.
        /// </summary>
        protected virtual void OnStop() { }


        /// <summary>
        /// Raise <see cref="LogReceived"/> with the given log entry.
        /// </summary>
        /// <param name="logLevel">The severity of the log entry.</param>
        /// <param name="message">The log message.</param>
        /// <param name="logId">An optional identifier used to correlate related log entries.</param>
        protected void Log(MTConnectLogLevel logLevel, string message, string logId = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message, logId);
        }
    }
}

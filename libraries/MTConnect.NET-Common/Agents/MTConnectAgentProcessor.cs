// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Input;
using MTConnect.Logging;

namespace MTConnect.Agents
{
    /// <summary>
    /// Base class for <see cref="IMTConnectAgentProcessor"/> implementations that turns the processing calls into overridable hook methods, defaulting to a pass-through transformation.
    /// </summary>
    public abstract class MTConnectAgentProcessor : IMTConnectAgentProcessor
    {
        /// <summary>
        /// A unique identifier that distinguishes this processor from other processors loaded by the Agent.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A human-readable description of the transformation this processor applies.
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Raised when the processor emits a log entry, allowing the host Agent to surface processor diagnostics.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Override to perform initialization required before processing begins. The default implementation does nothing.
        /// </summary>
        public virtual void Load() { }


        /// <summary>
        /// Process an incoming observation by dispatching to <see cref="OnProcess(ProcessObservation)"/>.
        /// </summary>
        /// <param name="observation">The observation, together with its resolved Device and DataItem context, to process.</param>
        /// <returns>The observation input to store, or <c>null</c> to suppress the observation.</returns>
        public IObservationInput Process(ProcessObservation observation) => OnProcess(observation);

        /// <summary>
        /// Process an incoming Asset by dispatching to <see cref="OnProcess(IAsset)"/>.
        /// </summary>
        /// <param name="asset">The Asset to process.</param>
        /// <returns>The Asset to store, or <c>null</c> to suppress the Asset.</returns>
        public IAsset Process(IAsset asset) => OnProcess(asset);


        /// <summary>
        /// Override to transform an incoming observation. The default implementation copies the observation through unchanged.
        /// </summary>
        /// <param name="observation">The observation, together with its resolved Device and DataItem context, to process.</param>
        /// <returns>The observation input to store, or <c>null</c> when the input observation is <c>null</c>.</returns>
        protected virtual IObservationInput OnProcess(ProcessObservation observation)
        {
            if (observation != null)
            {
                var observationInput = new ObservationInput();
                observationInput.DeviceKey = observation.DataItem?.Device?.Uuid;
                observationInput.DataItemKey = observation.DataItem?.Id;
                observationInput.Timestamp = observation.Timestamp.ToUnixTime();
                observationInput.Values = observation.Values;
                return observationInput;
            }

            return null;
        }

        /// <summary>
        /// Override to transform an incoming Asset. The default implementation returns the Asset unchanged.
        /// </summary>
        /// <param name="asset">The Asset to process.</param>
        /// <returns>The Asset to store.</returns>
        protected virtual IAsset OnProcess(IAsset asset) => asset;


        /// <summary>
        /// Raise <see cref="LogReceived"/> with the given log entry.
        /// </summary>
        /// <param name="logLevel">The severity of the log entry.</param>
        /// <param name="message">The log message.</param>
        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}

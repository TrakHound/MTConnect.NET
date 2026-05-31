// Copyright(c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Logging;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    /// <summary>
    /// Base class for a pluggable adapter module that feeds observations, assets, and devices into an <see cref="IMTConnectAdapter"/> from a source-specific protocol; subclasses implement the connection lifecycle and ingestion.
    /// </summary>
    public abstract class MTConnectAdapterModule : IMTConnectAdapterModule
    {
        /// <summary>
        /// The unique identifier of this module instance.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A human-readable description of what this module does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The adapter this module feeds data into.
        /// </summary>
        public IMTConnectAdapter Adapter { get; set; }


        /// <summary>
        /// Raised when the module emits a log entry.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes the module with its unique identifier.
        /// </summary>
        /// <param name="id">The module identifier.</param>
        public MTConnectAdapterModule(string id)
        {
            Id = id;
        }


        /// <summary>
        /// Starts the module by invoking the overridable <see cref="OnStart"/>.
        /// </summary>
        public void Start()
        {
            OnStart();
        }

        /// <summary>
        /// Stops the module by invoking the overridable <see cref="OnStop"/>.
        /// </summary>
        public void Stop()
        {
            OnStop();
        }


        /// <summary>
        /// Lifecycle hook invoked by <see cref="Start"/>; the base implementation does nothing.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Lifecycle hook invoked by <see cref="Stop"/>; the base implementation does nothing.
        /// </summary>
        protected virtual void OnStop() { }


        /// <summary>
        /// Handles observations passing through the module; the base implementation accepts them unconditionally. Overrides may transform or filter.
        /// </summary>
        /// <param name="observations">The observations to process.</param>
        public virtual bool AddObservations(IEnumerable<IObservationInput> observations)
        {
            return true;
        }

        /// <summary>
        /// Handles assets passing through the module; the base implementation accepts them unconditionally.
        /// </summary>
        /// <param name="assets">The assets to process.</param>
        public virtual bool AddAssets(IEnumerable<IAssetInput> assets)
        {
            return true;
        }

        /// <summary>
        /// Handles devices passing through the module; the base implementation accepts them unconditionally.
        /// </summary>
        /// <param name="devices">The devices to process.</param>
        public virtual bool AddDevices(IEnumerable<IDeviceInput> devices)
        {
            return true;
        }


        /// <summary>
        /// Raises <see cref="LogReceived"/> with the given severity and message.
        /// </summary>
        /// <param name="logLevel">The severity of the entry.</param>
        /// <param name="message">The log message.</param>
        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}

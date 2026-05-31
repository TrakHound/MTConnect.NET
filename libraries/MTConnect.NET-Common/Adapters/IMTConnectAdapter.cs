// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An MTConnect adapter: collects observations, assets, and devices from
    /// a data source and streams them to one or more MTConnect Agents.
    /// </summary>
    public interface IMTConnectAdapter
    {
        /// <summary>
        /// Get a unique identifier for the Adapter
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The Name or UUID of the Device to create a connection for
        /// </summary>
        string DeviceKey { get; }

        /// <summary>
        /// Determines whether to filter out duplicate data
        /// </summary>
        bool FilterDuplicates { get; set; }

        /// <summary>
        /// Determines whether to output Timestamps for each SHDR line
        /// </summary>
        bool OutputTimestamps { get; set; }


        /// <summary>
        /// Raised when new data is sent to the Agent. Includes the AgentClient ID and the Data sent as an argument.
        /// </summary>
        event EventHandler<AdapterEventArgs<string>> DataSent;

        /// <summary>
        /// Raised when an error occurs when sending a new line to the Agent. Includes the AgentClient ID and the Error message as an argument.
        /// </summary>
        event EventHandler<AdapterEventArgs<string>> SendError;


        /// <summary>
        /// Starts the Adapter to begins listening for Agent connections as well as starts the Queue for collecting and sending data to the Agent(s).
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the adapter which also stops listening for Agent connections, disconnects any existing Agent connections, and stops the Queue for sending data to the Agent(s).
        /// </summary>
        void Stop();

        /// <summary>
        /// Sends all Items that have changed since last sent to the Agent
        /// </summary>
        bool SendChanged();

        /// <summary>
        /// Sends all of the last sent Items, Assets, and Devices to the Agent. This can be used upon reconnection to the Agent
        /// </summary>
        bool SendLast(long timestamp = 0);

        /// <summary>
        /// Flushes the entire buffered set of items, assets, and devices to
        /// the Agent regardless of change state. Returns <c>true</c> when the
        /// buffer was sent.
        /// </summary>
        bool SendBuffer();


        /// <summary>
        /// Set all items to Unavailable
        /// </summary>
        void SetUnavailable(long timestamp = 0);


        /// <summary>
        /// Queues an observation for the given DataItem with the supplied
        /// value, timestamped at the current time.
        /// </summary>
        /// <param name="dataItemId">The DataItem identifier.</param>
        /// <param name="value">The observed value.</param>
        void AddObservation(string dataItemId, object value);

        /// <summary>
        /// Queues an observation for the given DataItem with the supplied
        /// value and explicit timestamp.
        /// </summary>
        /// <param name="dataItemId">The DataItem identifier.</param>
        /// <param name="value">The observed value.</param>
        /// <param name="timestamp">The observation timestamp.</param>
        void AddObservation(string dataItemId, object value, DateTime timestamp);

        /// <summary>
        /// Queues an observation for the given DataItem with the supplied
        /// value and a Unix-epoch timestamp.
        /// </summary>
        /// <param name="dataItemId">The DataItem identifier.</param>
        /// <param name="value">The observed value.</param>
        /// <param name="timestamp">The observation timestamp as Unix nanoseconds.</param>
        void AddObservation(string dataItemId, object value, long timestamp);

        /// <summary>
        /// Queues a fully formed observation input.
        /// </summary>
        /// <param name="observation">The observation to queue.</param>
        void AddObservation(IObservationInput observation);


        /// <summary>
        /// Queues and immediately sends an observation for the given DataItem,
        /// timestamped at the current time. Returns <c>true</c> when sent.
        /// </summary>
        /// <param name="dataItemId">The DataItem identifier.</param>
        /// <param name="value">The observed value.</param>
        bool SendObservation(string dataItemId, object value);

        /// <summary>
        /// Queues and immediately sends an observation for the given DataItem
        /// with an explicit timestamp. Returns <c>true</c> when sent.
        /// </summary>
        /// <param name="dataItemId">The DataItem identifier.</param>
        /// <param name="value">The observed value.</param>
        /// <param name="timestamp">The observation timestamp.</param>
        bool SendObservation(string dataItemId, object value, DateTime timestamp);

        /// <summary>
        /// Queues and immediately sends an observation for the given DataItem
        /// with a Unix-epoch timestamp. Returns <c>true</c> when sent.
        /// </summary>
        /// <param name="dataItemId">The DataItem identifier.</param>
        /// <param name="value">The observed value.</param>
        /// <param name="timestamp">The observation timestamp as Unix nanoseconds.</param>
        bool SendObservation(string dataItemId, object value, long timestamp);

        /// <summary>
        /// Queues and immediately sends a fully formed observation input.
        /// Returns <c>true</c> when sent.
        /// </summary>
        /// <param name="observation">The observation to send.</param>
        bool SendObservation(IObservationInput observation);


        /// <summary>
        /// Add the specified MTConnect Asset
        /// </summary>
        /// <param name="asset">The Asset to add</param>
        void AddAsset(IAssetInput asset);

        /// <summary>
        /// Remove the specified Asset using the SHDR command @REMOVE_ASSET@
        /// </summary>
        /// <param name="assetId">The AssetId of the Asset to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        void RemoveAsset(string assetId, long timestamp = 0);

        /// <summary>
        /// Remove all Assets of the specified Type using the SHDR command @REMOVE_ALL_ASSETS@
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The timestamp to send as part of the SHDR command</param>
        void RemoveAllAssets(string assetType, long timestamp = 0);


        /// <summary>
        /// Add the specified MTConnect Device
        /// </summary>
        /// <param name="device">The Device to add</param>
        void AddDevice(IDeviceInput device);
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
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
        public event EventHandler<AdapterEventArgs> DataSent;

        /// <summary>
        /// Raised when an error occurs when sending a new line to the Agent. Includes the AgentClient ID and the Error message as an argument.
        /// </summary>
        public event EventHandler<AdapterEventArgs> SendError;


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
        /// Set all items to Unavailable
        /// </summary>
        void SetUnavailable(long timestamp = 0);


        void AddObservation(string dataItemId, object value);

        void AddObservation(string dataItemId, object value, DateTime timestamp);

        void AddObservation(string dataItemId, object value, long timestamp);

        void AddObservation(IObservationInput observation);


        bool SendObservation(string dataItemId, object value);

        bool SendObservation(string dataItemId, object value, DateTime timestamp);

        bool SendObservation(string dataItemId, object value, long timestamp);

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
    }
}

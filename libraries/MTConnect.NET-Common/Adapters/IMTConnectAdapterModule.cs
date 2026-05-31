// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Logging;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    /// <summary>
    /// A pluggable adapter module: a data-source-specific component that feeds
    /// observations, assets, and devices into an <see cref="IMTConnectAdapter"/>.
    /// </summary>
    public interface IMTConnectAdapterModule
    {
        /// <summary>
        /// The unique identifier of the module.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A human-readable description of the module.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The adapter the module feeds. Set by the host before
        /// <see cref="Start"/>.
        /// </summary>
        IMTConnectAdapter Adapter { get; set; }


        /// <summary>
        /// Raised when the module emits a log entry.
        /// </summary>
        event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Starts the module so it begins producing data for the adapter.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the module and releases any data-source resources.
        /// </summary>
        void Stop();


        /// <summary>
        /// Pushes a batch of observations to the adapter. Returns <c>true</c>
        /// when accepted.
        /// </summary>
        /// <param name="observations">The observations to add.</param>
        bool AddObservations(IEnumerable<IObservationInput> observations);

        /// <summary>
        /// Pushes a batch of assets to the adapter. Returns <c>true</c> when
        /// accepted.
        /// </summary>
        /// <param name="assets">The assets to add.</param>
        bool AddAssets(IEnumerable<IAssetInput> assets);

        /// <summary>
        /// Pushes a batch of devices to the adapter. Returns <c>true</c> when
        /// accepted.
        /// </summary>
        /// <param name="devices">The devices to add.</param>
        bool AddDevices(IEnumerable<IDeviceInput> devices);
    }
}

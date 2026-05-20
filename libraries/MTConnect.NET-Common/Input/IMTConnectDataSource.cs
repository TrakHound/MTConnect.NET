// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using System;

namespace MTConnect.Input
{
    /// <summary>
    /// Engine used to handle a worker thread running at an Interval used to organize reading from a Data Source (ex. PLC) and writing to an Adapter
    /// </summary>
    public interface IMTConnectDataSource
    {
        /// <summary>
        /// The configuration that controls the polling interval and identifies the data source.
        /// </summary>
        IDataSourceConfiguration Configuration { get; set; }


        /// <summary>
        /// Raised when the data source produces a new Observation to be written to the Adapter.
        /// </summary>
        event EventHandler<IObservationInput> ObservationAdded;

        /// <summary>
        /// Raised when the data source produces a new Asset to be written to the Adapter.
        /// </summary>
        event EventHandler<IAssetInput> AssetAdded;

        /// <summary>
        /// Raised when the data source produces a new Device to be written to the Adapter.
        /// </summary>
        event EventHandler<IDeviceInput> DeviceAdded;


        /// <summary>
        /// Starts the worker thread that polls the data source at the configured interval.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the worker thread and ends polling of the data source.
        /// </summary>
        void Stop();


        /// <summary>
        /// Queues an Observation for the specified DataItem using the current time as the timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="resultValue">The Result value to report.</param>
        void AddObservation(string dataItemKey, object resultValue);

        /// <summary>
        /// Queues an Observation for the specified DataItem at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="resultValue">The Result value to report.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds, or <c>null</c> to use the current time.</param>
        void AddObservation(string dataItemKey, object resultValue, long? timestamp);

        /// <summary>
        /// Queues an Observation for the specified DataItem at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="resultValue">The Result value to report.</param>
        /// <param name="timestamp">The observation timestamp, or <c>null</c> to use the current time.</param>
        void AddObservation(string dataItemKey, object resultValue, DateTime? timestamp);

        /// <summary>
        /// Queues a fully constructed Observation.
        /// </summary>
        /// <param name="observation">The Observation to report.</param>
        void AddObservation(IObservationInput observation);


        /// <summary>
        /// Queues an Asset for reporting to the Adapter.
        /// </summary>
        /// <param name="asset">The Asset to report.</param>
        void AddAsset(IAsset asset);

        /// <summary>
        /// Queues a fully constructed Asset Input for reporting to the Adapter.
        /// </summary>
        /// <param name="assetInput">The Asset Input to report.</param>
        void AddAsset(IAssetInput assetInput);


        /// <summary>
        /// Queues a Device for reporting to the Adapter.
        /// </summary>
        /// <param name="device">The Device to report.</param>
        void AddDevice(IDevice device);

        /// <summary>
        /// Queues a fully constructed Device Input for reporting to the Adapter.
        /// </summary>
        /// <param name="deviceInput">The Device Input to report.</param>
        void AddDevice(IDeviceInput deviceInput);
    }
}

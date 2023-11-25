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
        IAdapterApplicationConfiguration Configuration { get; set; }


        event EventHandler<IObservationInput> ObservationAdded;

        event EventHandler<IAssetInput> AssetAdded;

        event EventHandler<IDevice> DeviceAdded;


        void Start();

        void Stop();


        void AddObservation(string dataItemKey, object resultValue);

        void AddObservation(string dataItemKey, object resultValue, long? timestamp);

        void AddObservation(string dataItemKey, object resultValue, DateTime? timestamp);

        void AddObservation(IObservationInput observation);


        void AddAsset(IAsset asset);


        void AddDevice(IDevice device);
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Logging;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    public interface IMTConnectAdapterModule
    {
        string Id { get; }

        string Description { get; }

        IMTConnectAdapter Adapter { get; set; }


        event MTConnectLogEventHandler LogReceived;


        void Start();

        void Stop();


        bool AddObservations(IEnumerable<IObservationInput> observations);

        bool AddAssets(IEnumerable<IAssetInput> assets);

        bool AddDevices(IEnumerable<IDevice> devices);
    }
}

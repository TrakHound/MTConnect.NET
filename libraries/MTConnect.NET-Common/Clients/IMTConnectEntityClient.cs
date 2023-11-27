// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;

namespace MTConnect.Clients
{
    public interface IMTConnectEntityClient
    {
        event EventHandler<IDevice> DeviceReceived;

        event EventHandler<IAsset> AssetReceived;

        event EventHandler<IObservation> ObservationReceived;
    }
}

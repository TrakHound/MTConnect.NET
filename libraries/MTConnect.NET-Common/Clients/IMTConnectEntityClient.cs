// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;

namespace MTConnect.Clients
{
    /// <summary>
    /// Abstraction for a client that streams MTConnect entities from an Agent, surfacing each decoded device, asset, and observation through events as it arrives.
    /// </summary>
    public interface IMTConnectEntityClient
    {
        /// <summary>
        /// Raised when a Device entity is received from the Agent.
        /// </summary>
        event EventHandler<IDevice> DeviceReceived;

        /// <summary>
        /// Raised when an Asset entity is received from the Agent.
        /// </summary>
        event EventHandler<IAsset> AssetReceived;

        /// <summary>
        /// Raised when an Observation is received from the Agent.
        /// </summary>
        event EventHandler<IObservation> ObservationReceived;
    }
}

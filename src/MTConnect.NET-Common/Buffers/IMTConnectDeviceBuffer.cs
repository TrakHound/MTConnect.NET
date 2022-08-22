// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System.Collections.Generic;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer interface used to store information describing both the physical and logical structure of the piece of equipment
    /// and a detailed description of each Data Entity that can be reported by the Agent associated with the piece of equipment.
    /// </summary>
    public interface IMTConnectDeviceBuffer
    {
        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        string Id { get; }


        /// <summary>
        /// Get all MTConnectDevices from the Buffer
        /// </summary>
        IEnumerable<IDevice> GetDevices(string type = null);

        /// <summary>
        /// Get the specified MTConnectDevice from the Buffer
        /// </summary>
        IDevice GetDevice(string deviceUuid);


        /// <summary>
        /// Add a new MTConnectDevice to the Buffer
        /// </summary>
        bool AddDevice(IDevice device);

        /// <summary>
        /// Add new MTConnectDevices to the Buffer
        /// </summary>
        bool AddDevices(IEnumerable<IDevice> devices);
    }
}

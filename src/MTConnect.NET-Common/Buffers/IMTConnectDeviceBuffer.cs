// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        IEnumerable<IDevice> GetDevices();

        /// <summary>
        /// Get all MTConnectDevices from the Buffer
        /// </summary>
        Task<IEnumerable<IDevice>> GetDevicesAsync();

        /// <summary>
        /// Get the specified MTConnectDevice from the Buffer
        /// </summary>
        IDevice GetDevice(string deviceUuid);

        /// <summary>
        /// Get the specified MTConnectDevice from the Buffer
        /// </summary>
        Task<IDevice> GetDeviceAsync(string deviceUuid);


        /// <summary>
        /// Add a new MTConnectDevice to the Buffer
        /// </summary>
        bool AddDevice(IDevice device);

        /// <summary>
        /// Add a new MTConnectDevice to the Buffer
        /// </summary>
        Task<bool> AddDeviceAsync(IDevice device);

        /// <summary>
        /// Add new MTConnectDevices to the Buffer
        /// </summary>
        bool AddDevices(IEnumerable<IDevice> devices);

        /// <summary>
        /// Add new MTConnectDevices to the Buffer
        /// </summary>
        Task<bool> AddDevicesAsync(IEnumerable<IDevice> devices);
    }
}

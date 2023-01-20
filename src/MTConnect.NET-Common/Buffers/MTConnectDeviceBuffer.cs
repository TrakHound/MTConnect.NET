// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer interface used to store information describing both the physical and logical structure of the piece of equipment
    /// and a detailed description of each Data Entity that can be reported by the Agent associated with the piece of equipment.
    /// </summary>
    public class MTConnectDeviceBuffer : IMTConnectDeviceBuffer
    {
        private readonly ConcurrentDictionary<string, IDevice> _storedDevices = new ConcurrentDictionary<string, IDevice>(); // Device.Uuid => IDevice
        private readonly string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        public string Id => _id;


        /// <summary>
        /// Get all MTConnectDevices from the Buffer
        /// </summary>
        public IEnumerable<IDevice> GetDevices(string type = null)
        {
            if (!string.IsNullOrEmpty(type)) return _storedDevices.Select(o => o.Value).Where(o => !string.IsNullOrEmpty(o.Type) && o.Type.ToLower() == type.ToLower());
            else return _storedDevices.Select(o => o.Value);
        }

        /// <summary>
        /// Get the specified MTConnectDevice from the Buffer
        /// </summary>
        public IDevice GetDevice(string deviceUuid)
        {
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                _storedDevices.TryGetValue(deviceUuid, out IDevice device);
                return device;
            }
            return null;
        }


        /// <summary>
        /// Add a new MTConnectDevice to the Buffer
        /// </summary>
        public bool AddDevice(IDevice device)
        {
            if (device != null && !string.IsNullOrEmpty(device.Uuid))
            {
                // Get the Existing Device (if exists)
                _storedDevices.TryGetValue(device.Uuid, out IDevice existingDevice);

                // Compare the ChangeId Hash of the Existing Device and the New Device
                if (existingDevice == null || existingDevice.ChangeId != device.ChangeId)
                {
                    // Add Device
                    _storedDevices.TryRemove(device.Uuid, out _);
                    _storedDevices.TryAdd(device.Uuid, device);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Add new MTConnectDevices to the Buffer
        /// </summary>
        public bool AddDevices(IEnumerable<IDevice> devices)
        {
            if (!devices.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var device in devices)
                {
                    success = AddDevice(device);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }


        private IEnumerable<IDataItem> GetDeviceDataItems(IDevice device)
        {
            if (device != null)
            {
                // Create List of DataItems to Add Initially
                var dataItems = new List<IDataItem>();

                // Add Root DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        if (dataItem.Type != "ASSET_CHANGED" && dataItem.Type != "ASSET_REMOVED")
                        {
                            dataItems.Add(dataItem);
                        }
                    }
                }

                // Add Component DataItems
                var components = device.GetComponents();
                if (!components.IsNullOrEmpty())
                {
                    foreach (var component in components)
                    {
                        if (!component.DataItems.IsNullOrEmpty())
                        {
                            dataItems.AddRange(component.DataItems);
                        }

                        // Add Composition DataItems
                        if (!component.Compositions.IsNullOrEmpty())
                        {
                            foreach (var composition in component.Compositions)
                            {
                                if (!composition.DataItems.IsNullOrEmpty())
                                {
                                    dataItems.AddRange(composition.DataItems);
                                }
                            }
                        }
                    }
                }

                // Add Composition DataItems
                if (!device.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in device.Compositions)
                    {
                        if (!composition.DataItems.IsNullOrEmpty())
                        {
                            dataItems.AddRange(composition.DataItems);
                        }
                    }
                }

                return dataItems;
            }

            return null;
        }
    }
}

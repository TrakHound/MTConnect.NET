// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using MTConnect.Interfaces;
using System;
using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// A document that contains information describing both the physical and logical structure of the piece of equipment
    /// and a detailed description of each Data Entity that can be reported by the Agent associated with the piece of equipment.
    /// </summary>
    public class DevicesResponseDocument : IDevicesResponseDocument
    {
        /// <summary>
        /// An XML container in an MTConnect Response Document that provides information from an Agent
        /// defining version information, storage capacity, and parameters associated with the data management within the Agent.
        /// </summary>
        public IMTConnectDevicesHeader Header { get; set; }

        /// <summary>
        /// The first, or highest level, Structural Element in a MTConnectDevices document.Devices is a container type XML element.
        /// </summary>
        public virtual IEnumerable<IDevice> Devices { get; set; }


        public IEnumerable<IInterface> Interfaces { get; set; }

        public Version Version { get; set; }


        public IEnumerable<IComponent> GetComponents()
        {
            var components = new List<IComponent>();

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices)
                {
                    var deviceComponents = device.GetComponents();
                    if (!deviceComponents.IsNullOrEmpty()) components.AddRange(deviceComponents);
                }
            }

            return components;
        }

        public IEnumerable<IDataItem> GetDataItems()
        {
            var dataItems = new List<IDataItem>();

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices)
                {
                    var deviceDataItems = device.GetDataItems();
                    if (!deviceDataItems.IsNullOrEmpty()) dataItems.AddRange(deviceDataItems);
                }
            }

            return dataItems;
        }
    }
}
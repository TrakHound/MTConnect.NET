// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Net;
using System.Net.NetworkInformation;

namespace MTConnect.DeviceFinder
{
    /// <summary>
    /// An MTConnect Device that was found using DeviceFinder
    /// </summary>
    public struct MTConnectDevice
    {
        /// <summary>
        /// The IP Address of the MTConnect Agent
        /// </summary>
        public IPAddress IpAddress { get; set; }

        /// <summary>
        /// The Port of the MTConnect Agent
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The MAC Address of the device that the MTConnect Agent is operating on
        /// </summary>
        public PhysicalAddress MacAddress { get; set; }

        /// <summary>
        /// The UUID of the MTConnect Device
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// The Name of the MTConnect Device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Description of the MTConnect Device
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Manufacturer of the MTConnect Device
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The Model of the MTConnect Device
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The SerialNumber of the MTConnect Device
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The Station of the MTConnect Device
        /// </summary>
        public string Station { get; set; }


        /// <summary>Constructs a discovered device record from the network-layer details and the UUID/name probed from the agent.</summary>
        /// <param name="ipAddress">The IP address that responded to the probe.</param>
        /// <param name="port">The TCP port the MTConnect agent is listening on.</param>
        /// <param name="macAddress">The MAC address resolved for <paramref name="ipAddress"/> on the local segment.</param>
        /// <param name="deviceUuid">The UUID returned in the probe response.</param>
        /// <param name="deviceName">The device <c>name</c> returned in the probe response.</param>
        public MTConnectDevice(
            IPAddress ipAddress,
            int port,
            PhysicalAddress macAddress,
            string deviceUuid,
            string deviceName
            )
        {
            IpAddress = ipAddress;
            Port = port;
            MacAddress = macAddress;
            Uuid = deviceUuid;
            Name = deviceName;
            Description = null;
            Manufacturer = null;
            Model = null;
            SerialNumber = null;
            Station = null;
        }
    }
}
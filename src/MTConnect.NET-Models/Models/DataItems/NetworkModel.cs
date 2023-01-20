// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// Network details of a component.
    /// </summary>
    public class NetworkModel
    {
        /// <summary>
        /// The IPV4 network address of the component.
        /// </summary>
        public string IPv4Address { get; set; }
        public IDataItemModel IPv4AddressDataItem { get; set; }

        /// <summary>
        /// The IPV6 network address of the component.
        /// </summary>
        public string IPv6Address { get; set; }
        public IDataItemModel IPv6AddressDataItem { get; set; }

        /// <summary>
        /// The Gateway for the component network.
        /// </summary>
        public string Gateway { get; set; }
        public IDataItemModel GatewayDataItem { get; set; }

        /// <summary>
        /// The SubNet mask for the component network.
        /// </summary>
        public string SubnetMask { get; set; }
        public IDataItemModel SubnetMaskDataItem { get; set; }

        /// <summary>
        /// The layer2 Virtual Local Network (VLAN) ID for the component network.
        /// </summary>
        public string VLanId { get; set; }
        public IDataItemModel VLanIdDataItem { get; set; }

        /// <summary>
        /// Media Access Control Address. The unique physical address of the network hardware.
        /// </summary>
        public string MacAddress { get; set; }
        public IDataItemModel MacAddressDataItem { get; set; }

        /// <summary>
        /// Identifies whether the connection type is wireless.
        /// </summary>
        public string Wireless { get; set; }
        public IDataItemModel WirelessDataItem { get; set; }
    }
}

// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
    public interface IDevicesResponseDocument
    {
        /// <summary>
        /// An XML container in an MTConnect Response Document that provides information from an Agent
        /// defining version information, storage capacity, and parameters associated with the data management within the Agent.
        /// </summary>
        IMTConnectDevicesHeader Header { get; }

        /// <summary>
        /// The first, or highest level, Structural Element in a MTConnectDevices document.Devices is a container type XML element.
        /// </summary>
        IEnumerable<IDevice> Devices { get; }

        /// <summary>
        /// Interfaces is an XML Structural Element in the MTConnectDevices XML document. Interfaces is a container type XML element. 
        /// Interfaces is used to group information de320 scribing Lower Level Interface XML elements, which each provide information for an individual Interface.
        /// </summary>
        IEnumerable<IInterface> Interfaces { get; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
        Version Version { get; }


        IEnumerable<IComponent> GetComponents();

        IEnumerable<IDataItem> GetDataItems();
    }
}

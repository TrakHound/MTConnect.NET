// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    public interface IDevice : IComponent
    {
        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        string Iso841Class { get; }

        /// <summary>
        /// The MTConnect version of the Devices Information Model used to configure
        /// the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        Version MTConnectVersion { get; }

        /// <summary>
        /// A MD5 Hash of the Device that can be used to compare Device objects
        /// </summary>
        [JsonIgnore]
        string ChangeId { get; }
    }
}

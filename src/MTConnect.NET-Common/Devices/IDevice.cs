// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

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
		/// The Agent InstanceId that produced this Device
		/// </summary>
		long InstanceId { get; }
	}
}
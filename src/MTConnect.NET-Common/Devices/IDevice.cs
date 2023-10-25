// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    public partial interface IDevice : IComponent
    {
		/// <summary>
		/// The Agent InstanceId that produced this Device
		/// </summary>
		long InstanceId { get; }

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        string Iso841Class { get; }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public partial interface IDevice : IComponent, IContainer
    {
		/// <summary>
		/// The Agent InstanceId that produced this Device
		/// </summary>
		ulong InstanceId { get; }

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        string Iso841Class { get; }

        /// <summary>
        /// The Type of Device
        /// </summary>
        string Type { get; }


        string GenerateHash();
    }
}
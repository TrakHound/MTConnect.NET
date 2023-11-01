// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Devices
{
    public partial interface IContainer : IMTConnectEntity
    {
        /// <summary>
        /// A container for the DataItem elements associated with this Component element.
        /// </summary>
        IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// The Container (Component or Device) that this DataItem is directly associated with
        /// </summary>
        IContainer Parent { get; set; }


		/// <summary>
		/// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
		/// </summary>
		string Hash { get; }

        /// <summary>
        /// 
        /// </summary>
        string Type { get; }


        /// <summary>
        /// The text description that describes what the Component Type represents
        /// </summary>
        string TypeDescription { get; }


        /// <summary>
        /// The full path of IDs that describes the location of the Component in the Device
        /// </summary>
        string IdPath { get; }

        /// <summary>
        /// The list of IDs (in order) that describes the location of the Component in the Device
        /// </summary>
        string[] IdPaths { get; }

        /// <summary>
        /// The full path of Types that describes the location of the Component in the Device
        /// </summary>
        string TypePath { get; }

        /// <summary>
        /// The list of Types (in order) that describes the location of the Component in the Device
        /// </summary>
        string[] TypePaths { get; }


        /// <summary>
        /// The maximum MTConnect Version that this Component Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        Version MaximumVersion { get; }

        /// <summary>
        /// The minimum MTConnect Version that this Component Type is valid 
        /// </summary>
        Version MinimumVersion { get; }
    }
}
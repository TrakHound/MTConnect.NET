// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using MTConnect.Input;
using MTConnect.Observations;
using System;

namespace MTConnect.Devices
{
    public partial interface IDataItem : IMTConnectEntity
    {
        /// <summary>
        /// The Device that this DataItem is associated with
        /// </summary>
        IDevice Device { get; }

        /// <summary>
        /// The Container (Component or Device) that this DataItem is directly associated with
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// The Agent InstanceId that produced this Device
        /// </summary>
        ulong InstanceId { get; }


        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        string Hash { get; }


		/// <summary>
		/// The text description that describes what the DataItem Type represents
		/// </summary>
		string TypeDescription { get; }

        /// <summary>
        /// The text description that describes what the DataItem SubType represents
        /// </summary>
        string SubTypeDescription { get; }


        /// <summary>
        /// The full path of IDs that describes the location of the DataItem in the Device
        /// </summary>
        string IdPath { get; }

        /// <summary>
        /// The list of IDs (in order) that describes the location of the DataItem in the Device
        /// </summary>
        string[] IdPaths { get; }

        /// <summary>
        /// The full path of Types that describes the location of the DataItem in the Device
        /// </summary>
        string TypePath { get; }

        /// <summary>
        /// The list of Types (in order) that describes the location of the DataItem in the Device
        /// </summary>
        string[] TypePaths { get; }


        /// <summary>
        /// The maximum MTConnect Version that this DataItem Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        Version MaximumVersion { get; }

        /// <summary>
        /// The minimum MTConnect Version that this DataItem Type is valid 
        /// </summary>
        Version MinimumVersion { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsExtended { get; }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        ValidationResult Validate(Version mtconnectVersion, IObservationInput observation);

        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        ValidationResult Validate(Version mtconnectVersion, IObservation observation);
    }
}
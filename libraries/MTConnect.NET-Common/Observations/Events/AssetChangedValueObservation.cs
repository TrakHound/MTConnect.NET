// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// The event generated when an asset is added or changed. 
    /// AssetChanged MUST be discrete and the value of the DataItem’s discrete attribute MUST be true.
    /// </summary>
    public class AssetChangedValueObservation : EventValueObservation
    {
        /// <summary>
        /// The type of asset that was removed.
        /// </summary>
        public string AssetType
        {
            get => GetValue(ValueKeys.AssetType);
            set => AddValue(new ObservationValue(ValueKeys.AssetType, value));
        }

		/// <summary>
		/// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
		/// </summary>
		public string Hash
		{
			get => GetValue(ValueKeys.Hash);
			set => AddValue(new ObservationValue(ValueKeys.Hash, value));
		}
	}
}
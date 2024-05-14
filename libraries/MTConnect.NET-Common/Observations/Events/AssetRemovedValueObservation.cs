// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// The value of the Result for the event MUST be the assetId of the asset that has been removed. 
    /// The asset will still be visible if requested with the includeRemoved parameter as described in the protocol section. 
    /// When assets are removed they are not moved to the beginning of the most recently modified list.
    /// </summary>
    public class AssetRemovedValueObservation : EventValueObservation
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
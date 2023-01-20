// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
    }
}

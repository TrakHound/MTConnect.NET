// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
    }
}

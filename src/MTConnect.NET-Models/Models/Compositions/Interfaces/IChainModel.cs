// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public interface IChainModel : ICompositionModel
    {
        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        AccumulatedTimeValue AccumulatedTime { get; set; }
        IDataItemModel AccumulatedTimeDataItem { get; }
    }
}

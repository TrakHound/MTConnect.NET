// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;
using MTConnect.Models.DataItems;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public interface IBeltModel : ICompositionModel
    {
        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        AccumulatedTimeValue AccumulatedTime { get; set; }
        IDataItemModel AccumulatedTimeDataItem { get; }


        /// <summary>
        /// The time and date code associated with a material or other physical item.
        /// </summary>
        DateCodeModel DateCode { get; set; }
    }
}
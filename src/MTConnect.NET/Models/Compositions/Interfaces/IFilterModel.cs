// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.DataItems;
using MTConnect.Streams.Samples;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// Any substance or structure through which liquids or gases are passed to remove suspended impurities or to recover solids.
    /// </summary>
    public interface IFilterModel : ICompositionModel
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

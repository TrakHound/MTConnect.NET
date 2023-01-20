// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.DataItems;
using MTConnect.Observations.Samples.Values;

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
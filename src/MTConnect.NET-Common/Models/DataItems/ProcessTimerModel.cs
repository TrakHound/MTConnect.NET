// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
    /// </summary>
    public class ProcessTimerMold
    {
        /// <summary>
        /// The measurement of the time from the beginning of production of a part or product on a piece of equipment until the time that production is complete for that part or product on that piece of equipment.
        /// This includes the time that the piece of equipment is running, producing parts or products, or in the process of producing parts.
        /// </summary>
        public ProcessTimerValue Process { get; set; }
        public IDataItemModel ProcessDataItem { get; set; }

        /// <summary>
        /// The elapsed time of a temporary halt of action.
        /// </summary>
        public ProcessTimerValue Delay { get; set; }
        public IDataItemModel DelayDataItem { get; set; }
    }
}

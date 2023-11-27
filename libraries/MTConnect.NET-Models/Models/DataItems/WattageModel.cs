// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of power flowing through or dissipated by an electrical circuit or piece of equipment.
    /// </summary>
    public class WattageModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public WattageValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// The goal of the operation or process.
        /// </summary>
        public WattageValue Target { get; set; }
        public IDataItemModel TargetDataItem { get; set; }
    }
}
// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The rate of change in spatial volume of material deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionAccelerationVolumetricModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public DepositionAccelerationVolumetricValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public DepositionAccelerationVolumetricValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }
    }
}

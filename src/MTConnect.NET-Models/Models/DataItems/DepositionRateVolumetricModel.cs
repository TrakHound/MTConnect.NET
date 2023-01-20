// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The rate at which a spatial volume of material is deposited in an additive manufacturing process.
    /// </summary>
    public class DepositionRateVolumetricModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public DepositionRateVolumetricValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public DepositionRateVolumetricValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }
    }
}

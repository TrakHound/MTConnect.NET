// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The change of pressure per unit time.
    /// </summary>
    public class PressurizationRateModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public PressurizationRateValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public PressurizationRateValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public PressurizationRateValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }
    }
}

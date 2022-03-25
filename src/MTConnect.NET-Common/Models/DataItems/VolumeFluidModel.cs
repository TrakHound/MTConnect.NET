// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The fluid volume of an object or container.
    /// </summary>
    public class VolumeFluidModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public VolumeFluidValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Boundary when an activity or an event commences.
        /// </summary>
        public VolumeFluidValue Start { get; set; }
        public IDataItemModel StartDataItem { get; set; }

        /// <summary>
        /// Boundary when an activity or an event terminates.
        /// </summary>
        public VolumeFluidValue Ended { get; set; }
        public IDataItemModel EndedDataItem { get; set; }

        /// <summary>
        /// Reported or measured value of the amount used in the manufacturing process.
        /// </summary>
        public VolumeFluidValue Consumed { get; set; }
        public IDataItemModel ConsumedDataItem { get; set; }

        /// <summary>
        /// Reported or measured value of the amount discarded.
        /// </summary>
        public VolumeFluidValue Waste { get; set; }
        public IDataItemModel WasteDataItem { get; set; }

        /// <summary>
        /// Reported or measured value of amount included in the Part.
        /// </summary>
        public VolumeFluidValue Part { get; set; }
        public IDataItemModel PartDataItem { get; set; }
    }
}

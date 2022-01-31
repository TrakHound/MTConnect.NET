// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Streams.Samples;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The geometric volume of an object or container.
    /// </summary>
    public class VolumeSpatialModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public VolumeSpatialValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Boundary when an activity or an event commences.
        /// </summary>
        public VolumeSpatialValue Start { get; set; }
        public IDataItemModel StartDataItem { get; set; }

        /// <summary>
        /// Boundary when an activity or an event terminates.
        /// </summary>
        public VolumeSpatialValue Ended { get; set; }
        public IDataItemModel EndedDataItem { get; set; }

        /// <summary>
        /// Reported or measured value of the amount used in the manufacturing process.
        /// </summary>
        public VolumeSpatialValue Consumed { get; set; }
        public IDataItemModel ConsumedDataItem { get; set; }

        /// <summary>
        /// Reported or measured value of the amount discarded.
        /// </summary>
        public VolumeSpatialValue Waste { get; set; }
        public IDataItemModel WasteDataItem { get; set; }

        /// <summary>
        /// Reported or measured value of amount included in the Part.
        /// </summary>
        public VolumeSpatialValue Part { get; set; }
        public IDataItemModel PartDataItem { get; set; }
    }
}

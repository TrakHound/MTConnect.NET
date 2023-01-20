// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the length of an object.
    /// </summary>
    public class LengthModel
    {
        /// <summary>
        /// The standard or original length of an object.
        /// </summary>
        public LengthValue Standard { get; set; }
        public IDataItemModel StandardDataItem { get; set; }

        /// <summary>
        /// The remaining total length of an object.
        /// </summary>
        public LengthValue Remaining { get; set; }
        public IDataItemModel RemainingDataItem { get; set; }

        /// <summary>
        /// The remaining useable length of an object.
        /// </summary>
        public LengthValue Useable { get; set; }
        public IDataItemModel UseableDataItem { get; set; }
    }
}

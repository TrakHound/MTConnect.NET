// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// Designates the layers of material applied to a part or product as part of an additive manufacturing process.
    /// </summary>
    public class MaterialLayerModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public int Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// The goal of the operation or process.
        /// </summary>
        public int Target { get; set; }
        public IDataItemModel TargetDataItem { get; set; }
    }
}
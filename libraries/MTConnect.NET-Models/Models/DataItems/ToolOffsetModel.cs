// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// A reference to the tool offset variables applied to the active cutting tool.
    /// </summary>
    public class ToolOffsetModel
    {
        /// <summary>
        /// A reference to a length type tool offset.
        /// </summary>
        public string Length { get; set; }
        public IDataItemModel LengthDataItem { get; set; }

        /// <summary>
        /// A reference to a radial type tool offset.
        /// </summary>
        public string Radial { get; set; }
        public IDataItemModel RadialDataItem { get; set; }
    }
}
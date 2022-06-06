// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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

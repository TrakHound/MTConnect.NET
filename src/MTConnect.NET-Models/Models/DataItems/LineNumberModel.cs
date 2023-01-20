// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// A reference to the position of a block of program code within a control program.
    /// </summary>
    public class LineNumberModel
    {
        /// <summary>
        /// The position of a block of program code relative to the beginning of the control program.
        /// </summary>
        public int Absolute { get; set; }
        public IDataItemModel AbsoluteDataItem { get; set; }

        /// <summary>
        /// The position of a block of program code relative to the occurrence of the last LINE_LABEL encountered in the control program.
        /// </summary>
        public int Incremental { get; set; }
        public IDataItemModel IncrementalDataItem { get; set; }
    }
}
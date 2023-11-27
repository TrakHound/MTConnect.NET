// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The logic or motion program being executed
    /// </summary>
    public class ProgramModel
    {
        /// <summary>
        /// The identity of the logic or motion program being executed.
        /// </summary>
        public string Program { get; set; }
        public IDataItemModel ProgramDataItem { get; set; }

        /// <summary>
        /// The non-executable header section of the control program.
        /// </summary>
        public string Header { get; set; }
        public IDataItemModel HeaderDataItem { get; set; }

        /// <summary>
        /// A comment or non-executable statement in the control program
        /// </summary>
        public string Comment { get; set; }
        public IDataItemModel CommentDataItem { get; set; }

        /// <summary>
        /// The Uniform Resource Identifier(URI) for the source file
        /// </summary>
        public string Location { get; set; }
        public IDataItemModel LocationDataItem { get; set; }

        /// <summary>
        /// Defines whether the logic or motion program defined by PROGRAM is being executed from the local memory of the controller or from an outside source.
        /// </summary>
        public ProgramLocationType LocationType { get; set; }
        public IDataItemModel LocationTypeDataItem { get; set; }

        /// <summary>
        /// The File Asset is an AbstractFile with information about the File instance and its URL.
        /// </summary>
        public MTConnect.Assets.Files.FileAsset File { get; set; }
    }
}
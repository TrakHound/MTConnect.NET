// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Models.DataItems;

namespace MTConnect.Models.Assets
{
    public class CuttingToolModel
    {
        /// <summary>
        /// The identifier assigned by the Controller component to a cutting tool when in use by a piece of equipment.
        /// </summary>
        public string Number { get; set; }
        public IDataItemModel NumberDataItem { get; set; }

        /// <summary>
        /// An identifier for the tool group associated with a specific tool. Commonly used to designate spare tools.
        /// </summary>
        public string Group { get; set; }
        public IDataItemModel GroupDataItem { get; set; }

        /// <summary>
        /// A reference to the tool offset variables applied to the active cutting tool.
        /// </summary>
        public ToolOffsetModel Offset { get; set; }

        /// <summary>
        /// The identifier of an individual tool asset.
        /// </summary>
        public string AssetId { get; set; }

        public CuttingToolAsset Asset { get; set; }
    }
}
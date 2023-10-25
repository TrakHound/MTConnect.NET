// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of the tool currently in use for a given `Path`.**DEPRECATED** in *Version 1.2.0*.   See `TOOL_ASSET_ID`.
    /// </summary>
    public class ToolIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_ID";
        public const string NameId = "";
             
        public new const string DescriptionText = "Identifier of the tool currently in use for a given `Path`.**DEPRECATED** in *Version 1.2.0*.   See `TOOL_ASSET_ID`.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version12;
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public ToolIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ToolIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
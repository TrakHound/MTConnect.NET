// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of an individual tool asset.
    /// </summary>
    public class ToolAssetIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_ASSET_ID";
        public const string NameId = "toolAssetId";
             
             
        public new const string DescriptionText = "Identifier of an individual tool asset.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ToolAssetIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ToolAssetIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Assetid of the Asset that has been removed.
    /// </summary>
    public class AssetRemovedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSET_REMOVED";
        public const string NameId = "";
             
        public new const string DescriptionText = "Assetid of the Asset that has been removed.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public AssetRemovedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AssetRemovedDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
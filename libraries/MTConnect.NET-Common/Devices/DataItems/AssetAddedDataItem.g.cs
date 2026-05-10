// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1744799118784_270323_23376

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// AssetId of the Asset that has been added.
    /// </summary>
    public class AssetAddedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSET_ADDED";
        public const string NameId = "assetAdded";
             
             
        public new const string DescriptionText = "AssetId of the Asset that has been added.";
        
        public override string TypeDescription => DescriptionText;
        
               


        public AssetAddedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public AssetAddedDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
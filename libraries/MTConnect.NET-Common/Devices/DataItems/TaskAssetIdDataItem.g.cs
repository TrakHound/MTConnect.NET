// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760879979899_265830_4037

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// AssetId of the Task that the Component binds to
    /// </summary>
    public class TaskAssetIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TASK_ASSET_ID";
        public const string NameId = "taskAssetId";
             
             
        public new const string DescriptionText = "AssetId of the Task that the Component binds to";
        
        public override string TypeDescription => DescriptionText;
        
               


        public TaskAssetIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public TaskAssetIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
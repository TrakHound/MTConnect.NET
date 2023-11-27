// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Total count of the number of blocks of program code that have been executed since execution started.
    /// </summary>
    public class BlockCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "BLOCK_COUNT";
        public const string NameId = "blockCount";
             
        public new const string DescriptionText = "Total count of the number of blocks of program code that have been executed since execution started.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public BlockCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public BlockCountDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
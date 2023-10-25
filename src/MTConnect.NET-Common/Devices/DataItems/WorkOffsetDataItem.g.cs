// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Reference to offset variables for a work piece or part.
    /// </summary>
    public class WorkOffsetDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WORK_OFFSET";
        public const string NameId = "";
             
        public new const string DescriptionText = "Reference to offset variables for a work piece or part.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public WorkOffsetDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public WorkOffsetDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
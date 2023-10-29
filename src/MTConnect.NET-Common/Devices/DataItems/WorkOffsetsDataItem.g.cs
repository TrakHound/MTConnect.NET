// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Properties of each addressable work offset.
    /// </summary>
    public class WorkOffsetsDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WORK_OFFSETS";
        public const string NameId = "workOffsets";
             
        public new const string DescriptionText = "Properties of each addressable work offset.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public WorkOffsetsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public WorkOffsetsDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
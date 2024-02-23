// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218488_839923_2316

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Reference to offset variables for a work piece or part.
    /// </summary>
    public class WorkOffsetDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WORK_OFFSET";
        public const string NameId = "workOffset";
             
             
        public new const string DescriptionText = "Reference to offset variables for a work piece or part.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public WorkOffsetDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
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
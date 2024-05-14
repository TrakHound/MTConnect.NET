// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218144_231993_1515

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// **DEPRECATED:** Replaced with `CONDITION` category data items in Version 1.1.0.
    /// </summary>
    public class AlarmDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ALARM";
        public const string NameId = "alarm";
             
             
        public new const string DescriptionText = "**DEPRECATED:** Replaced with `CONDITION` category data items in Version 1.1.0.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version11;
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public AlarmDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public AlarmDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
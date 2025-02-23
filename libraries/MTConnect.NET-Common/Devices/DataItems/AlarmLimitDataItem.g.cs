// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605646964681_939072_3338

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of limits used to trigger warning or alarm indicators.**DEPRECATED** in *Version 2.5*. Replaced by  `ALARM_LIMITS`.
    /// </summary>
    public class AlarmLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ALARM_LIMIT";
        public const string NameId = "alarmLimit";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.TABLE;     
             
        public new const string DescriptionText = "Set of limits used to trigger warning or alarm indicators.**DEPRECATED** in *Version 2.5*. Replaced by  `ALARM_LIMITS`.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public AlarmLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public AlarmLimitDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}
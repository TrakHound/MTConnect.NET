// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of limits used to trigger warning or alarm indicators.
    /// </summary>
    public class AlarmLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ALARM_LIMIT";
        public const string NameId = "";
             
        public new const string DescriptionText = "Set of limits used to trigger warning or alarm indicators.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public AlarmLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AlarmLimitDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
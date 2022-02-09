// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A set of limits used to trigger warning or alarm indicators.
    /// </summary>
    public class AlarmLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ALARM_LIMIT";
        public const string NameId = "alarmLimit";


        public AlarmLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public AlarmLimitDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

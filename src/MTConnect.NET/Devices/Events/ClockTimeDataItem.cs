// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The value provided by a timing device at a specific point in time.
    /// </summary>
    public class ClockTimeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CLOCK_TIME";
        public const string NameId = "clockTime";


        public ClockTimeDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ClockTimeDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

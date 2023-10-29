// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Time provided by a timing device at a specific point in time.
    /// </summary>
    public class ClockTimeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CLOCK_TIME";
        public const string NameId = "clockTime";
             
        public new const string DescriptionText = "Time provided by a timing device at a specific point in time.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ClockTimeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ClockTimeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
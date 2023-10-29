// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Present status of the battery.
    /// </summary>
    public class BatteryStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "BATTERY_STATE";
        public const string NameId = "batteryState";
             
        public new const string DescriptionText = "Present status of the battery.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public BatteryStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public BatteryStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
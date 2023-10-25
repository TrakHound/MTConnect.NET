// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// State of the axis lockout function when power has been removed and the axis is allowed to move freely.
    /// </summary>
    public class AxisInterlockDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "AXIS_INTERLOCK";
        public const string NameId = "";
             
        public new const string DescriptionText = "State of the axis lockout function when power has been removed and the axis is allowed to move freely.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public AxisInterlockDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AxisInterlockDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
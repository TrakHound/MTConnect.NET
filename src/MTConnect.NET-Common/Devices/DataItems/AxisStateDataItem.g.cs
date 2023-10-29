// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// State of a Linear or Rotary component representing an axis.
    /// </summary>
    public class AxisStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "AXIS_STATE";
        public const string NameId = "axisState";
             
        public new const string DescriptionText = "State of a Linear or Rotary component representing an axis.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public AxisStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public AxisStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
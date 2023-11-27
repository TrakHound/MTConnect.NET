// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Current operating mode for a Rotary type axis.
    /// </summary>
    public class RotaryModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ROTARY_MODE";
        public const string NameId = "rotaryMode";
             
        public new const string DescriptionText = "Current operating mode for a Rotary type axis.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public RotaryModeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public RotaryModeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
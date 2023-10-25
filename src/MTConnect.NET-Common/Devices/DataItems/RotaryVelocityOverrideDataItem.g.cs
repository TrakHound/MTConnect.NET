// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Percentage change to the velocity of the programmed velocity for a Rotary axis.
    /// </summary>
    public class RotaryVelocityOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ROTARY_VELOCITY_OVERRIDE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Percentage change to the velocity of the programmed velocity for a Rotary axis.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public RotaryVelocityOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public RotaryVelocityOverrideDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
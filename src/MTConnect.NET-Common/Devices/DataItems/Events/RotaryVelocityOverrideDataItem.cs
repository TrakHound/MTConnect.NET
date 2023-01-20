// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The value of a command issued to adjust the programmed velocity for a Rotary type axis.
    /// </summary>
    public class RotaryVelocityOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ROTARY_VELOCITY_OVERRIDE";
        public const string NameId = "ovr";
        public new const string DescriptionText = "The value of a command issued to adjust the programmed velocity for a Rotary type axis.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public RotaryVelocityOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public RotaryVelocityOverrideDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = Devices.Units.PERCENT;
        }
    }
}
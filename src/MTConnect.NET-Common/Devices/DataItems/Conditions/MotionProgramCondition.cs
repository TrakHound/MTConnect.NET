// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication that an error occurred in the motion program associated with a piece of equipment.
    /// </summary>
    public class MotionProgramCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "MOTION_PROGRAM";
        public const string NameId = "motionProgram";
        public new const string DescriptionText = "An indication that an error occurred in the motion program associated with a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public MotionProgramCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MotionProgramCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
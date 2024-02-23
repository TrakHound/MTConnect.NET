// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1598552917043_472168_592

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication that an error occurred in the motion program associated with a piece of equipment.
    /// </summary>
    public class MotionProgramDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "MOTION_PROGRAM";
        public const string NameId = "motionProgram";
             
             
        public new const string DescriptionText = "Indication that an error occurred in the motion program associated with a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public MotionProgramDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public MotionProgramDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
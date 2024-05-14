// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1598552904415_200781_584

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication that an error occurred in the logic program or programmable logic controller (PLC) associated with a piece of equipment.
    /// </summary>
    public class LogicProgramDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "LOGIC_PROGRAM";
        public const string NameId = "logicProgram";
             
             
        public new const string DescriptionText = "Indication that an error occurred in the logic program or programmable logic controller (PLC) associated with a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public LogicProgramDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public LogicProgramDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
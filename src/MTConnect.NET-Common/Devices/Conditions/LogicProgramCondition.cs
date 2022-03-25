// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication that an error occurred in the logic program or programmable logic controller(PLC) associated with a piece of equipment.
    /// </summary>
    public class LogicProgramCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "LOGIC_PROGRAM";
        public const string NameId = "logicProgram";
        public new const string DescriptionText = "An indication that an error occurred in the logic program or programmable logic controller(PLC) associated with a piece of equipment.";

        public override string TypeDescription => DescriptionText;


        public LogicProgramCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public LogicProgramCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

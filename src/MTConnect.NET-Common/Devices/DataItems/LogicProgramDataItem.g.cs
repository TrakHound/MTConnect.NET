// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication that an error occurred in the logic program or programmable logic controller (PLC) associated with a piece of equipment.
    /// </summary>
    public class LogicProgramDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "LOGIC_PROGRAM";
        public const string NameId = "";
             
        public new const string DescriptionText = "Indication that an error occurred in the logic program or programmable logic controller (PLC) associated with a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public LogicProgramDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
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
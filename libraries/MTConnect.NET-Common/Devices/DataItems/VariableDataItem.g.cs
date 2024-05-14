// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218466_234870_2250

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Data whose meaning may change over time due to changes in the operation of a piece of equipment or the process being executed on that piece of equipment.
    /// </summary>
    public class VariableDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "VARIABLE";
        public const string NameId = "variable";
             
             
        public new const string DescriptionText = "Data whose meaning may change over time due to changes in the operation of a piece of equipment or the process being executed on that piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public VariableDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public VariableDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
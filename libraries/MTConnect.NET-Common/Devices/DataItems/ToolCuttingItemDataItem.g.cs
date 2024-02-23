// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1696090268084_365387_2771

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indices of the currently active cutting tool edge.
    /// </summary>
    public class ToolCuttingItemDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_CUTTING_ITEM";
        public const string NameId = "toolCuttingItem";
             
             
        public new const string DescriptionText = "Indices of the currently active cutting tool edge.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version23;       


        public ToolCuttingItemDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ToolCuttingItemDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
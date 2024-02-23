// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218454_932545_2217

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier for the tool group associated with a specific tool. Commonly used to designate spare tools.
    /// </summary>
    public class ToolGroupDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_GROUP";
        public const string NameId = "toolGroup";
             
             
        public new const string DescriptionText = "Identifier for the tool group associated with a specific tool. Commonly used to designate spare tools.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public ToolGroupDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ToolGroupDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
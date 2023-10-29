// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
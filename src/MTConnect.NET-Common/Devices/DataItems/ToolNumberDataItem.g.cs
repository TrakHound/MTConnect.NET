// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier assigned by the Controller component to a cutting tool when in use by a piece of equipment.
    /// </summary>
    public class ToolNumberDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_NUMBER";
        public const string NameId = "";
             
        public new const string DescriptionText = "Identifier assigned by the Controller component to a cutting tool when in use by a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ToolNumberDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ToolNumberDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
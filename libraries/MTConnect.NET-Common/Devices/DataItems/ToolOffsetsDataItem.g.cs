// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Properties of each addressable tool offset.
    /// </summary>
    public class ToolOffsetsDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_OFFSETS";
        public const string NameId = "toolOffsets";
             
        public new const string DescriptionText = "Properties of each addressable tool offset.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public ToolOffsetsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ToolOffsetsDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
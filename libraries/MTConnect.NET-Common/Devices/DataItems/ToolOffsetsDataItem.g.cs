// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1678189804769_814006_17113

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
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.TABLE;     
             
        public new const string DescriptionText = "Properties of each addressable tool offset.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public ToolOffsetsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public ToolOffsetsDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}
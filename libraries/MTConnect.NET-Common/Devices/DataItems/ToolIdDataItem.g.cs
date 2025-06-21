// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218456_275899_2220

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of the tool currently in use for a given `Path`.**DEPRECATED** in *Version 1.2.0*.   See `TOOL_NUMBER`.
    /// </summary>
    public class ToolIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_ID";
        public const string NameId = "toolId";
             
             
        public new const string DescriptionText = "Identifier of the tool currently in use for a given `Path`.**DEPRECATED** in *Version 1.2.0*.   See `TOOL_NUMBER`.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version12;
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public ToolIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ToolIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1678101504782_455626_16476

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Event that represents a Component where the EntryDefinition identifies the Component and the CellDefinitions define the Component's observed DataItems.
    /// </summary>
    public class ComponentDataDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "COMPONENT_DATA";
        public const string NameId = "componentData";
             
             
        public new const string DescriptionText = "Event that represents a Component where the EntryDefinition identifies the Component and the CellDefinitions define the Component's observed DataItems.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public ComponentDataDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ComponentDataDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
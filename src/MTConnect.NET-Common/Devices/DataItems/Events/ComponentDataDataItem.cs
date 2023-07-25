// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Tabular Event that represents a Component where the EntryDefinition identifies the Component and the CellDefinitions define the Component’s observed DataItems.
	/// If the Component multiplicity can be determined, the device model MUST use a fixed set of Components.
	/// ComponentData MUST provide a DataItem Definition.
	/// </summary>
	public class ComponentDataDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "COMPONENT_DATA";
        public const string NameId = "componentData";
        public new const string DescriptionText = "Tabular Event that represents a Component where the EntryDefinition identifies the Component and the CellDefinitions define the Component’s observed DataItems. If the Component multiplicity can be determined, the device model MUST use a fixed set of Components. ComponentData MUST provide a DataItem Definition.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        public ComponentDataDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Representation = DataItemRepresentation.TABLE;
        }

        public ComponentDataDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Representation = DataItemRepresentation.TABLE;
            Type = TypeId;
            Name = NameId;
        }
    }
}
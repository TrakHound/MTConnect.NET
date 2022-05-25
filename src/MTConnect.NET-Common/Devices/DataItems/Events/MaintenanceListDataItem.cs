// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Actions or activities to be performed in support of a piece of equipment.
    /// </summary>
    public class MaintenanceListDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MAINTENANCE_LIST";
        public const string NameId = "maintenanceList";
        public new const string DescriptionText = "Actions or activities to be performed in support of a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version20;


        public MaintenanceListDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Representation = DataItemRepresentation.DATA_SET;
        }

        public MaintenanceListDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Representation = DataItemRepresentation.DATA_SET;
            Type = TypeId;
            Name = NameId;
        }
    }
}

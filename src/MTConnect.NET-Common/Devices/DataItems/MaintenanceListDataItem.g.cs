// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
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
            
        }

        public MaintenanceListDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
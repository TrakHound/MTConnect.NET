// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to change the type of material or product being loaded or fed to a piece of equipment.
    /// </summary>
    public class MaterialChangeDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MATERIAL_CHANGE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Operating state of the service to change the type of material or product being loaded or fed to a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public MaterialChangeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public MaterialChangeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
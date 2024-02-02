// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to load a piece of material or product.
    /// </summary>
    public class MaterialLoadDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MATERIAL_LOAD";
        public const string NameId = "materialLoad";
             
        public new const string DescriptionText = "Operating state of the service to load a piece of material or product.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public MaterialLoadDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public MaterialLoadDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
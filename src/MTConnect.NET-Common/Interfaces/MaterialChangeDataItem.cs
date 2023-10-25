// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to change the type of material or product being loaded or fed to a piece of equipment.
    /// </summary>
    public class MaterialChangeDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MATERIAL_CHANGE";
        public const string NameId = "materialChange";
        public new const string DescriptionText = "Operating state of the service to change the type of material or product being loaded or fed to a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public MaterialChangeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MaterialChangeDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }
    }
}
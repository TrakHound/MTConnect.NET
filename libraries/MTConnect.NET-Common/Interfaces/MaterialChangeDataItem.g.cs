// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
        public const string NameId = "materialChange";
        
        public new const string DescriptionText = "Operating state of the service to change the type of material or product being loaded or fed to a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public new enum SubTypes
        {
            /// <summary>
            /// Operating state of the request to change the type of material or product being loaded or fed to a piece of equipment.
            /// </summary>
            REQUEST,
            
            /// <summary>
            /// Operating state of the response to a request to change the type of material or product being loaded or fed to a piece of equipment.
            /// </summary>
            RESPONSE
        }


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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public new static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.REQUEST: return "Operating state of the request to change the type of material or product being loaded or fed to a piece of equipment.";
                case SubTypes.RESPONSE: return "Operating state of the response to a request to change the type of material or product being loaded or fed to a piece of equipment.";
            }

            return null;
        }

        public new static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.REQUEST: return "REQUEST";
                case SubTypes.RESPONSE: return "RESPONSE";
            }

            return null;
        }

    }
}

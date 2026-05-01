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


        public new enum SubTypes
        {
            /// <summary>
            /// Operating state of the request to load a piece of material or product.
            /// </summary>
            REQUEST,
            
            /// <summary>
            /// Operating state of the response to a request to load a piece of material or product.
            /// </summary>
            RESPONSE
        }


        public MaterialLoadDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public MaterialLoadDataItem(
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
                case SubTypes.REQUEST: return "Operating state of the request to load a piece of material or product.";
                case SubTypes.RESPONSE: return "Operating state of the response to a request to load a piece of material or product.";
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

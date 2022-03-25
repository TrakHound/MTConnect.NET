// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// Designates the layers of material applied to a part or product as part of an additive manufacturing process.
    /// </summary>
    public class MaterialLayerDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MATERIAL_LAYER";
        public const string NameId = "materialLayer";
        public new const string DescriptionText = "Designates the layers of material applied to a part or product as part of an additive manufacturing process.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// The measured or reported value of an observation.
            /// </summary>
            ACTUAL,

            /// <summary>
            /// The goal of the operation or process.
            /// </summary>
            TARGET
        }


        public MaterialLayerDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MaterialLayerDataItem(
            string parentId,
            SubTypes subType = SubTypes.ACTUAL
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTUAL: return "The measured or reported value of an observation.";
                case SubTypes.TARGET: return "The goal of the operation or process.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "act";
                case SubTypes.TARGET: return "target";
            }

            return null;
        }
    }
}

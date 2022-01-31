// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The measurement of the hardness of a material.
    /// </summary>
    public class HardnessDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "HARDNESS";
        public const string NameId = "hardness";

        public enum SubTypes
        {
            /// <summary>
            /// A scale to measure the resistance to deformation of a surface.
            /// </summary>
            ROCKWELL,

            /// <summary>
            /// A scale to measure the resistance to deformation of a surface.
            /// </summary>
            VICKERS,

            /// <summary>
            /// A scale to measure the resistance to deformation of a surface.
            /// </summary>
            SHORE,

            /// <summary>
            /// A scale to measure the resistance to deformation of a surface.
            /// </summary>
            BRINELL,

            /// <summary>
            /// A scale to measure the elasticity of a surface.
            /// </summary>
            LEEB,

            /// <summary>
            /// A scale to measure the resistance to scratching of a surface.
            /// </summary>
            MOHS
        }


        public HardnessDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public HardnessDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.BRINELL: return "brinell";
                case SubTypes.LEEB: return "leeb";
                case SubTypes.MOHS: return "mohs";
                case SubTypes.ROCKWELL: return "rockwell";
                case SubTypes.SHORE: return "shore";
                case SubTypes.VICKERS: return "vickers";
            }

            return null;
        }
    }
}

// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The measurement of the hardness of a material.
    /// </summary>
    public class HardnessDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "HARDNESS";
        public const string NameId = "hardness";
        public new const string DescriptionText = "The measurement of the hardness of a material.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;

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
            Category = CategoryId;
            Type = TypeId;
        }

        public HardnessDataItem(
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

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ROCKWELL: return "A scale to measure the resistance to deformation of a surface.";
                case SubTypes.VICKERS: return "A scale to measure the resistance to deformation of a surface.";
                case SubTypes.SHORE: return "A scale to measure the resistance to deformation of a surface.";
                case SubTypes.BRINELL: return "A scale to measure the resistance to deformation of a surface.";
                case SubTypes.LEEB: return "A scale to measure the elasticity of a surface.";
                case SubTypes.MOHS: return "A scale to measure the resistance to scratching of a surface.";
            }

            return null;
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
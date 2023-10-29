// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Hardness of a material.
    /// </summary>
    public class HardnessDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "HARDNESS";
        public const string NameId = "hardness";
             
        public new const string DescriptionText = "Hardness of a material.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Scale to measure the resistance to deformation of a surface.
            /// </summary>
            ROCKWELL,
            
            /// <summary>
            /// Scale to measure the resistance to deformation of a surface.
            /// </summary>
            VICKERS,
            
            /// <summary>
            /// Scale to measure the resistance to deformation of a surface.
            /// </summary>
            SHORE,
            
            /// <summary>
            /// Scale to measure the resistance to deformation of a surface.
            /// </summary>
            BRINELL,
            
            /// <summary>
            /// Scale to measure the elasticity of a surface.
            /// </summary>
            LEEB,
            
            /// <summary>
            /// Scale to measure the resistance to scratching of a surface.
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
                case SubTypes.ROCKWELL: return "Scale to measure the resistance to deformation of a surface.";
                case SubTypes.VICKERS: return "Scale to measure the resistance to deformation of a surface.";
                case SubTypes.SHORE: return "Scale to measure the resistance to deformation of a surface.";
                case SubTypes.BRINELL: return "Scale to measure the resistance to deformation of a surface.";
                case SubTypes.LEEB: return "Scale to measure the elasticity of a surface.";
                case SubTypes.MOHS: return "Scale to measure the resistance to scratching of a surface.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ROCKWELL: return "ROCKWELL";
                case SubTypes.VICKERS: return "VICKERS";
                case SubTypes.SHORE: return "SHORE";
                case SubTypes.BRINELL: return "BRINELL";
                case SubTypes.LEEB: return "LEEB";
                case SubTypes.MOHS: return "MOHS";
            }

            return null;
        }

    }
}
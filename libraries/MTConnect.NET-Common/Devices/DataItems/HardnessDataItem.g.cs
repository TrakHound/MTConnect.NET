// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218314_342350_1839

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Hardness of a material.
    /// </summary>
    public class HardnessDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "HARDNESS";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "hardness";

        /// <summary>
        /// The default <c>representation</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Hardness of a material.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
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


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public HardnessDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
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
            Representation = DefaultRepresentation; 
            
        }

        /// <summary>
        /// The MTConnect Standard description of this DataItem's current <c>subType</c>.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the MTConnect Standard description for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
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

        /// <summary>
        /// Returns the string identifier for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
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
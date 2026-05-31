// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727728953400_643342_24554

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Number of particles counted by their size or other characteristics.
    /// </summary>
    public class ParticleCountDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PARTICLE_COUNT";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "particleCount";

        /// <summary>
        /// The default <c>units</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const string DefaultUnits = Devices.Units.COUNT;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Number of particles counted by their size or other characteristics.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version25;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// GAS
            /// </summary>
            GAS,
            
            /// <summary>
            /// LIQUID
            /// </summary>
            LIQUID,
            
            /// <summary>
            /// SOLID
            /// </summary>
            SOLID
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public ParticleCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
        public ParticleCountDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
             
            Units = DefaultUnits;
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
                case SubTypes.GAS: return "GAS";
                case SubTypes.LIQUID: return "LIQUID";
                case SubTypes.SOLID: return "SOLID";
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
                case SubTypes.GAS: return "GAS";
                case SubTypes.LIQUID: return "LIQUID";
                case SubTypes.SOLID: return "SOLID";
            }

            return null;
        }

    }
}
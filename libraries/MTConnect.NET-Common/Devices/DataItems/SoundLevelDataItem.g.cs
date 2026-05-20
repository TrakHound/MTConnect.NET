// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218442_613083_2169

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Sound level or sound pressure level relative to atmospheric pressure.
    /// </summary>
    public class SoundLevelDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "SOUND_LEVEL";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "soundLevel";

        /// <summary>
        /// The default <c>units</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const string DefaultUnits = Devices.Units.DECIBEL;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Sound level or sound pressure level relative to atmospheric pressure.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// No weighting factor on the frequency scale
            /// </summary>
            NO_SCALE,
            
            /// <summary>
            /// A Scale weighting factor.   This is the default weighting factor if no factor is specified
            /// </summary>
            A_SCALE,
            
            /// <summary>
            /// B Scale weighting factor
            /// </summary>
            B_SCALE,
            
            /// <summary>
            /// C Scale weighting factor
            /// </summary>
            C_SCALE,
            
            /// <summary>
            /// D Scale weighting factor
            /// </summary>
            D_SCALE
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public SoundLevelDataItem()
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
        public SoundLevelDataItem(
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
                case SubTypes.NO_SCALE: return "No weighting factor on the frequency scale";
                case SubTypes.A_SCALE: return "A Scale weighting factor.   This is the default weighting factor if no factor is specified";
                case SubTypes.B_SCALE: return "B Scale weighting factor";
                case SubTypes.C_SCALE: return "C Scale weighting factor";
                case SubTypes.D_SCALE: return "D Scale weighting factor";
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
                case SubTypes.NO_SCALE: return "NO_SCALE";
                case SubTypes.A_SCALE: return "A_SCALE";
                case SubTypes.B_SCALE: return "B_SCALE";
                case SubTypes.C_SCALE: return "C_SCALE";
                case SubTypes.D_SCALE: return "D_SCALE";
            }

            return null;
        }

    }
}
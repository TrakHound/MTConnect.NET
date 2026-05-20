// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218222_423849_1632

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Operating state of a mechanism represented by a Composition entity.
    /// </summary>
    public class CompositionStateDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "COMPOSITION_STATE";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "compositionState";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Operating state of a mechanism represented by a Composition entity.";

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
            /// Indication of the operating state of a mechanism.
            /// </summary>
            ACTION,
            
            /// <summary>
            /// Indication of the position of a mechanism that may move in a lateral direction.
            /// </summary>
            LATERAL,
            
            /// <summary>
            /// Indication of the open or closed state of a mechanism.
            /// </summary>
            MOTION,
            
            /// <summary>
            /// Indication of the activation state of a mechanism.
            /// </summary>
            SWITCHED,
            
            /// <summary>
            /// Indication of the position of a mechanism that may move in a vertical direction.
            /// </summary>
            VERTICAL
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public CompositionStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
        public CompositionStateDataItem(
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
                case SubTypes.ACTION: return "Indication of the operating state of a mechanism.";
                case SubTypes.LATERAL: return "Indication of the position of a mechanism that may move in a lateral direction.";
                case SubTypes.MOTION: return "Indication of the open or closed state of a mechanism.";
                case SubTypes.SWITCHED: return "Indication of the activation state of a mechanism.";
                case SubTypes.VERTICAL: return "Indication of the position of a mechanism that may move in a vertical direction.";
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
                case SubTypes.ACTION: return "ACTION";
                case SubTypes.LATERAL: return "LATERAL";
                case SubTypes.MOTION: return "MOTION";
                case SubTypes.SWITCHED: return "SWITCHED";
                case SubTypes.VERTICAL: return "VERTICAL";
            }

            return null;
        }

    }
}
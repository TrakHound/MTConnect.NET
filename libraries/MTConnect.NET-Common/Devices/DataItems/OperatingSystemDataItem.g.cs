// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_91b028d_1587749356759_918178_2174

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Operating System (OS) of a Component.
    /// </summary>
    public class OperatingSystemDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "OPERATING_SYSTEM";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "operatingSystem";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Operating System (OS) of a Component.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// License code to validate or activate the hardware or software.
            /// </summary>
            LICENSE,
            
            /// <summary>
            /// Version of the hardware or software.
            /// </summary>
            VERSION,
            
            /// <summary>
            /// Date the hardware or software was released for general use.
            /// </summary>
            RELEASE_DATE,
            
            /// <summary>
            /// Date the hardware or software was installed.
            /// </summary>
            INSTALL_DATE,
            
            /// <summary>
            /// Corporate identity for the maker of the hardware or software.
            /// </summary>
            MANUFACTURER
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public OperatingSystemDataItem()
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
        public OperatingSystemDataItem(
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
                case SubTypes.LICENSE: return "License code to validate or activate the hardware or software.";
                case SubTypes.VERSION: return "Version of the hardware or software.";
                case SubTypes.RELEASE_DATE: return "Date the hardware or software was released for general use.";
                case SubTypes.INSTALL_DATE: return "Date the hardware or software was installed.";
                case SubTypes.MANUFACTURER: return "Corporate identity for the maker of the hardware or software.";
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
                case SubTypes.LICENSE: return "LICENSE";
                case SubTypes.VERSION: return "VERSION";
                case SubTypes.RELEASE_DATE: return "RELEASE_DATE";
                case SubTypes.INSTALL_DATE: return "INSTALL_DATE";
                case SubTypes.MANUFACTURER: return "MANUFACTURER";
            }

            return null;
        }

    }
}
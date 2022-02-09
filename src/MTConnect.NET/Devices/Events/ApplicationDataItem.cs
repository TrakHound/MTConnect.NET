// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The application on a component.
    /// </summary>
    public class ApplicationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "APPLICATION";
        public const string NameId = "application";

        public enum SubTypes
        {
            /// <summary>
            /// The date the hardware or software was installed.
            /// </summary>
            INSTALL_DATE,

            /// <summary>
            /// The license code to validate or activate the hardware or software.
            /// </summary>
            LICENSE,

            /// <summary>
            /// The corporate identity for the maker of the hardware or software.
            /// </summary>
            MANUFACTURER,

            /// <summary>
            /// The date the hardware or software was released for general use.
            /// </summary>
            RELEASE_DATE,

            /// <summary>
            /// The version of the hardware or software.
            /// </summary>
            VERSION
        }


        public ApplicationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ApplicationDataItem(
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


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.INSTALL_DATE: return "installDate";
                case SubTypes.LICENSE: return "license";
                case SubTypes.MANUFACTURER: return "manufacturer";
                case SubTypes.RELEASE_DATE: return "relDate";
                case SubTypes.VERSION: return "version";
            }

            return null;
        }
    }
}

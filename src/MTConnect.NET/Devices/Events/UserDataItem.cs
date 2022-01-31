// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The identifier of the person currently responsible for operating the piece of equipment.
    /// </summary>
    public class UserDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "USER";
        public const string NameId = "user";

        public enum SubTypes
        {
            /// <summary>
            /// The identifier of the person currently responsible for performing maintenance on the piece of equipment.
            /// </summary>
            MAINTENANCE,

            /// <summary>
            /// The identifier of the person currently responsible for operating the piece of equipment.
            /// </summary>
            OPERATOR,

            /// <summary>
            /// The identifier of the person currently responsible for preparing a piece of equipment for production
            /// or restoring the piece of equipment to a neutral state after production.
            /// </summary>
            SET_UP
        }


        public UserDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public UserDataItem(
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
                case SubTypes.MAINTENANCE: return "maintenance";
                case SubTypes.OPERATOR: return "operator";
                case SubTypes.SET_UP: return "setup";
            }

            return null;
        }
    }
}

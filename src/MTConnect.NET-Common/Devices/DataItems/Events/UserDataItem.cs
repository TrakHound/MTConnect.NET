// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The identifier of the person currently responsible for operating the piece of equipment.
    /// </summary>
    public class UserDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "USER";
        public const string NameId = "user";
        public new const string DescriptionText = "The identifier of the person currently responsible for operating the piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;

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
            Category = CategoryId;
            Type = TypeId;
        }

        public UserDataItem(
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
                case SubTypes.MAINTENANCE: return "The identifier of the person currently responsible for performing maintenance on the piece of equipment.";
                case SubTypes.OPERATOR: return "The identifier of the person currently responsible for operating the piece of equipment.";
                case SubTypes.SET_UP: return "The identifier of the person currently responsible for preparing a piece of equipment for production or restoring the piece of equipment to a neutral state after production.";
            }

            return null;
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

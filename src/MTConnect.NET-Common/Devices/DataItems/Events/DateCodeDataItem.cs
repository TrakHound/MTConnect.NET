// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The time and date code associated with a material or other physical item.
    /// </summary>
    public class DateCodeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DATE_CODE";
        public const string NameId = "dateCode";
        public new const string DescriptionText = "The time and date code associated with a material or other physical item.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;

        public enum SubTypes
        {
            /// <summary>
            /// The time and date code relating to the expiration or end of useful life for a material or other physical item.
            /// </summary>
            EXPIRATION,

            /// <summary>
            /// The time and date code relating the first use of a material or other physical item.
            /// </summary>
            FIRST_USE,

            /// <summary>
            /// The time and date code relating to the production of a material or other physical item.
            /// </summary>
            MANUFACTURE
        }


        public DateCodeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public DateCodeDataItem(
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
                case SubTypes.EXPIRATION: return "expiration";
                case SubTypes.FIRST_USE: return "firstUse";
                case SubTypes.MANUFACTURE: return "manufacture";
            }

            return null;
        }
    }
}

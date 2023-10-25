// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Time and date code associated with a material or other physical item.
    /// </summary>
    public class DateCodeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DATE_CODE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Time and date code associated with a material or other physical item.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public enum SubTypes
        {
            /// <summary>
            /// Time and date code relating to the production of a material or other physical item.
            /// </summary>
            MANUFACTURE,
            
            /// <summary>
            /// Time and date code relating to the expiration or end of useful life for a material or other physical item.
            /// </summary>
            EXPIRATION,
            
            /// <summary>
            /// Time and date code relating the first use of a material or other physical item.
            /// </summary>
            FIRST_USE
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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.MANUFACTURE: return "Time and date code relating to the production of a material or other physical item.";
                case SubTypes.EXPIRATION: return "Time and date code relating to the expiration or end of useful life for a material or other physical item.";
                case SubTypes.FIRST_USE: return "Time and date code relating the first use of a material or other physical item.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.MANUFACTURE: return "MANUFACTURE";
                case SubTypes.EXPIRATION: return "EXPIRATION";
                case SubTypes.FIRST_USE: return "FIRST_USE";
            }

            return null;
        }

    }
}
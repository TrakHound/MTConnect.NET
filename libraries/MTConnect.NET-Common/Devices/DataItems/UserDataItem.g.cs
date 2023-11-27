// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of the person currently responsible for operating the piece of equipment.
    /// </summary>
    public class UserDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "USER";
        public const string NameId = "user";
             
        public new const string DescriptionText = "Identifier of the person currently responsible for operating the piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Identifier of the person currently responsible for operating the piece of equipment.
            /// </summary>
            OPERATOR,
            
            /// <summary>
            /// Identifier of the person currently responsible for performing maintenance on the piece of equipment.
            /// </summary>
            MAINTENANCE,
            
            /// <summary>
            /// Identifier of the person currently responsible for preparing a piece of equipment for production or restoring the piece of equipment to a neutral state after production.
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
                case SubTypes.OPERATOR: return "Identifier of the person currently responsible for operating the piece of equipment.";
                case SubTypes.MAINTENANCE: return "Identifier of the person currently responsible for performing maintenance on the piece of equipment.";
                case SubTypes.SET_UP: return "Identifier of the person currently responsible for preparing a piece of equipment for production or restoring the piece of equipment to a neutral state after production.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.OPERATOR: return "OPERATOR";
                case SubTypes.MAINTENANCE: return "MAINTENANCE";
                case SubTypes.SET_UP: return "SET_UP";
            }

            return null;
        }

    }
}
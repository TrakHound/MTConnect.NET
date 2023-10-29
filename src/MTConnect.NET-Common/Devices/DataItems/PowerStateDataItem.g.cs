// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication of the status of the source of energy for an entity to allow it to perform its intended function or the state of an enabling signal providing permission for the entity to perform its functions.
    /// </summary>
    public class PowerStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "POWER_STATE";
        public const string NameId = "powerState";
             
        public new const string DescriptionText = "Indication of the status of the source of energy for an entity to allow it to perform its intended function or the state of an enabling signal providing permission for the entity to perform its functions.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public enum SubTypes
        {
            /// <summary>
            /// State of the power source for the entity.
            /// </summary>
            LINE,
            
            /// <summary>
            /// State of the enabling signal or control logic that enables or disables the function or operation of the entity.
            /// </summary>
            CONTROL
        }


        public PowerStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PowerStateDataItem(
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
                case SubTypes.LINE: return "State of the power source for the entity.";
                case SubTypes.CONTROL: return "State of the enabling signal or control logic that enables or disables the function or operation of the entity.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LINE: return "LINE";
                case SubTypes.CONTROL: return "CONTROL";
            }

            return null;
        }

    }
}
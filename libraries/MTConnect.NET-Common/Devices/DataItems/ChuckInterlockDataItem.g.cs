// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// State of an interlock function or control logic state intended to prevent the associated Chuck component from being operated.
    /// </summary>
    public class ChuckInterlockDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CHUCK_INTERLOCK";
        public const string NameId = "chuckInterlock";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "State of an interlock function or control logic state intended to prevent the associated Chuck component from being operated.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public enum SubTypes
        {
            /// <summary>
            /// Indication of the state of an operator controlled interlock that can inhibit the ability to initiate an unclamp action of an electronically controlled chuck.When ChuckInterlockManualUnclamp is `ACTIVE`, it is expected that a chuck cannot be unclamped until ChuckInterlockManualUnclamp is set to `INACTIVE`.
            /// </summary>
            MANUAL_UNCLAMP
        }


        public ChuckInterlockDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public ChuckInterlockDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.MANUAL_UNCLAMP: return "Indication of the state of an operator controlled interlock that can inhibit the ability to initiate an unclamp action of an electronically controlled chuck.When ChuckInterlockManualUnclamp is `ACTIVE`, it is expected that a chuck cannot be unclamped until ChuckInterlockManualUnclamp is set to `INACTIVE`.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.MANUAL_UNCLAMP: return "MANUAL_UNCLAMP";
            }

            return null;
        }

    }
}
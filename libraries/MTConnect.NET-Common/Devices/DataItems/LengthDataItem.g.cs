// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Length of an object.
    /// </summary>
    public class LengthDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LENGTH";
        public const string NameId = "length";
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Length of an object.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public enum SubTypes
        {
            /// <summary>
            /// Standard or original length of an object.
            /// </summary>
            STANDARD,
            
            /// <summary>
            /// Remaining total length of an object.
            /// </summary>
            REMAINING,
            
            /// <summary>
            /// Remaining usable length of an object.
            /// </summary>
            USEABLE
        }


        public LengthDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public LengthDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = DefaultUnits;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.STANDARD: return "Standard or original length of an object.";
                case SubTypes.REMAINING: return "Remaining total length of an object.";
                case SubTypes.USEABLE: return "Remaining usable length of an object.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.STANDARD: return "STANDARD";
                case SubTypes.REMAINING: return "REMAINING";
                case SubTypes.USEABLE: return "USEABLE";
            }

            return null;
        }

    }
}
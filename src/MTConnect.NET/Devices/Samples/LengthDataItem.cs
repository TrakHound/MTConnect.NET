// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of the length of an object.
    /// </summary>
    public class LengthDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LENGTH";
        public const string NameId = "length";
        public const string DefaultUnits = Devices.Units.MILLIMETER;

        public enum SubTypes
        {
            /// <summary>
            /// The standard or original length of an object.
            /// </summary>
            STANDARD,

            /// <summary>
            /// The remaining total length of an object.
            /// </summary>
            REMAINING,

            /// <summary>
            /// The remaining useable length of an object.
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
            SubTypes subType = SubTypes.REMAINING
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = DefaultUnits;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            { 
                case SubTypes.STANDARD: return "std";
                case SubTypes.REMAINING: return "rem";
                case SubTypes.USEABLE: return "use";
            }

            return null;
        }
    }
}

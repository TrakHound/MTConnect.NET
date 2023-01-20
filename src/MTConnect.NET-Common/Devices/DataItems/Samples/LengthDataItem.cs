// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
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
        public new const string DescriptionText = "The measurement of the length of an object.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;

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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.STANDARD: return "The standard or original length of an object.";
                case SubTypes.REMAINING: return "The remaining total length of an object.";
                case SubTypes.USEABLE: return "The remaining useable length of an object.";
            }

            return null;
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

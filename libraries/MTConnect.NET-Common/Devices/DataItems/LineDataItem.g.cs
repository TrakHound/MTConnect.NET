// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Current line of code being executed.**DEPRECATED** in *Version 1.4.0*.
    /// </summary>
    public class LineDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LINE";
        public const string NameId = "line";
             
        public new const string DescriptionText = "Current line of code being executed.**DEPRECATED** in *Version 1.4.0*.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version14;
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public enum SubTypes
        {
            /// <summary>
            /// Maximum line number of the code being executed.
            /// </summary>
            MAXIMUM,
            
            /// <summary>
            /// Minimum line number of the code being executed.
            /// </summary>
            MINIMUM
        }


        public LineDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public LineDataItem(
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
                case SubTypes.MAXIMUM: return "Maximum line number of the code being executed.";
                case SubTypes.MINIMUM: return "Minimum line number of the code being executed.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.MAXIMUM: return "MAXIMUM";
                case SubTypes.MINIMUM: return "MINIMUM";
            }

            return null;
        }

    }
}
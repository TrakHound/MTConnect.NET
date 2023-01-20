// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// DEPRECATED in Version 1.4.0.
    /// </summary>
    public class LineDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LINE";
        public const string NameId = "line";
        public new const string DescriptionText = "DEPRECATED in Version 1.4.0.";

        public override string TypeDescription => DescriptionText;

        public override Version MaximumVersion => MTConnectVersions.Version13;


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


        protected override IDataItem OnProcess(IDataItem dataItem, Version mtconnectVersion)
        {
            if (!string.IsNullOrEmpty(SubType) && mtconnectVersion < MTConnectVersions.Version11) return null;

            return dataItem;
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
                case SubTypes.MAXIMUM: return "max";
                case SubTypes.MINIMUM: return "min";
            }

            return null;
        }
    }
}

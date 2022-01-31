// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An optional identifier for a BLOCK of code in a PROGRAM.
    /// </summary>
    public class LineLabelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LINE_LABEL";
        public const string NameId = "lineLabel";


        public LineLabelDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public LineLabelDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

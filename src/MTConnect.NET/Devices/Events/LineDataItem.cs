// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
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


        public LineDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public LineDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

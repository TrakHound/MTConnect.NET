// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An identifier of a part in a manufacturing operation.
    /// </summary>
    public class PartIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_COUNT";
        public const string NameId = "partCount";
        public new const string DescriptionText = "An identifier of a part in a manufacturing operation.";

        public override string TypeDescription => DescriptionText;


        public PartIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public PartIdDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

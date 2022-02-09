// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A set of limits defining a range of values designating acceptable performance for a variable.
    /// </summary>
    public class SpecificationLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SPECIFICATION_LIMIT";
        public const string NameId = "specificationLimit";


        public SpecificationLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public SpecificationLimitDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DataItemRepresentation.DATA_SET;
        }
    }
}

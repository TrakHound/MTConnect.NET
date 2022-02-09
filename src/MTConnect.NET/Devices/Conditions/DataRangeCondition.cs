// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication that the value of the data associated with a measured value or a calculation is outside of an expected range.
    /// </summary>
    public class DataRangeCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "DATA_RANGE";
        public const string NameId = "dataRange";


        public DataRangeCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public DataRangeCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

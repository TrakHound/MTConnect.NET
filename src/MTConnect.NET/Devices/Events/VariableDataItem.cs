// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A data value whose meaning may change over time due to changes in the operation
    /// of a piece of equipment or the process being executed on that piece of equipment.
    /// </summary>
    public class VariableDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "VARIABLE";
        public const string NameId = "var";


        public VariableDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public VariableDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

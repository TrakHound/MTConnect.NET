// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The identifier of the person currently responsible for operating the piece of equipment.
    /// </summary>
    public class OperatorIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "OPERATOR_ID";
        public const string NameId = "operatorId";


        public OperatorIdDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public OperatorIdDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

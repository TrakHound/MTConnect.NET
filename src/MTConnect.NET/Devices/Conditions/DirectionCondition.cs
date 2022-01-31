// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication of a fault associated with the direction of motion of a Structural Element
    /// </summary>
    public class DirectionCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "DIRECTION";
        public const string NameId = "dirCond";


        public DirectionCondition()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public DirectionCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

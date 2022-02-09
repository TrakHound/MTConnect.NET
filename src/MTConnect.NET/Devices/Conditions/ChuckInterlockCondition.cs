// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication of the operational condition of the interlock function for an electronically controller chuck.
    /// </summary>
    public class ChuckInterlockCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "CHUCK_INTERLOCK";
        public const string NameId = "chuckInterlockCond";


        public ChuckInterlockCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ChuckInterlockCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

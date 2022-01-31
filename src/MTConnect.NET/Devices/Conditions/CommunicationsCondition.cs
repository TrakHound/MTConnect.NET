// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication that the piece of equipment has experienced a communications failure.
    /// </summary>
    public class CommunicationsCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "COMMUNICATIONS";
        public const string NameId = "comms";


        public CommunicationsCondition()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public CommunicationsCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

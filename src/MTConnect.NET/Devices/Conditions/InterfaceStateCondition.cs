// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication of the operation condition of an Interface component.
    /// </summary>
    public class InterfaceStateCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "INTERFACE_STATE";
        public const string NameId = "interfaceStateCond";


        public InterfaceStateCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public InterfaceStateCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

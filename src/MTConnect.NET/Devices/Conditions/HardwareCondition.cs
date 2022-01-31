// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication of a fault associated with the hardware subsystem of the Structural Element.
    /// </summary>
    public class HardwareCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "HARDWARE";
        public const string NameId = "hardware";


        public HardwareCondition()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public HardwareCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

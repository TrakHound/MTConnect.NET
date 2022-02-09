// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The identifier for the type of wire used as the cutting mechanism in Electrical Discharge Machining or similar processes.
    /// </summary>
    public class WireDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WIRE";
        public const string NameId = "wire";


        public WireDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public WireDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

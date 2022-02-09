// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The URI of the Adapter.
    /// </summary>
    public class AdapterUriDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ADAPTER_URI";
        public const string NameId = "adapterUri";


        public AdapterUriDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }

        public AdapterUriDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

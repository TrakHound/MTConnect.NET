// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The originator’s software version of the Adapter.
    /// </summary>
    public class AdapterSoftwareVersionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ADAPTER_SOFTWARE_VERSION";
        public const string NameId = "adapterSoftwareVersion";


        public AdapterSoftwareVersionDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public AdapterSoftwareVersionDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

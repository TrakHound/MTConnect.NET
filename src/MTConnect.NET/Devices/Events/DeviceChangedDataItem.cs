// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// DeviceChanged is an Event that provides the UUID of the device whose Metadata has changed.
    /// </summary>
    public class DeviceChangedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEVICE_CHANGED";
        public const string NameId = "deviceChanged";


        public DeviceChangedDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public DeviceChangedDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

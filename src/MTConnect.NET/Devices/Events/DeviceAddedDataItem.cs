// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// DeviceAdded is an Event that provides the UUID of a new device added to an MTConnect Agent.
    /// </summary>
    public class DeviceAddedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEVICE_ADDED";
        public const string NameId = "deviceAdded";


        public DeviceAddedDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public DeviceAddedDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

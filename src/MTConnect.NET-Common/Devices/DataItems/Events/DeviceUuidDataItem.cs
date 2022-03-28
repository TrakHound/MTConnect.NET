// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The identifier of another piece of equipment that is temporarily associated with a component of this piece of equipment to perform a particular function.
    /// </summary>
    public class DeviceUuidDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEVICE_UUI";
        public const string NameId = "deviceUui";
        public new const string DescriptionText = "The identifier of another piece of equipment that is temporarily associated with a component of this piece of equipment to perform a particular function.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public DeviceUuidDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public DeviceUuidDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}

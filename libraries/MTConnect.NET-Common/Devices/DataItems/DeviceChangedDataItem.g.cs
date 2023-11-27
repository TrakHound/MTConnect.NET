// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// UUID of the device whose metadata has changed.
    /// </summary>
    public class DeviceChangedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEVICE_CHANGED";
        public const string NameId = "deviceChanged";
             
        public new const string DescriptionText = "UUID of the device whose metadata has changed.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public DeviceChangedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public DeviceChangedDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
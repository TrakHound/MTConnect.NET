// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605103593648_589050_423

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// UUID of new device added to an MTConnect Agent.
    /// </summary>
    public class DeviceAddedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEVICE_ADDED";
        public const string NameId = "deviceAdded";
             
             
        public new const string DescriptionText = "UUID of new device added to an MTConnect Agent.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public DeviceAddedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public DeviceAddedDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
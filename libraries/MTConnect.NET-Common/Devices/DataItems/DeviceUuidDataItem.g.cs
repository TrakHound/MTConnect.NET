// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218277_625034_1752

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of another piece of equipment that is temporarily associated with a component of this piece of equipment to perform a particular function.
    /// </summary>
    public class DeviceUuidDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DEVICE_UUID";
        public const string NameId = "deviceUuid";
             
             
        public new const string DescriptionText = "Identifier of another piece of equipment that is temporarily associated with a component of this piece of equipment to perform a particular function.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public DeviceUuidDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public DeviceUuidDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
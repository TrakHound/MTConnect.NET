// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218441_671480_2166

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Serial number associated with a Component, Asset, or Device.
    /// </summary>
    public class SerialNumberDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SERIAL_NUMBER";
        public const string NameId = "serialNumber";
             
             
        public new const string DescriptionText = "Serial number associated with a Component, Asset, or Device.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public SerialNumberDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public SerialNumberDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
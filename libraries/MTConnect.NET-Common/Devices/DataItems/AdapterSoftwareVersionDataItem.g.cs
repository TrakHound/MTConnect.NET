// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605104021797_851627_751

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Originator’s software version of the adapter.
    /// </summary>
    public class AdapterSoftwareVersionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ADAPTER_SOFTWARE_VERSION";
        public const string NameId = "adapterSoftwareVersion";
             
             
        public new const string DescriptionText = "Originator’s software version of the adapter.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public AdapterSoftwareVersionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public AdapterSoftwareVersionDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
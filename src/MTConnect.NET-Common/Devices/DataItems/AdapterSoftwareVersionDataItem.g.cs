// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Originator’s software version of the adapter.
    /// </summary>
    public class AdapterSoftwareVersionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ADAPTER_SOFTWARE_VERSION";
        public const string NameId = "";
             
        public new const string DescriptionText = "Originator’s software version of the adapter.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public AdapterSoftwareVersionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
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
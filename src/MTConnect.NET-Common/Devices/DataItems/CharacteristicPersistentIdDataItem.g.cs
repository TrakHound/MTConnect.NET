// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// UUID of the characteristic.
    /// </summary>
    public class CharacteristicPersistentIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CHARACTERISTIC_PERSISTENT_ID";
        public const string NameId = "";
             
        public new const string DescriptionText = "UUID of the characteristic.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public CharacteristicPersistentIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public CharacteristicPersistentIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
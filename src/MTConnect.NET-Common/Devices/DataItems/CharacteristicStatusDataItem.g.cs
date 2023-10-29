// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Pass/fail result of the measurement.
    /// </summary>
    public class CharacteristicStatusDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CHARACTERISTIC_STATUS";
        public const string NameId = "characteristicStatus";
             
        public new const string DescriptionText = "Pass/fail result of the measurement.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public CharacteristicStatusDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public CharacteristicStatusDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
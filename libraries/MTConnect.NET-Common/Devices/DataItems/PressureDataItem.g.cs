// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Force per unit area measured relative to atmospheric pressure. Commonly referred to as gauge pressure.
    /// </summary>
    public class PressureDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PRESSURE";
        public const string NameId = "pressure";
             
        public const string DefaultUnits = Devices.Units.PASCAL;     
        public new const string DescriptionText = "Force per unit area measured relative to atmospheric pressure. Commonly referred to as gauge pressure.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public PressureDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public PressureDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}
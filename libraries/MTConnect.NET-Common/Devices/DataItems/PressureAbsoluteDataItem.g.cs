// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Force per unit area measured relative to a vacuum.
    /// </summary>
    public class PressureAbsoluteDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PRESSURE_ABSOLUTE";
        public const string NameId = "pressureAbsolute";
        public const string DefaultUnits = Devices.Units.PASCAL;     
        public new const string DescriptionText = "Force per unit area measured relative to a vacuum.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public PressureAbsoluteDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public PressureAbsoluteDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
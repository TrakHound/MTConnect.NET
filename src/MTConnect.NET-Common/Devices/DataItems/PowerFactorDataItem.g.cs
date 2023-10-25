// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Ratio of real power flowing to a load to the apparent power in that AC circuit.
    /// </summary>
    public class PowerFactorDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "POWER_FACTOR";
        public const string NameId = "";
        public const string DefaultUnits = Devices.Units.PERCENT;     
        public new const string DescriptionText = "Ratio of real power flowing to a load to the apparent power in that AC circuit.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public PowerFactorDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public PowerFactorDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
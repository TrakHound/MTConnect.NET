// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the ratio of real power flowing to a load to the apparent power in that AC circuit.
    /// </summary>
    public class PowerFactorDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "POWER_FACTOR";
        public const string NameId = "pwrFactor";
        public const string DefaultUnits = Devices.Units.PERCENT;
        public new const string DescriptionText = "The measurement of the ratio of real power flowing to a load to the apparent power in that AC circuit.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        public PowerFactorDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public PowerFactorDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
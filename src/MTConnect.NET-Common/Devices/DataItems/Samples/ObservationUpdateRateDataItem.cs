// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The average rate of change of values for data items in the MTConnect streams.
    /// The average is computed over a rolling window defined by the implementation.
    /// </summary>
    public class ObservationUpdateRateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "OBSERVATION_UPDATE_RATE";
        public const string NameId = "obsvrUpdateRate";
        public const string DefaultUnits = Devices.Units.COUNT_PER_SECOND;
        public new const string DescriptionText = "The average rate of change of values for data items in the MTConnect streams. The average is computed over a rolling window defined by the implementation.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ObservationUpdateRateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
            SignificantDigits = 1;
        }

        public ObservationUpdateRateDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
            SignificantDigits = 1;
        }
    }
}

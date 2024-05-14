// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605105279989_668208_1197

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Average rate of change of values for data items in the MTConnect streams. The average is computed over a rolling window defined by the implementation.
    /// </summary>
    public class ObservationUpdateRateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "OBSERVATION_UPDATE_RATE";
        public const string NameId = "observationUpdateRate";
             
        public const string DefaultUnits = Devices.Units.COUNT_PER_SECOND;     
        public new const string DescriptionText = "Average rate of change of values for data items in the MTConnect streams. The average is computed over a rolling window defined by the implementation.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public ObservationUpdateRateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ObservationUpdateRateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}
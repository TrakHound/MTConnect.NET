// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1678250724207_274165_18570

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Engineering units of the measurement.
    /// </summary>
    public class MeasurementUnitsDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MEASUREMENT_UNITS";
        public const string NameId = "measurementUnits";
             
             
        public new const string DescriptionText = "Engineering units of the measurement.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public MeasurementUnitsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public MeasurementUnitsDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}
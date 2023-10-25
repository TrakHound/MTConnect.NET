// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Measurement based on the measurement type.
    /// </summary>
    public class MeasurementValueDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MEASUREMENT_VALUE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Measurement based on the measurement type.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public MeasurementValueDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public MeasurementValueDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
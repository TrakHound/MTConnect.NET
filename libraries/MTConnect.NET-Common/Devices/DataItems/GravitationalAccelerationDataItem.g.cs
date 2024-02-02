// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Acceleration relative to Earth's gravity of 9.80665 `METER/SECOND^2`.
    /// </summary>
    public class GravitationalAccelerationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "GRAVITATIONAL_ACCELERATION";
        public const string NameId = "gravitationalAcceleration";
             
        public const string DefaultUnits = Devices.NativeUnits.GRAVITATIONAL_ACCELERATION;     
        public new const string DescriptionText = "Acceleration relative to Earth's gravity of 9.80665 `METER/SECOND^2`.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public GravitationalAccelerationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public GravitationalAccelerationDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}
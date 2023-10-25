// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The dimension between two parallel tangents on the outside edge of a flange.
    /// </summary>
    public class FlangeDiameterMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "FlangeDiameter";
        public const string CodeId = "DF";


        public FlangeDiameterMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public FlangeDiameterMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public FlangeDiameterMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
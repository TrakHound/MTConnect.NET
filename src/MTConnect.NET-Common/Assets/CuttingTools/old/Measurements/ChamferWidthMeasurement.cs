// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The width of the chamfer.
    /// </summary>
    public class ChamferWidthMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "ChamferWidth";
        public const string CodeId = "CHW";


        public ChamferWidthMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public ChamferWidthMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            Value = value;
        }

        public ChamferWidthMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
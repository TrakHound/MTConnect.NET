// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// Angle between the driving mechanism locator on a Tool Item and the main cutting edge.
    /// </summary>
    public class DriveAngleMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "DriveAngle";
        public const string CodeId = "DRVA";


        public DriveAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }

        public DriveAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
            Value = value;
        }

        public DriveAngleMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }
    }
}

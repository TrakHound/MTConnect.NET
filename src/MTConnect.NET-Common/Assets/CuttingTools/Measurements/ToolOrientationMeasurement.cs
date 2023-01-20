// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// The angle of the tool with respect to the workpiece for a given process.The value is application specific.
    /// </summary>
    public class ToolOrientationMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "ToolOrientation";


        public ToolOrientationMeasurement()
        {
            Type = TypeId;
            Units = Devices.Units.DEGREE;
        }

        public ToolOrientationMeasurement(double value)
        {
            Type = TypeId;
            Units = Devices.Units.DEGREE;
            Value = value;
        }

        public ToolOrientationMeasurement(Measurement measurement) : base(measurement)
        {
            Type = TypeId;
            Units = Devices.Units.DEGREE;
        }
    }
}

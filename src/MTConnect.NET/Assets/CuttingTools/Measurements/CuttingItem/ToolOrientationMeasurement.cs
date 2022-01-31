// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.CuttingItem
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
            CDATA = value;
        }
    }
}

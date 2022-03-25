// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.CuttingItem
{
    /// <summary>
    /// The angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
    /// </summary>
    public class PointAngleMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "PointAngle";
        public const string CodeId = "SIG";


        public PointAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
        }

        public PointAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.DEGREE;
            CDATA = value;
        }
    }
}

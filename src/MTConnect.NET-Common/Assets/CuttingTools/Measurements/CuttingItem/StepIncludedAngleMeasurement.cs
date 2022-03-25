// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.CuttingItem
{
    /// <summary>
    /// The angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
    /// </summary>
    public class StepIncludedAngleMeasurement : CuttingItemMeasurement
    {
        public const string TypeId = "StepIncludedAngle";
        public const string CodeId = "STAx";


        public StepIncludedAngleMeasurement()
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
        }

        public StepIncludedAngleMeasurement(double value)
        {
            Type = TypeId;
            Code = CodeId;
            Units = Devices.Units.MILLIMETER;
            CDATA = value;
        }
    }
}

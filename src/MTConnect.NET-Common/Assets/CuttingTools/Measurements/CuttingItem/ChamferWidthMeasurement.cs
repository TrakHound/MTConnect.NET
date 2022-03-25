// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools.Measurements.CuttingItem
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
            CDATA = value;
        }
    }
}
